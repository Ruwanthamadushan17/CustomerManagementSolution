using CustomerUI.Configurations;
using CustomerUI.Models;
using CustomerUI.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading;

namespace CustomerUI.Services
{
    public class CustomerApiService : ICustomerApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CustomerApiService> _logger;
        private readonly ApiSettings _apiSettings;

        public CustomerApiService(HttpClient httpClient, ILogger<CustomerApiService> logger, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiSettings = apiSettings.Value;
        }

        public async Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiSettings.CustomerEndpoint);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<CustomerViewModel>>()
                       ?? new List<CustomerViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customers.");
                throw;
            }
        }

        public async Task CreateAsync(CustomerViewModel customer)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_apiSettings.CustomerEndpoint, customer);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new customer.");
                throw;
            }
        }

        public async Task<CustomerViewModel> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiSettings.CustomerEndpoint}/{id}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CustomerViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching customer with id {id}.");
                throw;
            }
        }

        public async Task UpdateAsync(Guid id, CustomerViewModel customer)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiSettings.CustomerEndpoint}/{id}", customer);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating customer with id {id}.");
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiSettings.CustomerEndpoint}/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting customer with id {id}.");
                throw;
            }
        }
    }
}
