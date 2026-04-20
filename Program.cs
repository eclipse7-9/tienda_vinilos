using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.Middleware;
using MiPrimeraAPI.Services;
using System.Text;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Npgsql;


var builder = WebApplication.CreateBuilder(args);


// Controladores
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
   {
       c.SwaggerDoc("v1", new() { Title = "MediaStore API", Version = "v1" });

       c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
       {
           Name = "Authorization",
           Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
           Scheme = "Bearer",
           BearerFormat = "JWT",
           In = Microsoft.OpenApi.Models.ParameterLocation.Header,
           Description = "Ingrese el token JWT en el formato: Bearer {token}"
       });

       c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
   });

// DbContext  conexin a LocalDB
builder.Services.AddDbContext<MediaStoreContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(new PerformanceInterceptor());
});

// token
builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens
        .TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//Servicio de telemetría

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("MiPrimeraAPI")
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MiPrimeraAPI"))
        .AddAspNetCoreInstrumentation() 
        // Cambiamos SqlClient por Npgsql para que coincida con tu DB
        .AddSource("Npgsql") 
        // Añadimos esto para poder ver los resultados en la consola de Cloud Shell
        .AddConsoleExporter());

//Servicio de Redis

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "MediaStore_";
});

var app = builder.Build();

// 1. CORS DEBE SER LO PRIMERO. 
// Esto responde a los navegadores antes de que el Middleware de Error o Auth intervengan.
app.UseCors("FrontendPolicy");

// 2. Middleware de error después de CORS
app.UseMiddleware<ErrorMiddleware>();

// 3. Swagger y demás
if (app.Environment.IsDevelopment() || app.Configuration["EnableSwagger"] == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Archivos estáticos
app.UseStaticFiles();

// 4. Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Redirección para el panel
app.MapGet("/panel-productos", async context => {
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/panel-productos.html");
});

app.Run();