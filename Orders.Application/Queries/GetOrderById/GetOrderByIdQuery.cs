namespace Orders.Application.Queries.GetOrderById
{
    using MediatR;

    public record GetOrderByIdQuery(
        Guid OrderId
        ) : IRequest<GetOrderByIdResponse>;
}
