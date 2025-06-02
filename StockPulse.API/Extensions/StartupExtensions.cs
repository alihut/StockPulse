using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StockPulse.API.Helpers;
using StockPulse.API.Services;
using StockPulse.Application.Interfaces;
using StockPulse.Application.Services;
using StockPulse.Application.Settings;
using StockPulse.Infrastructure.Repositories;
using StockPulse.Infrastructure.Services;

namespace StockPulse.API.Extensions
{
    public  static class StartupExtensions
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


        //public static void AddJwtAuthentication(this WebApplicationBuilder builder)
        //{
        //    var jwtSettings = builder.Configuration.GetSettings<JwtSettings>();
        //    var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

        //    builder.Services.AddAuthentication(options =>
        //        {
        //            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //        })
        //        .AddJwtBearer(options =>
        //        {
        //            options.RequireHttpsMetadata = false;
        //            options.SaveToken = true;
        //            options.TokenValidationParameters = new TokenValidationParameters
        //            {
        //                ValidateIssuer = true,
        //                ValidIssuer = jwtSettings.Issuer,
        //                ValidateAudience = true,
        //                ValidAudience = jwtSettings.Audience,
        //                ValidateIssuerSigningKey = true,
        //                IssuerSigningKey = new SymmetricSecurityKey(key),
        //                ValidateLifetime = true
        //            };

        //        });

        //    builder.Services.AddAuthorization();
        //}

        public static void AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserContext, UserContext>();

            if (!string.Equals(Environment.GetEnvironmentVariable("DISABLE_SIMULATOR"), "true", StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.AddHostedService<StockPriceSimulator>();
            }

            builder.Services.AddSingleton<ISymbolValidator, SymbolValidator>();
            builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAlertService, AlertService>();
            builder.Services.AddScoped<IStockPriceService, StockPriceService>();
            builder.Services.AddScoped<IStockPricePublisherService, StockPricePublisherService>();
            builder.Services.AddScoped<INotificationService, SignalRNotificationService>();
            builder.Services.AddScoped<IAlertEvaluationService, AlertEvaluationService>();
        }

        public static void AddRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAlertRepository, AlertRepository>();
            builder.Services.AddScoped<IStockPriceRepository, StockPriceRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
