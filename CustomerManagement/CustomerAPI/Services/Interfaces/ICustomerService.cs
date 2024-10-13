using CustomerAPI.DTOs;

namespace CustomerAPI.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponseDto>> GetAllAsync();
    }
}
