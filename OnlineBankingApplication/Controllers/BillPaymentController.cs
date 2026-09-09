using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.DTOs;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Services;

using ApplicationUser = OnlineBankingApplication.Models.ApplicationUser;

namespace OnlineBankingApplication.Controllers
{
    [Authorize]
    public class BillPaymentController : Controller
    {
        private readonly IBillPaymentService _billPaymentService;
        private readonly IBankAccountService _bankAccountService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BillPaymentController(
            IBillPaymentService billPaymentService,
            IBankAccountService bankAccountService,
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _billPaymentService = billPaymentService;
            _bankAccountService = bankAccountService;
            _customerService = customerService;
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        // GET: /BillPayment
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            // Admin can see all bill payments
            if (isAdmin)
            {
                var allPayments =
                    _billPaymentService.GetAllBillPayments();

                var paymentDtos =
                    _mapper.Map<List<BillPaymentDto>>(allPayments);

                return View(paymentDtos);
            }

            // Normal user can see only own payments
            var customer = _customerService
                .GetAllCustomers()
                .FirstOrDefault(c =>
                    c.ApplicationUserId == user.Id);

            if (customer == null)
            {
                return NotFound();
            }

            var bankAccounts =
                _bankAccountService
                    .GetBankAccountsByCustomerId(
                        customer.CustomerId);

            var accountIds =
                bankAccounts
                    .Select(a => a.BankAccountId)
                    .ToList();

            var payments =
                _billPaymentService
                    .GetAllBillPayments()
                    .Where(p =>
                        accountIds.Contains(p.BankAccountId))
                    .ToList();

            var paymentDtoList =
                _mapper.Map<List<BillPaymentDto>>(payments);

            return View(paymentDtoList);
        }

        // GET: /BillPayment/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });
            }

            var billPayment =
                _billPaymentService.GetBillPaymentById(id);

            if (billPayment == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            // Admin can view any payment
            if (isAdmin)
            {
                var paymentDto =
                    _mapper.Map<BillPaymentDto>(billPayment);

                return View(paymentDto);
            }

            // Normal user can view only own payment
            var customer = _customerService
                .GetAllCustomers()
                .FirstOrDefault(c =>
                    c.ApplicationUserId == user.Id);

            if (customer == null)
            {
                return NotFound();
            }

            if (billPayment.BankAccount == null)
            {
                return NotFound();
            }

            if (billPayment.BankAccount.CustomerId !=
                customer.CustomerId)
            {
                return NotFound();
            }

            var billPaymentDto =
                _mapper.Map<BillPaymentDto>(billPayment);

            return View(billPaymentDto);
        }

        // GET: /BillPayment/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            // Admin can use any bank account
            if (isAdmin)
            {
                ViewBag.BankAccounts =
                    _bankAccountService.GetAllBankAccounts();

                return View();
            }

            // Normal user gets only own accounts
            var customer = _customerService
                .GetAllCustomers()
                .FirstOrDefault(c =>
                    c.ApplicationUserId == user.Id);

            if (customer == null)
            {
                return NotFound();
            }

            ViewBag.BankAccounts =
                _bankAccountService
                    .GetBankAccountsByCustomerId(
                        customer.CustomerId);

            return View();
        }

        // POST: /BillPayment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            BillPaymentDto billPaymentDto)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            Customer? customer = null;

            // Normal user's customer
            if (!isAdmin)
            {
                customer = _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == user.Id);

                if (customer == null)
                {
                    return NotFound();
                }
            }

            // Find bank account
            var bankAccount =
                _bankAccountService.GetBankAccountById(
                    billPaymentDto.BankAccountId);

            if (bankAccount == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Bank account not found.");
            }
            else
            {
                // Normal user can use only own account
                if (!isAdmin &&
                    bankAccount.CustomerId != customer!.CustomerId)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "You can only make payment from your own account.");
                }

                if (!bankAccount.IsActive)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "The bank account is inactive.");
                }

                if (billPaymentDto.Amount <= 0)
                {
                    ModelState.AddModelError(
                        nameof(billPaymentDto.Amount),
                        "Amount must be greater than zero.");
                }

                if (billPaymentDto.Amount > bankAccount.Balance)
                {
                    ModelState.AddModelError(
                        nameof(billPaymentDto.Amount),
                        "Insufficient balance.");
                }
            }

            if (!ModelState.IsValid)
            {
                if (isAdmin)
                {
                    ViewBag.BankAccounts =
                        _bankAccountService.GetAllBankAccounts();
                }
                else
                {
                    ViewBag.BankAccounts =
                        _bankAccountService
                            .GetBankAccountsByCustomerId(
                                customer!.CustomerId);
                }

                return View(billPaymentDto);
            }

            // Map DTO to Entity
            var billPayment =
                _mapper.Map<BillPayment>(billPaymentDto);

            // DATABASE TRANSACTION
            using var dbTransaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                // Deduct amount from account
                bankAccount!.Balance -=
                    billPaymentDto.Amount;

                billPayment.Status = "Success";
                billPayment.PaymentDate = DateTime.Now;

                // Update bank account
                _bankAccountService
                    .UpdateBankAccount(bankAccount);

                // Add bill payment
                _billPaymentService
                    .AddBillPayment(billPayment);

                // Add audit log
                var auditLog = new AuditLog
                {
                    UserId = user.Id,

                    Action = "Bill Payment",

                    Description =
                        $"Paid {billPayment.Amount:C} for " +
                        $"{billPayment.BillerName} using account " +
                        $"{bankAccount.AccountNumber}.",

                    CreatedDate = DateTime.Now
                };

                _context.AuditLogs.Add(auditLog);

                // Save everything
                await _context.SaveChangesAsync();

                // Commit only when everything succeeds
                await dbTransaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                // Roll back account/payment/audit changes
                await dbTransaction.RollbackAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Bill payment failed. No money was deducted.");

                if (isAdmin)
                {
                    ViewBag.BankAccounts =
                        _bankAccountService.GetAllBankAccounts();
                }
                else
                {
                    ViewBag.BankAccounts =
                        _bankAccountService
                            .GetBankAccountsByCustomerId(
                                customer!.CustomerId);
                }

                return View(billPaymentDto);
            }
        }
    }
}