using CustomerAPI.Entities;

namespace CustomerAPI.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<CustomerEnt>> GetAllAsync();
        Task AddAsync(CustomerEnt customer);
        Task<CustomerEnt> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, CustomerEnt customer);
        Task DeleteAsync(Guid id);
    }
}
