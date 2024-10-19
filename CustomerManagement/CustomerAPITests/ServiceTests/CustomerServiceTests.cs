using AutoFixture;
using AutoMapper;
using CustomerAPI.DTOs;
using CustomerAPI.Entities;
using CustomerAPI.Exceptions;
using CustomerAPI.Repositories.Interfaces;
using CustomerAPI.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace CustomerAPITests.ServiceTests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CustomerService>> _loggerMock;
        private readonly CustomerService _customerService;
        private readonly Fixture _fixture;
        private readonly IMemoryCache _cache;

        public CustomerServiceTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CustomerService>>();
            _cache = new MemoryCache(new MemoryCacheOptions());
            _customerService = new CustomerService(_customerRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object, _cache);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfCustomersFromDb_WhenCaheMiss()
        {
            // Arrange
            var filterOptions = _fixture.Build<GetRequestFilterOptions>()
                                        .With(o => o.Take, 10)
                                        .With(o => o.Skip, 0)
                                        .Create();

            var customers = new List<CustomerEnt>
            {
                new CustomerEnt { Id = Guid.NewGuid(), FirstName = "Test User1" },
                new CustomerEnt { Id = Guid.NewGuid(), FirstName = "Test User2" }
            };
            var cacheKey = "AllCustomers";


            _customerRepositoryMock.Setup(repo => repo.GetAllAsync(filterOptions))
                .ReturnsAsync(customers);
            _mapperMock.Setup(m => m.Map<IEnumerable<CustomerResponseDto>>(It.IsAny<IEnumerable<CustomerEnt>>()))
                .Returns(new List<CustomerResponseDto> { new CustomerResponseDto(), new CustomerResponseDto() });

            // Act
            var result = await _customerService.GetAllAsync(filterOptions);

            // Assert
            var cachedAllCustomers = _cache.Get(cacheKey) as IEnumerable<CustomerEnt>;
            Assert.NotNull(cachedAllCustomers);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfCustomersFromCache_WhenCaheHit()
        {
            // Arrange
            var filterOptions = _fixture.Build<GetRequestFilterOptions>()
                                        .With(o => o.Take, 10)
                                        .With(o => o.Skip, 0)
                                        .Create();
            var cacheKey = "AllCustomers";

            var customers = new List<CustomerEnt>
            {
                new CustomerEnt { Id = Guid.NewGuid(), FirstName = "Test User1" },
                new CustomerEnt { Id = Guid.NewGuid(), FirstName = "Test User2" }
            };


            _customerRepositoryMock.Setup(repo => repo.GetAllAsync(filterOptions))
                .ReturnsAsync(customers);
            _mapperMock.Setup(m => m.Map<IEnumerable<CustomerResponseDto>>(It.IsAny<IEnumerable<CustomerEnt>>()))
                .Returns(new List<CustomerResponseDto> { new CustomerResponseDto(), new CustomerResponseDto() });
            _cache.Set(cacheKey, customers, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

            // Act
            var result = await _customerService.GetAllAsync(filterOptions);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddAsync_ShouldCallRepositoryAddMethod_WhenCacheMiss()
        {
            // Arrange
            var customerDto = new CustomerRequestDto { FirstName = "Test User1", Address = "Test Address" };
            var customerEntity = new CustomerEnt { Id = Guid.NewGuid(), FirstName = "Test User1", Address = "Test Address" };
            var cacheKey = "AllCustomers";

            _mapperMock.Setup(m => m.Map<CustomerEnt>(customerDto))
                .Returns(customerEntity);

            // Act
            await _customerService.AddAsync(customerDto);

            // Assert
            var cachedAllCustomers = _cache.Get(cacheKey) as IEnumerable<CustomerEnt>;
            Assert.Null(cachedAllCustomers);
            _customerRepositoryMock.Verify(repo => repo.AddAsync(customerEntity), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCustomerFromCache_WhenCacheHit()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customerEntity = new CustomerEnt { Id = customerId, FirstName = "Test User1" };
            var cacheKey = $"Customer_{customerId}";

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync(customerEntity);
            _mapperMock.Setup(m => m.Map<CustomerResponseDto>(customerEntity))
                .Returns(new CustomerResponseDto { Id = customerId, FirstName = "Test User1" });
            _cache.Set(cacheKey, customerEntity, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

            // Act
            var result = await _customerService.GetByIdAsync(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerEntity.Id, result.Id);
            Assert.Equal(customerEntity.FirstName, result.FirstName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCustomerFromDb_WhenCacheMiss()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customerEntity = new CustomerEnt { Id = customerId, FirstName = "Test User1" };
            var cacheKey = $"Customer_{customerId}";

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync(customerEntity);
            _mapperMock.Setup(m => m.Map<CustomerResponseDto>(customerEntity))
                .Returns(new CustomerResponseDto());

            // Act
            var result = await _customerService.GetByIdAsync(customerId);

            // Assert
            var cachedCustomer = _cache.Get(cacheKey) as CustomerEnt;
            Assert.NotNull(cachedCustomer);
            _customerRepositoryMock.Verify(repo => repo.GetByIdAsync(customerId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowCustomerNotFoundException_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync((CustomerEnt?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomerNotFoundException>(() => 
                                            _customerService.GetByIdAsync(customerId));
            Assert.Equal($"GetByIdAsync failed, Customer with ID {customerId} not found.", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallRepositoryUpdateMethod()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customerDto = new CustomerRequestDto { FirstName = "Test User1", Address = "Test Address" };
            var customerEntity = new CustomerEnt { Id = customerId, FirstName = "Test User1", Address = "Test Address" };
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync(customerEntity);
            _mapperMock.Setup(m => m.Map<CustomerEnt>(customerDto))
                .Returns(new CustomerEnt { Id = customerId, FirstName = "Test User1", Address = "Test Address" });

            // Act
            await _customerService.UpdateAsync(customerId, customerDto);

            // Assert
            _customerRepositoryMock.Verify(repo => repo.UpdateAsync(customerId, It.IsAny<CustomerEnt>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowCustomerNotFoundException_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customerDto = new CustomerRequestDto { FirstName = "Test User1", Address = "Test Address" };
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync((CustomerEnt?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomerNotFoundException>(() => 
                                            _customerService.UpdateAsync(customerId, customerDto));
            Assert.Equal($"UpdateAsync failed, Customer with ID {customerId} not found.", exception.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallRepositoryDeleteMethod()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customerEntity = new CustomerEnt { Id = customerId, FirstName = "Test User1" };
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync(customerEntity);

            // Act
            await _customerService.DeleteAsync(customerId);

            // Assert
            _customerRepositoryMock.Verify(repo => repo.DeleteAsync(customerId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowCustomerNotFoundException_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync((CustomerEnt?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomerNotFoundException>(() => _customerService.DeleteAsync(customerId));
            Assert.Equal($"DeleteAsync failed, Customer with ID {customerId} not found.", exception.Message);
        }
    }
}
