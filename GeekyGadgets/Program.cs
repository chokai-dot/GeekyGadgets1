using Automarket.DAL;
using Automarket.DAL.Repositories;
using Automarket.Service.Implementations;
using Automarket.Service.JWT;
using GeekyGadgets.DAL;
using GeekyGadgets.DAL.Interfaces;
using GeekyGadgets.DAL.Repositories;
using GeekyGadgets.Domain.Entity;
using GeekyGadgets.Service.Implementations;
using GeekyGadgets.Service.Interfaces;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(connection));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(context.Exception);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine(context.Error);
                return Task.CompletedTask;
            }
        };
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/accessdenied";
    });
// Чтение настроек из appsettings.json
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
//
// Регистрация JwtService
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IBaseRepository<User>, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<HttpService>();
//builder.Services.AddScoped<HttpService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();

//builder.Services.AddJwtAuthentication(Configuration);

builder.Services.AddScoped<IBaseRepository<User>, UserRepository>();

//builder.Services.AddScoped<IAccountService, AccountService>();
//builder.Services.InitializeRepositories();
//builder.Services.InitializeServices();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var endpoint = context.GetEndpoint();
    if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null ||
        endpoint?.Metadata.GetMetadata<IAuthorizeData>() == null)
    {
        await next();
        return;
    }

    // Check if the request has a token in the Authorization header
    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
    {
        var token = authHeader.ToString().Replace("Bearer ", "");
        if (!string.IsNullOrEmpty(token))
        {
            // Validate and decode the token
            var jwtService = context.RequestServices.GetService<JwtService>();
            var isValid = jwtService.ValidateToken(token);
            if (isValid != null)
            {
                var claimsPrincipal = await jwtService.ValidateToken(token);
                if (claimsPrincipal != null)
                {
                    // Set the user identity and continue with the request pipeline
                    var identity = new ClaimsIdentity(claimsPrincipal.Claims, "jwt");
                    context.User = new ClaimsPrincipal(identity);
                    await next();
                    return;
                }
            }
            if (!context.User.Identity.IsAuthenticated)
            {
                // If the user is not authenticated, redirect to the login page
                context.Response.StatusCode = 401;
                context.Response.Headers["Location"] = "/User/Login";
                await context.Response.WriteAsync("Unauthorized");
                return;
            }


        }
    }

    // If the request does not have a valid token, return a 401 Unauthorized response
   // context.Response.StatusCode = 401;
});


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();




//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//{
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

//});