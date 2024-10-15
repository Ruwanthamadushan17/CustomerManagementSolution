using CustomerAPI.DTOs;

namespace CustomerAPI.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponseDto>> GetAllAsync(GetRequestFilterOptions filterOptions);
        Task AddAsync(CustomerRequestDto customerDto);
        Task<CustomerResponseDto> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, CustomerRequestDto customerDto);
        Task DeleteAsync(Guid id);
    }
}
