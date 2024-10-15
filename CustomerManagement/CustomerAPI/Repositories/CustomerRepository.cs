using CustomerAPI.Data;
using CustomerAPI.DTOs;
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

        public async Task<IEnumerable<CustomerEnt>> GetAllAsync(GetRequestFilterOptions filterOptions)
        {
            var query = _context.Customers.Where(c => !c.IsDeleted)
                            .Select(c => new CustomerEnt
                            {
                                Id = c.Id,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                Email = c.Email,
                                Address = c.Address,
                                MobileNo = c.MobileNo,
                            })
                            .OrderBy(c => c.Id)
                            .AsNoTracking();

            if (filterOptions?.Skip.HasValue == true)
            {
                query = query.Skip(filterOptions.Skip.Value);
            }

            if (filterOptions?.Take.HasValue == true)
            {
                query = query.Take(filterOptions.Take.Value);
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(CustomerEnt customer)
        {
            customer.Id = Guid.NewGuid();
            customer.CreatedAt = DateTime.UtcNow;
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<CustomerEnt?> GetByIdAsync(Guid id)
        {
            return await _context.Customers.Where(c => c.Id == id && !c.IsDeleted)
                            .Select(c => new CustomerEnt
                            {
                                Id = c.Id,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                Email = c.Email,
                                Address = c.Address,
                                MobileNo = c.MobileNo
                            })
                            .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Guid id, CustomerEnt customer)
        {
            var existing = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id && !c.IsDeleted);

            if (customer != null)
            {
                existing.FirstName = customer.FirstName;
                existing.LastName = customer.LastName;
                existing.Email = customer.Email;
                existing.Address = customer.Address;
                existing.MobileNo = customer.MobileNo;
                existing.updatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (customer != null)
            {
                customer.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        private void SeedData()
        {
            // Seed initial data
            if (!_context.Customers.Any())
            {
                _context.Customers.AddRange(new[] {
                    new CustomerEnt { Id = Guid.NewGuid(), FirstName = "User1", LastName = "Last1", Email = "user1@example.com", Address = "1 street1 city1", MobileNo = "+440012345678", IsDeleted = false, CreatedAt = DateTime.UtcNow },
                    new CustomerEnt { Id = Guid.NewGuid(), FirstName = "User2", LastName = "Last2", Email = "user2@example.com", Address = "2 street2 city2", MobileNo = "+441112345678", IsDeleted = false, CreatedAt = DateTime.UtcNow }
                });
                _context.SaveChanges();
            }
        }
    }
}
