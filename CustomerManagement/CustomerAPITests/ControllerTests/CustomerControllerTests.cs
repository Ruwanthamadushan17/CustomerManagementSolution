using CustomerAPI.Controllers;
using CustomerAPI.DTOs;
using CustomerAPI.Exceptions;
using CustomerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using AutoFixture;

namespace CustomerAPITests.ControllerTests
{
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly CustomerController _customerController;
        private readonly Fixture _fixture;

        public CustomerControllerTests()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _customerController = new CustomerController(_customerServiceMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetCustomers_ShouldReturnOk_WhenCustomersExist()
        {
            // Arrange
            var filterOptions = _fixture.Build<GetRequestFilterOptions>()
                                        .With(o => o.Take, 10)
                                        .With(o => o.Skip, 0)
                                        .Create();

            var customers = new List<CustomerResponseDto>
            {
                GetCustomerResponseDto()
            };

            _customerServiceMock.Setup(service => service.GetAllAsync(filterOptions))
                .ReturnsAsync(customers);

            // Act
            var result = await _customerController.GetCustomers(filterOptions);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCustomers = Assert.IsType<List<CustomerResponseDto>>(okResult.Value);
            Assert.Single(returnedCustomers);
            _customerServiceMock.Verify(x => x.GetAllAsync(filterOptions), Times.Once);
        }

        [Fact]
        public async Task AddCustomer_ShouldBeSuccess()
        {
            // Arrange
            var newCustomer = GetCustomerResquetDto();

            _customerServiceMock.Setup(service => service.AddAsync(newCustomer))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _customerController.AddCustomer(newCustomer);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _customerServiceMock.Verify(x => x.AddAsync(newCustomer), Times.Once);
        }

        [Fact]
        public async Task GetCustomerById_ShouldReturnOk_WhenCustomerExists()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = GetCustomerResponseDto();
            customer.Id = customerId;   

            _customerServiceMock.Setup(service => service.GetByIdAsync(customerId))
                .ReturnsAsync(customer);

            // Act
            var result = await _customerController.GetCustomer(customerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCustomer = Assert.IsType<CustomerResponseDto>(okResult.Value);
            Assert.Equal(customerId, returnedCustomer.Id);
            _customerServiceMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        }

        [Fact]
        public async Task GetCustomer_ShouldThrowCustomerNotFoundException_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            _customerServiceMock.Setup(service => service.GetByIdAsync(customerId))
                .ThrowsAsync(new CustomerNotFoundException());

            // Act
            await Assert.ThrowsAsync<CustomerNotFoundException>(() => _customerController.GetCustomer(customerId));            
        }

        [Fact]
        public async Task DeleteCustomer_ShouldReturnNoContent_WhenCustomerIsDeleted()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            _customerServiceMock.Setup(service => service.DeleteAsync(customerId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _customerController.DeleteCustomer(customerId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _customerServiceMock.Verify(x => x.DeleteAsync(customerId), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomer_ShouldReturnOk()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var updatingCustomer = GetCustomerResquetDto();

            _customerServiceMock.Setup(service => service.UpdateAsync(customerId, updatingCustomer))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _customerController.UpdateCustomer(customerId, updatingCustomer);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _customerServiceMock.Verify(x => x.UpdateAsync(customerId, updatingCustomer), Times.Once);
        }

        private CustomerRequestDto GetCustomerResquetDto()
        {
            return _fixture.Build<CustomerRequestDto>()
                                        .With(o => o.FirstName, "Test User")
                                        .With(o => o.Address, "Test Address")
                                        .With(o => o.Email, "test@test.com")
                                        .Create();
        }

        private CustomerResponseDto GetCustomerResponseDto()
        {
            return _fixture.Build<CustomerResponseDto>()
                                        .With(o => o.FirstName, "Test User")
                                        .With(o => o.Address, "Test Address")
                                        .With(o => o.Email, "test@test.com")
                                        .Create();
        }
    }
}
