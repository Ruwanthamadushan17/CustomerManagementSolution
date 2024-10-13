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
                return View("Error", GetErrorViewModel());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _customerApiService.CreateAsync(customer);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating customer.");
                    ModelState.AddModelError("", "Failed to create customer.");
                }
            }

            return View(customer);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var customer = await _customerApiService.GetByIdAsync(id);
                if (customer == null) return NotFound();

                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading customer with id {id} for edit.");
                return View("Error", GetErrorViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CustomerViewModel customer)
        {
            if (id != customer.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _customerApiService.UpdateAsync(id, customer);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating customer with id {id}.");
                    ModelState.AddModelError("", "Failed to update customer.");
                }
            }

            return View(customer);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var customer = await _customerApiService.GetByIdAsync(id);
                if (customer == null) return NotFound();

                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading customer with id {id} for deletion.");
                return View("Error", GetErrorViewModel());
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _customerApiService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting customer with id {id}.");
                return View("Error", GetErrorViewModel());
            }
        }

        private ErrorViewModel GetErrorViewModel()
        {
            return new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
        }
    }
}
