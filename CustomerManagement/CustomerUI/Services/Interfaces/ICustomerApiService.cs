using CustomerUI.Models;

namespace CustomerUI.Services.Interfaces
{
    public interface ICustomerApiService
    {
        Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync();
    }
}
