using Microsoft.Extensions.DependencyInjection;
using Restaurant.BLL.Services.Authentication;

namespace Restaurant.BLL
{
    public static class DependecyInjuction
    {
        public static IServiceCollection AddBLLServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthServices, AuthServices>();

            return services;
        }
    }
}
