using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingApplication.DTOs;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Services;

using ApplicationUser = OnlineBankingApplication.Models.ApplicationUser;

namespace OnlineBankingApplication.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CustomerController(
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _customerService = customerService;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: /Customer
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                var allCustomers =
                    _customerService.GetAllCustomers();

                var customerDtos =
                    _mapper.Map<List<CustomerDto>>(allCustomers);

                return View(customerDtos);
            }

            var customer = _customerService
                .GetAllCustomers()
                .FirstOrDefault(c =>
                    c.ApplicationUserId == user.Id);

            if (customer == null)
            {
                return View(new List<CustomerDto>());
            }

            var customerDto =
                _mapper.Map<CustomerDto>(customer);

            return View(new List<CustomerDto> { customerDto });
        }

        // GET: /Customer/Create
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var customers = _customerService.GetAllCustomers();

            var customerUserIds = customers
                .Select(c => c.ApplicationUserId)
                .ToHashSet();

            var users = _userManager.Users
                .Where(u => !customerUserIds.Contains(u.Id))
                .OrderBy(u => u.FullName)
                .ToList();

            ViewBag.Users = users;

            return View();
        }

        // POST: /Customer/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerDto customerDto)
        {
            if (string.IsNullOrWhiteSpace(customerDto.ApplicationUserId))
            {
                ModelState.AddModelError(
                    nameof(CustomerDto.ApplicationUserId),
                    "Please select an Identity user.");
            }
            else
            {
                var existingCustomer = _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == customerDto.ApplicationUserId);

                if (existingCustomer != null)
                {
                    ModelState.AddModelError(
                        nameof(CustomerDto.ApplicationUserId),
                        "This user already has a customer profile.");
                }
            }

            if (ModelState.IsValid)
            {
                var customer =
                    _mapper.Map<Customer>(customerDto);

                _customerService.AddCustomer(customer);

                return RedirectToAction(nameof(Index));
            }

            var customers = _customerService.GetAllCustomers();

            var customerUserIds = customers
                .Select(c => c.ApplicationUserId)
                .ToHashSet();

            var users = _userManager.Users
                .Where(u =>
                    !customerUserIds.Contains(u.Id) ||
                    u.Id == customerDto.ApplicationUserId)
                .OrderBy(u => u.FullName)
                .ToList();

            ViewBag.Users = users;

            return View(customerDto);
        }

        // GET: /Customer/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var customer =
                _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin &&
                customer.ApplicationUserId != user.Id)
            {
                return NotFound();
            }

            var customerDto =
                _mapper.Map<CustomerDto>(customer);

            return View(customerDto);
        }

        // GET: /Customer/Edit/1
        // ADMIN ONLY
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var customer =
                _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return NotFound();
            }

            var customerDto =
                _mapper.Map<CustomerDto>(customer);

            return View(customerDto);
        }

        // POST: /Customer/Edit/1
        // ADMIN ONLY
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id,
            CustomerDto customerDto)
        {
            if (id != customerDto.CustomerId)
            {
                return NotFound();
            }

            var existingCustomer =
                _customerService.GetCustomerById(id);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            customerDto.ApplicationUserId =
                existingCustomer.ApplicationUserId;

            ModelState.Remove(nameof(CustomerDto.ApplicationUserId));

            if (ModelState.IsValid)
            {
                var customer =
                    _mapper.Map<Customer>(customerDto);

                _customerService.UpdateCustomer(customer);

                return RedirectToAction(nameof(Index));
            }

            return View(customerDto);
        }

        // GET: /Customer/Delete/1
        // ADMIN ONLY
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var customer =
                _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return NotFound();
            }

            var customerDto =
                _mapper.Map<CustomerDto>(customer);

            return View(customerDto);
        }

        // POST: /Customer/Delete/1
        // ADMIN ONLY
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(
            int id,
            bool confirm)
        {
            var customer =
                _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return NotFound();
            }

            _customerService.DeleteCustomer(customer);

            return RedirectToAction(nameof(Index));
        }
    }
}