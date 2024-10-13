using CustomerUI.Configurations;
using CustomerUI.Services.Interfaces;
using CustomerUI.Services;
using Microsoft.Extensions.Options;

namespace CustomerUI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.Configure<ApiSettings>(services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection("ApiSettings"));

            services.AddHttpClient<ICustomerApiService, CustomerApiService>((serviceProvider, client) =>
            {
                var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;

                client.BaseAddress = new Uri(apiSettings.BaseUrl);
            });

            return services;
        }
    }
}
