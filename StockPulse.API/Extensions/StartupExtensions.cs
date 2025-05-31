using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StockPulse.API.Helpers;
using StockPulse.API.Services;
using StockPulse.Application.Interfaces;
using StockPulse.Application.Services;
using StockPulse.Application.Settings;
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
                });

            builder.Services.AddAuthorization();
        }

        public static void AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<StockPriceSimulator>();

            builder.Services.AddScoped<ISymbolValidator, SymbolValidator>();

            builder.Services.AddScoped<IAlertService, AlertService>();
            builder.Services.AddScoped<IStockPriceService, StockPriceService>();
            builder.Services.AddScoped<INotificationService, SignalRNotificationService>();
            builder.Services.AddScoped<IAlertEvaluationService, AlertEvaluationService>();
        }
    }
}
