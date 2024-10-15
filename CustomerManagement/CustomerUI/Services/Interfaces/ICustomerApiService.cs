using CustomerUI.Models;

namespace CustomerUI.Services.Interfaces
{
    public interface ICustomerApiService
    {
        Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync(int? skip, int? take);
        Task CreateAsync(CustomerViewModel customer);
        Task<CustomerViewModel> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, CustomerViewModel customer);
        Task DeleteAsync(Guid id);
    }
}
