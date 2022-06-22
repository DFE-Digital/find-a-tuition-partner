using System.Text.Json.Serialization;
using Application;
using Application.Extensions;
using FluentValidation.AspNetCore;
using GovUk.Frontend.AspNetCore;
using Infrastructure;
using Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using UI.Filters;
using UI.Routing;
using AssemblyReference = UI.AssemblyReference;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddNtpDbContext(builder.Configuration);
builder.Services.AddSearchRequestBuilder();
builder.Services.AddRepositories();
builder.Services.AddCqrs();
builder.Services.AddRegionalTuitionPartnerData();

builder.Services.AddMediatR(typeof(AssemblyReference));

builder.Services.AddGovUkFrontend(new GovUkFrontendAspNetCoreOptions()
{
    AddImportsToHtml = false
});

builder.Services.AddControllersWithViews(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new SeoRouteConvention()));
        options.Filters.Add<FluentValidationExceptionAttribute>();
    }).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    // Supports both data annotation based validation as well as more complex cross property validation using the fluent validation library
    .AddFluentValidation(options => options.RegisterValidatorsFromAssembly(typeof(AssemblyReference).Assembly));

builder.Services.AddRazorPages(options => options.Conventions.Add(new SeoRouteConvention()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // This is fine for development and while we're spiking but an on demand migration run will be required for production
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<NtpDbContext>();
    db.Database.Migrate();

    var importer = scope.ServiceProvider.GetRequiredService<ITuitionPartnerDataImporter>();
    var localImporter = scope.ServiceProvider.GetRequiredService<ITuitionPartnerLocalRegionDataImporter>();
    importer.Import();
    //Disabling the import file for local regions
   // localImporter.Import();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

public partial class Program { }
