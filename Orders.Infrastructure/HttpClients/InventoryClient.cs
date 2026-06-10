using Orders.Application.Interfaces;
using System.Text.Json;

namespace Orders.Infrastructure.HttpClients
{

    public class InventoryClient : IInventoryClient
    {
        private readonly HttpClient _httpClient;
        public InventoryClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CheckStockAsync(
            Guid productId,
            int quantity,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(
                $"/api/inventory/{productId}/check-stock?quantity={quantity}", cancellationToken);

            if (!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<StockCheckResult>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.IsAvailable ?? false;
        }

        public record StockCheckResult(bool IsAvailable);
    }
}
