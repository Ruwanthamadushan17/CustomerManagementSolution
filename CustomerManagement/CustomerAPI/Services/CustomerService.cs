using AutoMapper;
using CustomerAPI.DTOs;
using CustomerAPI.Entities;
using CustomerAPI.Exceptions;
using CustomerAPI.Repositories.Interfaces;
using CustomerAPI.Services.Interfaces;

namespace CustomerAPI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetAllAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        }

        public async Task AddAsync(CustomerRequestDto customerDto)
        {
            var customer = _mapper.Map<CustomerEnt>(customerDto);
            await _customerRepository.AddAsync(customer);
        }

        public async Task<CustomerResponseDto> GetByIdAsync(Guid id)
        {
            var customer = await GetCustomerOrThrowAsync(id, nameof(GetByIdAsync));
            return _mapper.Map<CustomerResponseDto>(customer);
        }

        public async Task UpdateAsync(Guid id, CustomerRequestDto customerDto)
        {
            var customer = await GetCustomerOrThrowAsync(id, nameof(UpdateAsync));
            var updatedCustomer = _mapper.Map<CustomerEnt>(customerDto);
            await _customerRepository.UpdateAsync(id, updatedCustomer);
        }

        public async Task DeleteAsync(Guid id)
        {
            var customer = await GetCustomerOrThrowAsync(id, nameof(DeleteAsync));
            await _customerRepository.DeleteAsync(id);
        }

        private async Task<CustomerEnt> GetCustomerOrThrowAsync(Guid id, string method)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                _logger.LogError($"{method} failed, Customer with ID {id} not found.");
                throw new CustomerNotFoundException($"{method} failed, Customer with ID {id} not found.");
            }
            return customer;
        }
    }
}
