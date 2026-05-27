namespace Hydron.Application.Common.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (this.validators.Any())
        {
            var validationResults = await Task.WhenAll(this.validators.Select(validator => validator.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken)));

            var failures = validationResults
                .Where(result => result.Errors.Any())
                .SelectMany(result => result.Errors)
                .ToList();

            if (failures.Count is not 0)
            {
                return (TResponse)(object)Result.Fail(failures.Select(failure => failure.ErrorMessage));
            }
        }

        return await next();
    }
}
