using CustomerAPI.DTOs;
using CustomerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustomerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] GetRequestFilterOptions filterOptions)
        {
            return Ok(await _customerService.GetAllAsync(filterOptions));
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerRequestDto customer)
        {
            await _customerService.AddAsync(customer);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            return Ok(customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, CustomerRequestDto customer)
        {
            await _customerService.UpdateAsync(id, customer);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            await _customerService.DeleteAsync(id);
            return NoContent();
        }
    }
}
