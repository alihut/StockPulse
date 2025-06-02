using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using StockPulse.Application.Interfaces;
using StockPulse.Infrastructure.Services;

namespace StockPulse.Infrastructure.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();

                cfg.AddConsumers(typeof(MassTransitExtensions).Assembly);

                cfg.UsingRabbitMq((context, busCfg) =>
                {
                    busCfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    busCfg.ConfigureEndpoints(context);
                });
            });

            services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

            return services;
        }
    }
}
