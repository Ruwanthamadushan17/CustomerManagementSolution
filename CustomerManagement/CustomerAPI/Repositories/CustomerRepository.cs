using CustomerAPI.Data;
using CustomerAPI.Entities;
using CustomerAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerAPI.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _context;

        public CustomerRepository(CustomerDbContext context)
        {
            _context = context;
            SeedData();
        }

        public async Task<IEnumerable<CustomerEnt>> GetAllAsync()
        {
            return await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();
        }

        public async Task AddAsync(CustomerEnt customer)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    customer.Id = Guid.NewGuid();
                    customer.CreatedAt = DateTime.UtcNow;
                    await _context.Customers.AddAsync(customer);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<CustomerEnt?> GetByIdAsync(Guid id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task UpdateAsync(Guid id, CustomerEnt customer)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existing = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id && !c.IsDeleted);

                    if (customer != null)
                    {
                        existing.Name = customer.Name;
                        existing.Email = customer.Email;
                        existing.Address = customer.Address;
                        existing.updatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

                    if (customer != null)
                    {
                        customer.IsDeleted = true;
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        private void SeedData()
        {
            // Seed initial data
            if (!_context.Customers.Any())
            {
                _context.Customers.AddRange(new[] {
                    new CustomerEnt { Id = Guid.NewGuid(), Name = "User One", Email = "user1@example.com", Address = "1 street1 city1", IsDeleted = false, CreatedAt = DateTime.UtcNow },
                    new CustomerEnt { Id = Guid.NewGuid(), Name = "User Two", Email = "user2@example.com", Address = "2 street2 city2", IsDeleted = false, CreatedAt = DateTime.UtcNow }
                });
                _context.SaveChanges();
            }
        }
    }
}
