using System.Text.Json.Serialization;
using Application.Extensions;
using FluentValidation.AspNetCore;
using GovUk.Frontend.AspNetCore;
using Infrastructure;
using Infrastructure.Configuration;
using Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
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
        .Build();

    await host.RunAsync();

    return;
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddNtpDbContext(builder.Configuration);
builder.Services.AddLocationFilterService();
builder.Services.AddRepositories();
builder.Services.AddCqrs();

builder.Services.AddMediatR(typeof(AssemblyReference));

builder.Services.AddGovUkFrontend(new GovUkFrontendAspNetCoreOptions()
{
    AddImportsToHtml = false
});

builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new SeoRouteConvention()));
        options.Filters.Add<FluentValidationExceptionAttribute>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    // Supports both data annotation based validation as well as more complex cross property validation using the fluent validation library
    .AddFluentValidation(options => options.RegisterValidatorsFromAssembly(typeof(AssemblyReference).Assembly));

builder.Services.AddRazorPages(options =>
    {
        options.Conventions.Add(new SeoRouteConvention());
    })
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = false;
    })
    // Supports both data annotation based validation as well as more complex cross property validation using the fluent validation library
    .AddFluentValidation(options => options.RegisterValidatorsFromAssembly(typeof(AssemblyReference).Assembly));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.AddLogging(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.Run();

public partial class Program { }
