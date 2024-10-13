using CustomerUI.Models;
using CustomerUI.Services.Interfaces;
using System.Net.Http;
using System.Threading;

namespace CustomerUI.Services
{
    public class CustomerApiService : ICustomerApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CustomerApiService> _logger;

        public CustomerApiService(HttpClient httpClient, ILogger<CustomerApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Customer");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<CustomerViewModel>>()
                       ?? new List<CustomerViewModel>();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
