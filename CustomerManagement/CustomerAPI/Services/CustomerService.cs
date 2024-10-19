using AutoMapper;
using CustomerAPI.DTOs;
using CustomerAPI.Entities;
using CustomerAPI.Exceptions;
using CustomerAPI.Repositories.Interfaces;
using CustomerAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CustomerAPI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;
        private readonly IMemoryCache _cache;
        private const string AllCustomersCacheKey = "AllCustomers";
        private const string CustomerCacheKeyPrefix = "Customer_";

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper, ILogger<CustomerService> logger, IMemoryCache cache)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetAllAsync(GetRequestFilterOptions filterOptions)
        {
            if (!_cache.TryGetValue(AllCustomersCacheKey, out IEnumerable<CustomerEnt> customers))
            {
                customers = await _customerRepository.GetAllAsync(filterOptions);

                if (customers != null)
                {
                    SetCache(AllCustomersCacheKey, customers, TimeSpan.FromMinutes(10));
                }
            }
            return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        }

        public async Task AddAsync(CustomerRequestDto customerDto)
        {
            var customer = _mapper.Map<CustomerEnt>(customerDto);
            await _customerRepository.AddAsync(customer);

            ClearAllCustomersCache();
        }

        public async Task<CustomerResponseDto> GetByIdAsync(Guid id)
        {
            var cacheKey = GenerateCustomerCacheKey(id);

            if (!_cache.TryGetValue(cacheKey, out CustomerEnt customer))
            {
                customer = await GetCustomerOrThrowAsync(id, nameof(GetByIdAsync));

                if (customer != null)
                {
                    SetCache(cacheKey, customer, TimeSpan.FromMinutes(10));
                }
            }

            return _mapper.Map<CustomerResponseDto>(customer);
        }

        public async Task UpdateAsync(Guid id, CustomerRequestDto customerDto)
        {
            var customer = await GetCustomerOrThrowAsync(id, nameof(UpdateAsync));
            var updatedCustomer = _mapper.Map<CustomerEnt>(customerDto);
            await _customerRepository.UpdateAsync(id, updatedCustomer);

            ClearAllCustomersCache();
            ClearAllCustomersCache();
        }

        public async Task DeleteAsync(Guid id)
        {
            var customer = await GetCustomerOrThrowAsync(id, nameof(DeleteAsync));
            await _customerRepository.DeleteAsync(id);

            ClearAllCustomersCache();
            ClearAllCustomersCache();
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

        private string GenerateCustomerCacheKey(Guid id)
        {
            return $"{CustomerCacheKeyPrefix}{id}";
        }

        private void SetCache<T>(string cacheKey, T value, TimeSpan expirationTime)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime
            };
            _cache.Set(cacheKey, value, cacheEntryOptions);
        }

        private void ClearCustomerCache(Guid id)
        {
            _cache.Remove(GenerateCustomerCacheKey(id));
        }

        private void ClearAllCustomersCache()
        {
            _cache.Remove(AllCustomersCacheKey);
        }
    }
}
