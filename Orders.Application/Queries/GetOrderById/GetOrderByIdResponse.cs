namespace Orders.Application.Queries.GetOrderById
{
    public record GetOrderByIdResponse
    (
         Guid OrderId,
        string Status,
        DateTime CreatedAt
        );
}
