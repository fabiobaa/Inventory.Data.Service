using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Inventory.Data.Service.Data;
using Inventory.Data.Service.DTOs;

namespace Inventory.Data.Service
{
    public class QueueProcessorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<QueueProcessorService> _logger;

        public QueueProcessorService(IServiceScopeFactory scopeFactory, ILogger<QueueProcessorService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queue Processor Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessQueueMessageAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred in the main processing loop.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            _logger.LogInformation("Queue Processor Service is stopping.");
        }

        private async Task ProcessQueueMessageAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

            var message = await dbContext.QueuedMessages
                .OrderBy(m => m.CreatedAt)
                .FirstOrDefaultAsync(m => m.Status == "Pendiente", stoppingToken);

            if (message == null)
            {
                return;
            }

            try
            {
                _logger.LogInformation($"Processing message ID: {message.Id}");
                var saleEvent = JsonSerializer.Deserialize<SaleOccurredEvent>(message.Payload);

                if (saleEvent is not null)
                {

                    var inventoryItem = await dbContext.InventoryProducts
                        .FirstOrDefaultAsync(i => i.ProductId == saleEvent.ProductId && i.StoreId == saleEvent.StoreId, stoppingToken);

                    if (inventoryItem != null && inventoryItem.Quantity >= saleEvent.QuantitySold)
                    {
                        inventoryItem.Quantity -= saleEvent.QuantitySold;
                        message.Status = "Procesado";
                        _logger.LogInformation($"Message for transaction {saleEvent.TransactionId} processed successfully.");
                    }
                    else
                    {
                        message.Status = "Error";
                        _logger.LogWarning($"Failed to process transaction {saleEvent.TransactionId}: Insufficient stock or item not found.");
                        message.ErrorMessage = $"Failed to process transaction {saleEvent.TransactionId}: Insufficient stock or item not found.";
                    }

                    message.ProcessedAt = DateTime.UtcNow;

                    await dbContext.SaveChangesAsync(stoppingToken);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process message {message.Id}.");
                message.Status = "Error"; // Es mejor marcarlo como Error para no reintentar indefinidamente
                message.ErrorMessage = ex.Message; // Guardamos el mensaje de la excepción
                message.ProcessedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(stoppingToken);
            }
        }
    }
}