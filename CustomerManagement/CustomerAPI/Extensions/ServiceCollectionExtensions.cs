using CustomerAPI.Mappings;
using CustomerAPI.Repositories.Interfaces;
using CustomerAPI.Repositories;
using CustomerAPI.Services.Interfaces;
using CustomerAPI.Services;

namespace CustomerAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            return services;
        }

        private static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            return services;
        }

        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddServices();
            services.AddRepositories();
            services.AddAutoMapperProfiles();

            return services;
        }
    }
}
