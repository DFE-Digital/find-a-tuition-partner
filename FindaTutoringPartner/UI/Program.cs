using System.Text.Json.Serialization;
using Application.Extensions;
using FluentValidation.AspNetCore;
using GovUk.Frontend.AspNetCore;
using Infrastructure;
using Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UI.Filters;
using AssemblyReference = UI.AssemblyReference;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddNtpDbContext(builder.Configuration);
builder.Services.AddAddressLookup();
builder.Services.AddCqrs();

builder.Services.AddMediatR(typeof(AssemblyReference));

builder.Services.AddGovUkFrontend();

builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add<FluentValidationExceptionFilterAttribute>();
    }).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    // Supports both data annotation based validation as well as more complex cross property validation using the fluent validation library
    .AddFluentValidation(options => options.RegisterValidatorsFromAssembly(typeof(AssemblyReference).Assembly));

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

app.Run();
