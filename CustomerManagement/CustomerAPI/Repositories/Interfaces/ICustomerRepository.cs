using CustomerAPI.Entities;

namespace CustomerAPI.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<CustomerEnt>> GetAllAsync();
    }
}
