
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AiWorkspace.Application.Interfaces;
using AiWorkspace.Domain.Entities;
using AiWorkspace.Infrastructure.AI;
using AiWorkspace.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AiWorkspace.Api.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDevelopment", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IOnboardingAgent, OnboardingAgent>();

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string 'DefaultConnection' is missing.");
}

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<AuthDbContext>();

var jwtKey = builder.Configuration["Jwt:SigningKey"];

if (string.IsNullOrWhiteSpace(jwtKey) ||
    Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "JWT signing key must contain at least 32 bytes.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? "AiWorkspace.Api";

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? "AiWorkspace.Client";

var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtKey));

var authentication = builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "ApplicationAuth";
    options.DefaultChallengeScheme = "ApplicationAuth";
});

authentication.AddPolicyScheme(
    "ApplicationAuth",
    "JWT or Development Static Token",
    options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            var authorization = context.Request.Headers.Authorization
                .ToString();

            if (context.RequestServices
                    .GetRequiredService<IHostEnvironment>()
                    .IsDevelopment())
            {
                var staticToken = context.RequestServices
                    .GetRequiredService<IConfiguration>()
                    ["Authentication:DevelopmentStaticToken"];

                if (!string.IsNullOrWhiteSpace(staticToken) &&
                    authorization.StartsWith(
                        "Bearer ",
                        StringComparison.OrdinalIgnoreCase))
                {
                    var providedToken = authorization[7..].Trim();

                    var providedBytes = Encoding.UTF8.GetBytes(
                        providedToken);

                    var expectedBytes = Encoding.UTF8.GetBytes(
                        staticToken);

                    if (providedBytes.Length == expectedBytes.Length &&
                        CryptographicOperations.FixedTimeEquals(
                            providedBytes,
                            expectedBytes))
                    {
                        return "DevelopmentStatic";
                    }
                }
            }

            return JwtBearerDefaults.AuthenticationScheme;
        };
    });

authentication.AddJwtBearer(
    JwtBearerDefaults.AuthenticationScheme,
    options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,

            ClockSkew = TimeSpan.FromSeconds(30),

            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

if (builder.Environment.IsDevelopment())
{
    authentication.AddScheme<
        AuthenticationSchemeOptions,
        DevelopmentStaticTokenHandler>(
        "DevelopmentStatic",
        _ => { });
}

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter a JWT or the Development static token."
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AngularDevelopment");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
