using AutoMapper;
using CustomerAPI.DTOs;
using CustomerAPI.Entities;
using CustomerAPI.Exceptions;
using CustomerAPI.Repositories.Interfaces;
using CustomerAPI.Services;
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

        public CustomerServiceTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CustomerService>>();
            _customerService = new CustomerService(_customerRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfCustomers()
        {
            // Arrange
            var customers = new List<CustomerEnt>
            {
                new CustomerEnt { Id = Guid.NewGuid(), Name = "Test User1" },
                new CustomerEnt { Id = Guid.NewGuid(), Name = "Test User2" }
            };
            _customerRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(customers);
            _mapperMock.Setup(m => m.Map<IEnumerable<CustomerResponseDto>>(It.IsAny<IEnumerable<CustomerEnt>>()))
                .Returns(new List<CustomerResponseDto> { new CustomerResponseDto(), new CustomerResponseDto() });

            // Act
            var result = await _customerService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddAsync_ShouldCallRepositoryAddMethod()
        {
            // Arrange
            var customerDto = new CustomerRequestDto { Name = "Test User1", Address = "Test Address" };
            var customerEntity = new CustomerEnt { Id = Guid.NewGuid(), Name = "Test User1", Address = "Test Address" };

            _mapperMock.Setup(m => m.Map<CustomerEnt>(customerDto))
                .Returns(customerEntity);

            // Act
            await _customerService.AddAsync(customerDto);

            // Assert
            _customerRepositoryMock.Verify(repo => repo.AddAsync(customerEntity), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customerEntity = new CustomerEnt { Id = customerId, Name = "Test User1" };
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync(customerEntity);
            _mapperMock.Setup(m => m.Map<CustomerResponseDto>(customerEntity))
                .Returns(new CustomerResponseDto());

            // Act
            var result = await _customerService.GetByIdAsync(customerId);

            // Assert
            Assert.NotNull(result);
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
            var customerDto = new CustomerRequestDto { Name = "Test User1", Address = "Test Address" };
            var customerEntity = new CustomerEnt { Id = customerId, Name = "Test User1", Address = "Test Address" };
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync(customerEntity);
            _mapperMock.Setup(m => m.Map<CustomerEnt>(customerDto))
                .Returns(new CustomerEnt { Id = customerId, Name = "Test User1", Address = "Test Address" });

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
            var customerDto = new CustomerRequestDto { Name = "Test User1", Address = "Test Address" };
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
            var customerEntity = new CustomerEnt { Id = customerId, Name = "Test User1" };
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
