using MediatR;
using Orders.Application.Interfaces;
using Orders.Domain.Entities;

namespace Orders.Application.Queries.GetOrderById
{

    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, GetOrderByIdResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<GetOrderByIdResponse> Handle(
            GetOrderByIdQuery request,
            CancellationToken cancellationToken)
        {
            Order order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");

            return new GetOrderByIdResponse(
                order.Id,
                order.Status.ToString(),
                order.CreatedAt
            );
        }
    }
}
