using Inventory.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _repository;
        public InventoryController(IInventoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{productId}/check-stock")]
        public async Task<IActionResult> CheckStock(Guid productId, [FromQuery] int quantity, CancellationToken cancellationToken)
        {
            var item = await _repository.GetByProductIdAsync(productId, cancellationToken);
            if (item == null)
                return NotFound($"Product with ID {productId} not found.");

            var isAvaible = item.Stock >= quantity;
            return Ok(new
            {
                ProductId = productId,
                RequestedQuantity = quantity,
                AvailableStock = item.Stock,
                IsAvailable = isAvaible
            });
        }
    }
}
