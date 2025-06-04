using Moq;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using StockPulse.Domain.Entities;
using StockPulse.Domain.Enums;
using StockPulse.IntegrationTests.Stubs;

namespace StockPulse.IntegrationTests.Tests
{
    public class PriceBatchAlertEvaluatorTests
    {
        [Fact]
        public async Task EvaluateAsync_WithoutLock_AllowsRaceCondition()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var alertId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var symbol = "AAPL";

            var alert = new Alert
            {
                Id = alertId,
                UserId = userId,
                Symbol = symbol,
                PriceThreshold = 100,
                Type = AlertType.Above,
                IsActive = true
            };

            var priceList = new List<RecordPriceRequestDto>
        {
            new() { Symbol = symbol, Price = 150 }
        };

            var mockRepo = new Mock<IAlertRepository>();
            mockRepo.Setup(r => r.GetActiveAlertsBySymbolsAsync(It.IsAny<IEnumerable<string>>()))
                    .ReturnsAsync(new List<Alert> { alert });

            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Alert>()))
                    .Returns(Task.CompletedTask);

            var mockNotifier = new Mock<INotificationService>();
            mockNotifier.Setup(n => n.NotifyUserAsync(It.IsAny<Guid>(), It.IsAny<object>()))
                        .Returns(Task.CompletedTask);

            var mockCache = new Mock<ICacheService>();
            mockCache.Setup(c => c.Get<IEnumerable<RecordPriceRequestDto>>($"PriceBatch:{batchId}"))
                     .Returns(priceList);

            var evaluator = new LocklessPriceBatchAlertEvaluator(
                mockRepo.Object,
                mockNotifier.Object,
                mockCache.Object
            );


            // Act: simulate race condition
            var task1 = evaluator.EvaluateAsync(batchId);
            var task2 = evaluator.EvaluateAsync(batchId);

            await Task.WhenAll(task1, task2);

            // Assert: notification was sent twice
            mockNotifier.Verify(n =>
                n.NotifyUserAsync(userId, It.IsAny<object>()),
                Times.Exactly(2)); // 🔥 Race occurred: both triggered
        }
    }
}
