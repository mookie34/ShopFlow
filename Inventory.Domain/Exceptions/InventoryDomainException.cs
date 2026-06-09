namespace Inventory.Domain.Exceptions
{
    using System;

    public class InventoryDomainException : Exception
    {
        public InventoryDomainException(string message) : base(message) { }
    }
}
