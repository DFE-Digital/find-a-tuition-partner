using System.Globalization;
using System.Text.Json.Serialization;
using Application.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using GovUk.Frontend.AspNetCore;
using Infrastructure;
using Infrastructure.Configuration;
using Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog.Events;
using UI.Filters;
using UI.Routing;
using AssemblyReference = UI.AssemblyReference;

if (args.Any(x => x == "import"))
{
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.Configure<DataEncryption>(hostContext.Configuration.GetSection(nameof(DataEncryption)));
            services.AddOptions();
            services.AddNtpDbContext(hostContext.Configuration);
            services.AddDataImporter();
            services.AddHostedService<DataImporterService>();
        })
        .AddLogging(LogEventLevel.Warning)
        .Build();

    await host.RunAsync();

    return;
}

var builder = WebApplication.CreateBuilder(args);

// Rename add and rename cookies for application
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = ".FindATuitionPartner.Antiforgery";
});

builder.Services.Configure<CookieTempDataProviderOptions>(options => options.Cookie.Name = ".FindATuitionPartner.Mvc.CookieTempDataProvider");


// Add services to the container.
builder.Services.AddNtpDbContext(builder.Configuration);
builder.Services.AddLocationFilterService();
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
builder.Services.AddSwaggerGen();

builder.Host.AddLogging();

var app = builder.Build();

app.UseMiddleware<ExceptionLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
    .AddContentSecurityPolicy(builder =>
        {
            // configure policies
            builder.AddUpgradeInsecureRequests(); // upgrade-insecure-requests
            builder.AddBlockAllMixedContent(); // block-all-mixed-content
            builder.AddDefaultSrc() // default-src 'self' http://testUrl.com
                .Self();
        },
        asReportOnly: true); // report-only
app.UseSecurityHeaders(policyCollection);

// Ensure all date and currency formatting is set to UK/GB
var cultureInfo = new CultureInfo("en-GB");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.Run();

public partial class Program
{ }