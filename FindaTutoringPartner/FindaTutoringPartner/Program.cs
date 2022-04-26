using Application;
using FindaTutoringPartner;
using GovUk.Frontend.AspNetCore;
using Infrastructure;
using Mapster;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//TODO: Move registering application implementations to extension method in infrastructure
builder.Services.AddTransient<IAddressLookup, HardCodedAddressLookup>();

builder.Services.AddMediatR(typeof(AssemblyReference));

builder.Services.AddGovUkFrontend();

builder.Services.AddControllersWithViews();

var app = builder.Build();

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
