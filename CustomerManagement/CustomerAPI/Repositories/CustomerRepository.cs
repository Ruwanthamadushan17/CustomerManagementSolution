using CustomerAPI.Entities;
using CustomerAPI.Repositories.Interfaces;

namespace CustomerAPI.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private static readonly List<CustomerEnt> _customers = new();
        private static bool _isSeeded = false;

        public CustomerRepository()
        {
            SeedData();
        }

        public async Task<IEnumerable<CustomerEnt>> GetAllAsync()
        {
            await SimulateIODelay();
            return _customers.Where(c => !c.IsDeleted);
        }

        public async Task AddAsync(CustomerEnt customer)
        {
            await SimulateIODelay();
            customer.Id = Guid.NewGuid();
            customer.CreatedAt = DateTime.UtcNow;
            _customers.Add(customer);
        }

        private void SeedData()
        {
            // Seed initial data
            if (!_isSeeded)
            {
                _customers.Add(new CustomerEnt { Id = Guid.NewGuid(), Name = "User One", Email = "user1@example.com", Address = "1 street1 city1", IsDeleted = false, CreatedAt = DateTime.UtcNow });
                _customers.Add(new CustomerEnt { Id = Guid.NewGuid(), Name = "User Two", Email = "user2@example.com", Address = "2 street2 city2", IsDeleted = false, CreatedAt = DateTime.UtcNow });

                _isSeeded = true;
            }
        }

        private async Task SimulateIODelay()
        {
            await Task.Delay(500);
        }
    }
}
