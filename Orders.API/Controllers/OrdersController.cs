using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Commands.CreateOrder;
using Orders.Application.Queries.GetOrderById;
using Polly.CircuitBreaker;

namespace Orders.API.Controllers;

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
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(CreateOrder), new { id = response.OrderId }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, new { error = "Inventory service is currently unavailable. Please try again later." });
        }
        catch (BrokenCircuitException)
        {
            return StatusCode(503, new { error = "Inventory service is currently unavailable due to high failure rate. Please try again later." });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}