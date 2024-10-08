using System.Globalization;
using System.Text.Json.Serialization;
using Application.Common.Interfaces;
using Azure.Identity;
using FluentValidation.AspNetCore;
using GovUk.Frontend.AspNetCore;
using Infrastructure.Analytics;
using Infrastructure.Configuration;
using Infrastructure.DataImport;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using UI.Constants;
using UI.Filters;
using UI.Routing;
using UI.Services;
using AppEnvironmentVariables = Infrastructure.Constants.EnvironmentVariables;
using AssemblyReference = UI.AssemblyReference;

// Data import is a stand-alone process that should terminate once completed
if (await Import.RunImport(args)) return;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

if (!string.IsNullOrEmpty(builder.Configuration[AppEnvironmentVariables.FatpAzureKeyVaultName]))
{
    var keyVaultName = builder.Configuration[AppEnvironmentVariables.FatpAzureKeyVaultName];
    builder.Configuration.AddAzureKeyVault(new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
}

builder.AddEnvironmentConfiguration();
builder.Services.AddHttpContextAccessor();

builder.Host.AddLogging();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.EnableDebugLogger = false; // Disable standard logging
});

builder.Services.AddDistributedCache(builder.Configuration);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(DoubleConstants.SessionTimeoutInMinutes);
    options.Cookie.IsEssential = true;
    options.Cookie.Name = Application.Constants.StringConstants.SessionCookieName;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddScoped<ITuitionPartnerCompareListStorageService, CookieBasedTuitionPartnerCompareListStorageService>();
builder.Services.AddScoped<ISessionService, DistributedSessionService>();

// Rename add and rename cookies for application
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = ".FindATuitionPartner.Antiforgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Add services to the container.
builder.Services.AddNtpDbContext(builder.Configuration);
builder.Services.AddLocationFilterService();
builder.Services.AddServices();
builder.Services.AddNotificationConfig(builder.Configuration)
    .AddNotificationClientServiceConfiguration(builder.Configuration);
builder.Services.AddEmailSettingsConfig(builder.Configuration);
builder.Services.AddFeatureFlagConfig(builder.Configuration);
builder.Services.AddSingleton<IConfigureOptions<ServiceUnavailableSettings>, ServiceUnavailableSettingsConfigure>();
builder.Services.AddRepositories();
builder.Services.AddCqrs();
builder.Services.LogKeyMetrics();

builder.Services.AddMediatR(typeof(AssemblyReference));

builder.Services.AddGovUkFrontend(options =>
{
    options.AddImportsToHtml = false;
});

builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new SeoRouteConvention()));
        options.Filters.Add<FluentValidationExceptionAttribute>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddRazorPages(options =>
    {
        options.Conventions.Add(new SeoRouteConvention());
    })
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = false;
    })
    .AddMvcOptions(options =>
    {
        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => "The value is invalid.");
        options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(s => "The value is invalid.");
        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => "The value is invalid.");
        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => "The value is invalid.");
        options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(x => "The value is invalid.");
        options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(x => "The value is invalid.");
        options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => "The value is invalid.");
    });

builder.Services.AddHttpContextAccessor();

// Supports both data annotation based validation as well as more complex cross property validation using the fluent validation library
builder.Services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly).AddFluentValidationAutoValidation();//.AddFluentValidationClientsideAdapters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(x => x.FullName);
});

var connectionString = builder.Configuration.GetNtpConnectionString();

builder.Services.AddHealthChecks()
    .AddCheck("Custom PostgreSQL check", new PostgreSqlCustomHealthCheck(connectionString), HealthStatus.Unhealthy, tags: new[] { "db", "postgresql" });

builder.AddAnalytics();

var app = builder.Build();

Initialize(app.Services.GetRequiredService<ILoggerFactory>());


app.UseMiddleware<ExceptionLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAnalytics();

// Handle runtime exceptions withing the application
app.UseExceptionHandler("/Error");

// Handle status code exceptions like page not found
app.UseStatusCodePagesWithReExecute("/Error", "?Status={0}");

app.UseHttpsRedirection();

