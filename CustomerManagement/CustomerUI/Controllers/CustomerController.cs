using CustomerUI.Models;
using CustomerUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CustomerUI.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerApiService _customerApiService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerApiService customerService, ILogger<CustomerController> logger)
        {
            _customerApiService = customerService;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var customers = await _customerApiService.GetAllCustomersAsync();
                return View(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading customers.");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            
        }
    }
}
