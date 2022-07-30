using Orders.Api.Services;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
    }
}