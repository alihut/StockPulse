using MassTransit;
using StockPulse.Application.Interfaces;

namespace StockPulse.Infrastructure.Services
{
    public class MassTransitEventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
        {
            return _publishEndpoint.Publish(@event, cancellationToken);
        }
    }

}
