using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Reservas_Laboratorio.Data;
using Reservas_Laboratorio.MappingProfiles;
using Reservas_Laboratorio.Middleware;
using Reservas_Laboratorio.Models.Settings;
using Reservas_Laboratorio.Repositories;
using Reservas_Laboratorio.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();


// Leer configuración JWT y SMTP del appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));


// ===================== AUTO MAPPER =====================

// Update the AutoMapper registration to use the correct overload that accepts an assembly.  
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UserProfile).Assembly));
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ReservaProfile).Assembly));

// ===================== CONTEXT Y DB =====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbReserva")));

// ===================== SERVICIOS =====================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IReservaService, ReservaService>();

// ===================== COOKIE & SESSION =====================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options => {
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("access_token"))
                context.Token = context.Request.Cookies["access_token"];
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/Auth/Login");
                context.HandleResponse(); // evita el comportamiento por defecto
            }
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/Auth/AccessDenied");

            }
            return Task.CompletedTask;
        }
    };

    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == 404 && !response.HasStarted)
    {
        response.Redirect("/Home/NotFoundPage");
    }
    if (response.StatusCode == 403 && !response.HasStarted)
    {
        response.Redirect("/Auth/AccessDenied");
    }
    if (response.StatusCode == 401 && !response.HasStarted)
    {
        response.Redirect("/Auth/Login");
    }

    await Task.CompletedTask;
});

app.UseRouting();
app.UseSession();
app.UseMiddleware<JwtRefreshMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Middleware para manejar redirecciones de autenticación
app.Use(async (context, next) =>
{
    await next();

    // Manejar redirección para no autenticados (401)
    if (context.Response.StatusCode == 401 && !context.Response.HasStarted)
    {
        Console.WriteLine("Redireccionando a login...");
        context.Response.Redirect($"/Auth/Login?returnUrl={context.Request.Path}");
    }
    // Manejar acceso denegado (403)
    else if (context.Response.StatusCode == 403 && !context.Response.HasStarted)
    {
        context.Response.Redirect("/Auth/AccessDenied");
    }
});

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
