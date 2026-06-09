namespace Orders.Application.Commands.CreateOrder
{
    using FluentValidation;

    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId is required.");
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId is required.");
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("Total amount must be greater than zero.");
        }
    }
}
