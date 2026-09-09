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
    public class BankAccountController : Controller
    {
        private readonly IBankAccountService _bankAccountService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public BankAccountController(
            IBankAccountService bankAccountService,
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _bankAccountService = bankAccountService;
            _customerService = customerService;
            _userManager = userManager;
            _mapper = mapper;
        }

        // =========================================================
        // INDEX
        // =========================================================

        // GET: /BankAccount
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

            // ADMIN
            if (isAdmin)
            {
                var allAccounts =
                    _bankAccountService.GetAllBankAccounts();

                var accountDtos =
                    _mapper.Map<List<BankAccountDto>>(allAccounts);

                return View(accountDtos);
            }

            // CUSTOMER
            var customer = _customerService
                .GetAllCustomers()
                .FirstOrDefault(c =>
                    c.ApplicationUserId == user.Id);

            if (customer == null)
            {
                return View(new List<BankAccountDto>());
            }

            var bankAccounts =
                _bankAccountService
                    .GetBankAccountsByCustomerId(customer.CustomerId);

            var bankAccountDtos =
                _mapper.Map<List<BankAccountDto>>(bankAccounts);

            return View(bankAccountDtos);
        }

        // =========================================================
        // DETAILS
        // =========================================================

        // GET: /BankAccount/Details/1
        [HttpGet]
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

            var bankAccount =
                _bankAccountService.GetBankAccountById(id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            // CUSTOMER can only view their own account
            if (!isAdmin)
            {
                var customer = _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == user.Id);

                if (customer == null ||
                    bankAccount.CustomerId != customer.CustomerId)
                {
                    return Forbid();
                }
            }

            var bankAccountDto =
                _mapper.Map<BankAccountDto>(bankAccount);

            return View(bankAccountDto);
        }

        // =========================================================
        // CREATE
        // =========================================================

        // GET: /BankAccount/Create
        [HttpGet]
        public async Task<IActionResult> Create()
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

            // ADMIN
            if (isAdmin)
            {
                ViewBag.Customers =
                    _customerService.GetAllCustomers();

                return View();
            }

            // CUSTOMER
            var customer = _customerService
                .GetAllCustomers()
                .FirstOrDefault(c =>
                    c.ApplicationUserId == user.Id);

            if (customer == null)
            {
                return View("NoCustomerProfile");
            }

            return View();
        }

        // POST: /BankAccount/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            BankAccountDto bankAccountDto)
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

            // =====================================================
            // CUSTOMER
            // =====================================================

            if (!isAdmin)
            {
                var customer = _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == user.Id);

                if (customer == null)
                {
                    return View("NoCustomerProfile");
                }

                // NEVER trust CustomerId from the browser
                bankAccountDto.CustomerId =
                    customer.CustomerId;

                ModelState.Remove(
                    nameof(BankAccountDto.CustomerId));
            }

            // =====================================================
            // CREATE
            // =====================================================

            if (ModelState.IsValid)
            {
                var bankAccount =
                    _mapper.Map<BankAccount>(bankAccountDto);

                _bankAccountService
                    .AddBankAccount(bankAccount);

                return RedirectToAction(nameof(Index));
            }

            // Re-populate customer list if validation fails
            if (isAdmin)
            {
                ViewBag.Customers =
                    _customerService.GetAllCustomers();
            }

            return View(bankAccountDto);
        }

        // =========================================================
        // EDIT - ADMIN ONLY
        // =========================================================

        // GET: /BankAccount/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
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

            // Customers cannot edit bank accounts
            if (!isAdmin)
            {
                return Forbid();
            }

            var bankAccount =
                _bankAccountService.GetBankAccountById(id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            ViewBag.Customers =
                _customerService.GetAllCustomers();

            var bankAccountDto =
                _mapper.Map<BankAccountDto>(bankAccount);

            return View(bankAccountDto);
        }

        // POST: /BankAccount/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            BankAccountDto bankAccountDto)
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

            // Customers cannot edit bank accounts
            if (!isAdmin)
            {
                return Forbid();
            }

            if (id != bankAccountDto.BankAccountId)
            {
                return NotFound();
            }

            var existingAccount =
                _bankAccountService.GetBankAccountById(id);

            if (existingAccount == null)
            {
                return NotFound();
            }

            // Never allow ownership to be changed through the form
            bankAccountDto.CustomerId =
                existingAccount.CustomerId;

            ModelState.Remove(
                nameof(BankAccountDto.CustomerId));

            if (ModelState.IsValid)
            {
                var bankAccount =
                    _mapper.Map<BankAccount>(bankAccountDto);

                _bankAccountService
                    .UpdateBankAccount(bankAccount);

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Customers =
                _customerService.GetAllCustomers();

            return View(bankAccountDto);
        }

        // =========================================================
        // DELETE - ADMIN ONLY
        // =========================================================

        // GET: /BankAccount/Delete/1
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
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

            // Customers cannot delete bank accounts
            if (!isAdmin)
            {
                return Forbid();
            }

            var bankAccount =
                _bankAccountService.GetBankAccountById(id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            var bankAccountDto =
                _mapper.Map<BankAccountDto>(bankAccount);

            return View(bankAccountDto);
        }

        // POST: /BankAccount/Delete/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id,
            bool confirm)
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

            // Customers cannot delete bank accounts
            if (!isAdmin)
            {
                return Forbid();
            }

            var bankAccount =
                _bankAccountService.GetBankAccountById(id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            _bankAccountService
                .DeleteBankAccount(bankAccount);

            return RedirectToAction(nameof(Index));
        }
    }
}