// Retire the site from midnight on 1st September by redirecting all traffic to a gov.uk page about tutoring
app.Use(async (context, next) =>
{
    // 00:00 BST on 1st September 2024 is 23:00 UTC on 31st August
    if (DateTime.UtcNow >= new DateTime(2024, 8, 31, 23, 0, 0))
    {
        context.Response.Redirect("https://www.gov.uk/government/publications/tutoring-in-education-settings");
        return;
    }

    await next();
});

var cacheMaxAgeOneWeek = (60 * 60 * 24 * 7).ToString();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cacheMaxAgeOneWeek}");
    }
});

app.UseRouting();

app.EnsureDistributedCacheIsUsed(!app.Environment.IsDevelopment() && !app.Environment.IsTesting());

app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

var policyCollection = new HeaderPolicyCollection()
    .AddContentTypeOptionsNoSniff()
    .AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365) // maxage = one year in seconds
    .AddFrameOptionsDeny()
    .AddReferrerPolicyStrictOriginWhenCrossOrigin()
    .RemoveServerHeader()
    .AddContentSecurityPolicy(cspBuilder =>
        {
            // configure policies
            cspBuilder.AddBaseUri() // base-uri 'self'
                .Self();
            cspBuilder.AddBlockAllMixedContent(); // block-all-mixed-content
            cspBuilder.AddDefaultSrc() // default-src 'self'
                .Self();
            cspBuilder.AddImgSrc() // img-src 'self'
                .Self();
            cspBuilder.AddFrameSrc() // frame-src 'self' https://www.googletagmanager.com
                .Self()
                .From(new List<string>() { "https://www.googletagmanager.com", "https://forms.office.com" })
                .WithNonce();
            cspBuilder.AddMediaSrc() // media-src 'none'
                .None();
            cspBuilder.AddObjectSrc() // object-src 'none'
                .None();

            var scriptBuilder = cspBuilder.AddScriptSrc() // script-src 'self' 'https://www.googletagmanager.com'
                .Self()
                .From("https://www.googletagmanager.com")
                .WithNonce();

            cspBuilder.AddFontSrc() // font-src 'self'
                .Self();

            var styleBuilder = cspBuilder.AddStyleSrc() // style-src 'self' 'strict-dynamic'
                .Self()
                .StrictDynamic()
                .WithHashTagHelper(); // Allow whitelisted elements based on their SHA256 hash value

            var connectBuilder = cspBuilder.AddConnectSrc() // connect-src 'self' https://*.google-analytics.com
                .Self()
                .From("https://*.google-analytics.com");

            if (app.Environment.IsDevelopment())
            {
                // Support webpack development mode used in npm run build:dev and nom run watch
                scriptBuilder.UnsafeEval();

                // Visual Studio Browser Link
                styleBuilder.UnsafeInline();

                // For hot reload and similar developer tooling
                connectBuilder
                    .From("http://localhost:*")
                    .From("https://localhost:*")
                    .From("ws://localhost:*")
                    .From("wss://localhost:*");
            }

            cspBuilder.AddUpgradeInsecureRequests(); // upgrade-insecure-requests
            cspBuilder.AddFormAction() // form-action 'self'
                .Self();
            cspBuilder.AddFrameAncestors() // frame-ancestors 'none'
                .None();
        });
app.UseSecurityHeaders(policyCollection);

// Ensure all date and currency formatting is set to UK/GB
var cultureInfo = new CultureInfo("en-GB");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health");
});

app.Use(async (context, next) =>
{
    if ((string)context.Request.Path != "/service-unavailable" &&
        builder.Configuration.IsServiceUnavailable())
    {
        context.Response.Redirect("/service-unavailable");
        return;
    }

    await next();
});

// If our application gets hit really hard, then threads need to be spawned
// By default the number of threads that exist in the threadpool is the amount of CPUs (1)
// Each time we have to spawn a new thread it gets delayed by 500ms
// Setting the min higher means there will not be that delay in creating threads up to the min
// Re-evaluate this based on performance tests
// Found because redis kept timing out because it was delayed too long waiting for a thread to execute
ThreadPool.SetMinThreads(400, 400);

app.Run();

namespace UI
{
    public partial class Program
    { }
}