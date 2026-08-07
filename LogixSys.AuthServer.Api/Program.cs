using LogixSys.AuthServer.Api.Extensions;
using LogixSys.AuthServer.Api.Infrastructure;
using LogixSys.AuthServer.Application.DependencyInjection;
using LogixSys.AuthServer.Infrastructure.DependencyInjection;
using LogixSys.AuthServer.Persistence.DependencyInjection;
using LogixSys.AuthServer.Api.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddApiServices();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority =
            "https://localhost:7128";

        options.RequireHttpsMetadata = true;

        options.Audience = "LogixSys.Api";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseRouting();

app.UseApiPipeline();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

using (var scope = app.Services.CreateScope())
{
    await OpenIddictSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();

