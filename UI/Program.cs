using System.Globalization;
using System.Text.Json.Serialization;
using Application.Common.Interfaces;
using FluentValidation.AspNetCore;
using GovUk.Frontend.AspNetCore;
using Infrastructure.Analytics;
using Infrastructure.DataImport;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using UI.Constants;
using UI.Filters;
using UI.Routing;
using UI.Services;
using AssemblyReference = UI.AssemblyReference;

// Data import is a stand-alone process that should terminate once completed
if (await Import.RunImport(args)) return;

var builder = WebApplication.CreateBuilder(args);
builder.AddEnvironmentConfiguration();
builder.Services.AddHttpContextAccessor();

builder.Host.AddLogging();

builder.Services.AddDistributedCache(builder.Configuration);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(DoubleConstants.SessionTimeoutInMinutes);
    options.Cookie.IsEssential = true;
    options.Cookie.Name = StringConstants.SessionCookieName;
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
    .AddXssProtectionBlock()
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

app.Run();

namespace UI
{
    public partial class Program
    { }
}