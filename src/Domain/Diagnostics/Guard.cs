namespace Hydron.Domain.Diagnostics;

using System.Runtime.CompilerServices;

public static class Guard
{
    public static void AgainstDefault<T>(T value, string? message = null, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : struct, IEquatable<T>
    {
        if (value.Equals(default))
        {
            throw new ArgumentException(message ?? $"Value cannot be the default value of {typeof(T).Name}.", paramName);
        }
    }
}
