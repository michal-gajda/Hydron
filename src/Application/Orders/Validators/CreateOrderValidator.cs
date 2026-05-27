namespace Hydron.Application.Orders.Validators;

using Hydron.Application.Customers.Interfaces;
using Hydron.Application.Orders.Commands;

internal sealed class CreateOrderValidator : AbstractValidator<CreateOrder>
{
    private readonly ICustomerProvider customerProvider;

    public CreateOrderValidator(ICustomerProvider customerProvider)
    {
        this.customerProvider = customerProvider;

        base.RuleFor(command => command.Id.Value).NotEmpty();
        base.RuleFor(command => command.CustomerId.Value).NotEmpty();
        base.RuleFor(command => command.CustomerId)
            .MustAsync(this.IsExist)
            .WithMessage("Customer with the given ID does not exist.");
    }

    private async Task<bool> IsExist(CustomerId id, CancellationToken cancellationToken = default)
    {
        var exists = await customerProvider.ExistsAsync(id, cancellationToken);

        return exists;
    }
}
