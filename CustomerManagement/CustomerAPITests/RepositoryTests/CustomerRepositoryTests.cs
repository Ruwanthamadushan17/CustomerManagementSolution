using CustomerAPI.Repositories;

namespace CustomerAPITests.RepositoryTests
{
    public class CustomerRepositoryTests : IClassFixture<DatabaseFixture>
    {
        DatabaseFixture _databaseFixture;
        private readonly CustomerRepository _customerRepository;

        public CustomerRepositoryTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _customerRepository = new CustomerRepository(_databaseFixture._dbContext);
        }

        [Fact]
        public void GetAllAsync_Returns_AllCustomers()
        {
            var response = _customerRepository.GetAllAsync();

            Assert.NotNull(response);
            Assert.True(response.Result.Count() > 0);
        }

        [Theory]
        [InlineData("77b1e4d8-a2a3-44b2-a54b-ecf41304d0ff")]
        public void GetByIdAsync_Returns_MatchingCustomer(string id)
        {
            var customerId = Guid.Parse(id);
            var response = _customerRepository.GetByIdAsync(customerId);

            Assert.NotNull(response);
            Assert.True(response.IsCompletedSuccessfully);
        }
    }
}
