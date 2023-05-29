using GeekyGadgets.DAL;
using GeekyGadgets.DAL.Interfaces;
using GeekyGadgets.DAL.Repositories;
using GeekyGadgets.Domain.Entity;
using GeekyGadgets.Service.Implementations;
using GeekyGadgets.Service.Interfaces;
using GeekyGadgets.Service.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using System;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = false;
});

// Регистрация сервисов
builder.Services.AddScoped<IBaseRepository<User>, UserRepository>();
builder.Services.AddScoped<IBaseRepository<Profile>, ProfileRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddSingleton<JwtSettings>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<HttpService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddScoped<ISmartphoneService, SmartphoneService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IBaseRepository<Smartphone>, SmartphoneRepository>();
builder.Services.AddScoped<IBaseRepository<Basket>, BasketRepository>();
builder.Services.AddScoped<IBaseRepository<Order>, OrderRepository>();


//builder.Services.AddScoped<IUserRepository, AccountService>();



builder.Services.AddAuthorization();

builder.Services.AddCors();

builder.Services.AddHttpClient();

// Настройка аутентификации и авторизации через JWT токены
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Чтение токена из заголовка Authorization
            if (context.Request.Headers.TryGetValue("Authorization", out var token))
            {
                context.Token = token.ToString().Replace("Bearer ", "");
            }
            return Task.CompletedTask;
        }
    };
}).AddCookie("token", options =>
{
    options.Cookie.Name = "token";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
});



// Настройка заголовков безопасности
builder.Services.AddHsts(options =>
{
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Настройка заголовков безопасности
app.Use(async (context, next) =>
{
    // Директива CSP
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; frame-ancestors 'self'; form-action 'self'");

    // Заголовок X-Frame-Options
    context.Response.Headers.Add("X-Frame-Options", "DENY");

    // Заголовок X-Content-Type-Options
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

    // Заголовок Cache-Control
    context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");

    if (context.Request.Cookies.TryGetValue("token", out var token))
    {
        // Установка заголовка Authorization на основе значения токена из куки
        context.Request.Headers["Authorization"] = "Bearer " + token;
    }

    await next.Invoke();
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();




