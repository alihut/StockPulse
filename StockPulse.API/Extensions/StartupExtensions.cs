using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using StockPulse.API.Helpers;
using StockPulse.API.Services;
using StockPulse.Application.Interfaces;
using StockPulse.Application.Services;
using StockPulse.Application.Settings;
using StockPulse.Infrastructure.Data;
using StockPulse.Infrastructure.Repositories;
using StockPulse.Infrastructure.Services;
using System.Net;
using System.Text;

namespace StockPulse.API.Extensions
{
    public static class StartupExtensions
    {
        public static void AddConfigurationSettings(this WebApplicationBuilder builder)
        {
            builder.Services.ConfigureSettings<JwtSettings>(builder.Configuration);
            builder.Services.ConfigureSettings<StockSettings>(builder.Configuration);
        }

        public static void AddJwtAuthentication(this WebApplicationBuilder builder)
        {
            var jwtSettings = builder.Configuration.GetSettings<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateLifetime = true
                    };

                    // ✅ Enable JWT token resolution from query string for SignalR
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            // Adjust "/alerts" if your hub route is different
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/alerts"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization();
        }

        public static void AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserContext, UserContext>();

            builder.AddStockPriceSimulator();

            builder.Services.AddSingleton<ISymbolValidator, SymbolValidator>();
            builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
            //builder.Services.AddSingleton<ICacheService, RedisCacheService>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAlertService, AlertService>();
            builder.Services.AddScoped<IStockPriceService, StockPriceService>();
            builder.Services.AddScoped<IStockPricePublisherService, StockPricePublisherService>();
            builder.Services.AddScoped<INotificationService, SignalRNotificationService>();
            builder.Services.AddScoped<IPriceBatchAlertEvaluator, PriceBatchAlertEvaluator>();
        }

        private static void AddStockPriceSimulator(this WebApplicationBuilder builder)
        {
            var disableViaEnv = string.Equals(
                Environment.GetEnvironmentVariable("DISABLE_SIMULATOR"),
                "true",
                StringComparison.OrdinalIgnoreCase
            );

            var simulatorEnabled = builder.Configuration.GetValue<bool>("EnableBackgroundSimulator");

            if (simulatorEnabled && !disableViaEnv)
            {
                builder.Services.AddHostedService<StockPriceSimulator>();
            }
        }

        public static void AddRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAlertRepository, AlertRepository>();
            builder.Services.AddScoped<IStockPriceRepository, StockPriceRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
        }

        public static void AddCustomCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173", "http://localhost:8080")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        public static void AddCustomSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "StockPulse API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token in the format: Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" // This MUST match the AddSecurityDefinition key
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        public static void AddRedis(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(configuration);
            });
        }

        public static void AddRedLockFactory(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IDistributedLockFactory>(_ =>
            {
                var redisEndpoints = new List<RedLockEndPoint>
                {
                    new DnsEndPoint("redis", 6379) // docker service name + port
                };

                return RedLockFactory.Create(redisEndpoints);
            });
        }

        public static async Task ApplyMigrationsAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StockPulseDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
