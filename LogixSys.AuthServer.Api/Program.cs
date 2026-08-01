using LogixSys.AuthServer.Api.Extensions;
using LogixSys.AuthServer.Application.DependencyInjection;
using LogixSys.AuthServer.Infrastructure.DependencyInjection;
using LogixSys.AuthServer.Persistence.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddApiServices();

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenIddict();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/account/login";
    });

builder.Services.AddAuthorization();

var app = builder.Build();


app.UseHttpsRedirection();

app.UseRouting();

app.UseApiPipeline();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();

