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
        public async Task<IActionResult> GetCustomers()
        {
            return Ok(await _customerService.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerRequestDto customer)
        {
            await _customerService.AddAsync(customer);
            return Ok();
        }
    }
}
