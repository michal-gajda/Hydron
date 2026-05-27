namespace Hydron.Infrastructure;

using System.Reflection;
using Hydron.Application.Customers.Interfaces;
using Hydron.Domain.Interfaces;
using Hydron.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(cfg =>
        {
        }, assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        services.AddSingleton<ICustomerProvider, CustomerProvider>();
        services.AddSingleton<IOrderRepository, OrderRepository>();

        return services;
    }
}

