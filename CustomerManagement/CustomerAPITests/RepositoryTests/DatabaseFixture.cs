using CustomerAPI.Data;
using CustomerAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerAPITests.RepositoryTests
{
    public class DatabaseFixture : IDisposable
    {
        private static DbContextOptions<CustomerDbContext> _dbContextOptions;
        public readonly CustomerDbContext _dbContext;
        public DatabaseFixture()
        {
            _dbContextOptions = new DbContextOptionsBuilder<CustomerDbContext>().UseInMemoryDatabase(databaseName: "CustomerDb").Options;
            _dbContext = new CustomerDbContext(_dbContextOptions);
            SetupTheDatabase();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        private void SetupTheDatabase()
        {
            _dbContext.Customers.AddRange(GetFakeData().AsQueryable());
            _dbContext.SaveChanges();
        }

        private List<CustomerEnt> GetFakeData()
        {

            var customers = new List<CustomerEnt> {
                new CustomerEnt { Id = Guid.Parse("77b1e4d8-a2a3-44b2-a54b-ecf41304d0ff"), FirstName = "Test User1", LastName = "Test User1", Email = "test@test1.com", Address = "1 Test Address", IsDeleted = false },
                new CustomerEnt { Id = Guid.Parse("88b1e4d8-a2a3-44b2-a54b-ecf41304d0ff"), FirstName = "Test User2", LastName = "Test User1", Email = "test@test2.com", Address = "2 Test Address", IsDeleted = false },
                new CustomerEnt { Id = Guid.Parse("99b1e4d8-a2a3-44b2-a54b-ecf41304d0ff"), FirstName = "Test User3", LastName = "Test User1", Email = "test@test3.com", Address = "3 Test Address", IsDeleted = false }
            };
            return customers;
        }
    }
}
