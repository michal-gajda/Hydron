namespace Hydron.Application.Customers.Interfaces;

public interface ICustomerProvider
{
    Task<bool> ExistsAsync(CustomerId id, CancellationToken cancellationToken);
}
