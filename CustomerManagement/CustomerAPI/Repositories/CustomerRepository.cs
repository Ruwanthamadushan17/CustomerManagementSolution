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
            await Task.Delay(500);
            return _customers.Where(c => !c.IsDeleted);
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
    }
}
