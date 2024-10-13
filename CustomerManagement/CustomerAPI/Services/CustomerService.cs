using AutoMapper;
using CustomerAPI.DTOs;
using CustomerAPI.Entities;
using CustomerAPI.Repositories.Interfaces;
using CustomerAPI.Services.Interfaces;

namespace CustomerAPI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
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
    }
}
