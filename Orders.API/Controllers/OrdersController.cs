namespace Orders.API.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Orders.Application.Commands.CreateOrder;
    using Orders.Application.Contracts;

    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send<CreateOrderResponse>(command, cancellationToken);
            return CreatedAtAction(nameof(CreateOrder), new { id = response.OrderId }, response);
        }
    }
}
