namespace Orders.Domain.Exceptions
{
    using System;

    public class OrderDomainException : Exception
    {
        public OrderDomainException(string message) : base(message) { }
    }
}
