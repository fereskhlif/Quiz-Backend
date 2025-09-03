using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quizzz;
using Quizzz.Configurations;
using Quizzz.Models;
using Quizzz.Repository;
using Quizzz.UtilityService;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 📦 Configuration EF Core avec SQL Server
builder.Services.AddDbContext<QuizzContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).LogTo(Console.WriteLine, LogLevel.Information)
       .EnableSensitiveDataLogging());
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });


// 🔁 AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));

// ✅ Injection du Repository générique
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// ✅ Configuration de MailKit pour l’envoi d’e-mails
var mailKitOptions = new MailKitOptions();
builder.Configuration.GetSection("EmailSettings").Bind(mailKitOptions);

// Validation simple de la config MailKit
if (string.IsNullOrWhiteSpace(mailKitOptions.Server) ||
    mailKitOptions.Port == 0 ||
    string.IsNullOrWhiteSpace(mailKitOptions.SenderEmail) ||
    string.IsNullOrWhiteSpace(mailKitOptions.Account) ||
    string.IsNullOrWhiteSpace(mailKitOptions.Password))
{
    throw new Exception("La configuration EmailSettings est manquante ou incorrecte.");
}

builder.Services.AddMailKit(options =>
{
    options.UseMailKit(mailKitOptions);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var jwtSecret = builder.Configuration["JwtSecretForLocal"];
var localIssuer = builder.Configuration["LocalIssuer"];
var localAudience = builder.Configuration["LocalAudience"];

if (string.IsNullOrEmpty(jwtSecret) ||
    string.IsNullOrEmpty(localIssuer) ||
    string.IsNullOrEmpty(localAudience))
{
    throw new Exception("La configuration JWT est manquante ou incorrecte.");
}

var keyLocal = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Local", options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = keyLocal,
        ValidateIssuer = true,
        ValidIssuer = localIssuer,
        ValidateAudience = true,
        ValidAudience = localAudience,
        ValidateLifetime = true
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
