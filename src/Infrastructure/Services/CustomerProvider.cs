namespace Hydron.Infrastructure.Services;

using Hydron.Application.Customers.Interfaces;
using Hydron.Domain.ValueObjects;

internal sealed class CustomerProvider : ICustomerProvider
{
    public Task<bool> ExistsAsync(CustomerId id, CancellationToken cancellationToken) => Task.FromResult(true);
}
