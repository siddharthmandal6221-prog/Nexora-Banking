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
    public class ChequeBookRequestController : Controller
    {
        private readonly IChequeBookRequestService _chequeBookRequestService;
        private readonly IBankAccountService _bankAccountService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ChequeBookRequestController(
            IChequeBookRequestService chequeBookRequestService,
            IBankAccountService bankAccountService,
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _chequeBookRequestService = chequeBookRequestService;
            _bankAccountService = bankAccountService;
            _customerService = customerService;
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        // ============================================================
        // INDEX
        // ============================================================

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
                var allRequests =
                    _chequeBookRequestService
                        .GetAllChequeBookRequests();

                var dtoList =
                    _mapper.Map<List<ChequeBookRequestDto>>(
                        allRequests);

                return View(dtoList);
            }

            var customer =
                _customerService
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

            var requests =
                _chequeBookRequestService
                    .GetAllChequeBookRequests()
                    .Where(r =>
                        accountIds.Contains(
                            r.BankAccountId))
                    .ToList();

            var requestDtos =
                _mapper.Map<List<ChequeBookRequestDto>>(
                    requests);

            return View(requestDtos);
        }

        // ============================================================
        // DETAILS
        // ============================================================

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

            var request =
                _chequeBookRequestService
                    .GetChequeBookRequestById(id);

            if (request == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(
                    user,
                    "Admin");

            if (!isAdmin)
            {
                var customer =
                    _customerService
                        .GetAllCustomers()
                        .FirstOrDefault(c =>
                            c.ApplicationUserId == user.Id);

                if (customer == null ||
                    request.BankAccount == null ||
                    request.BankAccount.CustomerId !=
                    customer.CustomerId)
                {
                    return NotFound();
                }
            }

            var requestDto =
                _mapper.Map<ChequeBookRequestDto>(
                    request);

            return View(requestDto);
        }

        // ============================================================
        // CREATE - GET
        // ============================================================

        [HttpGet("/ChequeBookRequest/Create")]
        public async Task<IActionResult> Create()
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(
                    user,
                    "Admin");

            if (isAdmin)
            {
                ViewBag.BankAccounts =
                    _bankAccountService
                        .GetAllBankAccounts();
            }
            else
            {
                var customer =
                    _customerService
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
                            customer.CustomerId)
                        .Where(a => a.IsActive)
                        .ToList();
            }

            return View();
        }

        // ============================================================
        // CREATE - POST
        // ============================================================

        [HttpPost("/ChequeBookRequest/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ChequeBookRequestDto requestDto)
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(
                    user,
                    "Admin");

            // ========================================================
            // ADMIN
            // ========================================================

            if (isAdmin)
            {
                var bankAccount =
                    _bankAccountService
                        .GetBankAccountById(
                            requestDto.BankAccountId);

                if (bankAccount == null)
                {
                    ModelState.AddModelError(
                        nameof(requestDto.BankAccountId),
                        "Invalid bank account.");
                }
                else if (!bankAccount.IsActive)
                {
                    ModelState.AddModelError(
                        nameof(requestDto.BankAccountId),
                        "Bank account is inactive.");
                }

                if (requestDto.NumberOfLeaves <= 0)
                {
                    ModelState.AddModelError(
                        nameof(requestDto.NumberOfLeaves),
                        "Number of leaves must be greater than zero.");
                }

                if (ModelState.IsValid)
                {
                    requestDto.Status = "Pending";
                    requestDto.RequestDate = DateTime.Now;

                    var request =
                        _mapper.Map<ChequeBookRequest>(
                            requestDto);

                    _chequeBookRequestService
                        .AddChequeBookRequest(request);

                    var auditLog = new AuditLog
                    {
                        UserId = user.Id,

                        Action = "Cheque Book Request",

                        Description =
                            $"Requested a cheque book with " +
                            $"{requestDto.NumberOfLeaves} leaves " +
                            $"for account " +
                            $"{bankAccount!.AccountNumber}.",

                        CreatedDate = DateTime.Now
                    };

                    _context.AuditLogs.Add(auditLog);

                    await _context.SaveChangesAsync();

                    return RedirectToAction(
                        nameof(Index));
                }

                ViewBag.BankAccounts =
                    _bankAccountService
                        .GetAllBankAccounts();

                return View(requestDto);
            }

            // ========================================================
            // CUSTOMER
            // ========================================================

            var customer =
                _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == user.Id);

            if (customer == null)
            {
                return NotFound();
            }

            var customerBankAccount =
                _bankAccountService
                    .GetBankAccountById(
                        requestDto.BankAccountId);

            // Check ownership
            if (customerBankAccount == null ||
                customerBankAccount.CustomerId !=
                customer.CustomerId)
            {
                ModelState.AddModelError(
                    nameof(requestDto.BankAccountId),
                    "Invalid bank account.");
            }
            else if (!customerBankAccount.IsActive)
            {
                ModelState.AddModelError(
                    nameof(requestDto.BankAccountId),
                    "Bank account is inactive.");
            }

            // Check leaves
            if (requestDto.NumberOfLeaves <= 0)
            {
                ModelState.AddModelError(
                    nameof(requestDto.NumberOfLeaves),
                    "Number of leaves must be greater than zero.");
            }

            if (ModelState.IsValid)
            {
                requestDto.Status = "Pending";
                requestDto.RequestDate = DateTime.Now;

                var request =
                    _mapper.Map<ChequeBookRequest>(
                        requestDto);

                _chequeBookRequestService
                    .AddChequeBookRequest(request);

                var auditLog = new AuditLog
                {
                    UserId = user.Id,

                    Action = "Cheque Book Request",

                    Description =
                        $"Requested a cheque book with " +
                        $"{requestDto.NumberOfLeaves} leaves " +
                        $"for account " +
                        $"{customerBankAccount!.AccountNumber}.",

                    CreatedDate = DateTime.Now
                };

                _context.AuditLogs.Add(auditLog);

                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index));
            }

            ViewBag.BankAccounts =
                _bankAccountService
                    .GetBankAccountsByCustomerId(
                        customer.CustomerId)
                    .Where(a => a.IsActive)
                    .ToList();

            return View(requestDto);
        }

        // ============================================================
        // EDIT - GET
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var request =
                _chequeBookRequestService
                    .GetChequeBookRequestById(id);

            if (request == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(
                    user,
                    "Admin");

            if (!isAdmin)
            {
                var customer =
                    _customerService
                        .GetAllCustomers()
                        .FirstOrDefault(c =>
                            c.ApplicationUserId == user.Id);

                if (customer == null ||
                    request.BankAccount == null ||
                    request.BankAccount.CustomerId !=
                    customer.CustomerId)
                {
                    return NotFound();
                }

                if (!request.Status.Equals(
                        "Pending",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(
                        "Only pending cheque book requests can be edited.");
                }

                ViewBag.BankAccounts =
                    _bankAccountService
                        .GetBankAccountsByCustomerId(
                            customer.CustomerId);
            }
            else
            {
                if (request.BankAccount == null)
                {
                    return NotFound();
                }

                ViewBag.BankAccounts =
                    new List<BankAccount>
                    {
                        request.BankAccount
                    };
            }

            var requestDto =
                _mapper.Map<ChequeBookRequestDto>(
                    request);

            return View(requestDto);
        }

        // ============================================================
        // EDIT - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ChequeBookRequestDto chequeBookRequestDto)
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            if (id !=
                chequeBookRequestDto.ChequeBookRequestId)
            {
                return NotFound();
            }

            var existingRequest =
                _chequeBookRequestService
                    .GetChequeBookRequestById(id);

            if (existingRequest == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(
                    user,
                    "Admin");

            // CUSTOMER
            if (!isAdmin)
            {
                var customer =
                    _customerService
                        .GetAllCustomers()
                        .FirstOrDefault(c =>
                            c.ApplicationUserId == user.Id);

                if (customer == null ||
                    existingRequest.BankAccount == null ||
                    existingRequest.BankAccount.CustomerId !=
                    customer.CustomerId)
                {
                    return NotFound();
                }

                if (!existingRequest.Status.Equals(
                        "Pending",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(
                        "Only pending cheque book requests can be edited.");
                }

                var selectedAccount =
                    _bankAccountService
                        .GetBankAccountById(
                            chequeBookRequestDto.BankAccountId);

                if (selectedAccount == null ||
                    selectedAccount.CustomerId !=
                    customer.CustomerId)
                {
                    ModelState.AddModelError(
                        nameof(
                            chequeBookRequestDto.BankAccountId),
                        "Invalid bank account.");
                }

                if (selectedAccount != null &&
                    !selectedAccount.IsActive)
                {
                    ModelState.AddModelError(
                        nameof(
                            chequeBookRequestDto.BankAccountId),
                        "Bank account is inactive.");
                }

                if (chequeBookRequestDto.NumberOfLeaves <= 0)
                {
                    ModelState.AddModelError(
                        nameof(
                            chequeBookRequestDto.NumberOfLeaves),
                        "Number of leaves must be greater than zero.");
                }

                if (ModelState.IsValid)
                {
                    existingRequest.BankAccountId =
                        selectedAccount!.BankAccountId;

                    existingRequest.NumberOfLeaves =
                        chequeBookRequestDto.NumberOfLeaves;

                    _chequeBookRequestService
                        .UpdateChequeBookRequest(
                            existingRequest);

                    return RedirectToAction(
                        nameof(Index));
                }

                ViewBag.BankAccounts =
                    _bankAccountService
                        .GetBankAccountsByCustomerId(
                            customer.CustomerId);

                return View(chequeBookRequestDto);
            }

            // ADMIN

            if (existingRequest.BankAccount == null)
            {
                return NotFound();
            }

            if (chequeBookRequestDto.NumberOfLeaves <= 0)
            {
                ModelState.AddModelError(
                    nameof(
                        chequeBookRequestDto.NumberOfLeaves),
                    "Number of leaves must be greater than zero.");
            }

            if (ModelState.IsValid)
            {
                existingRequest.NumberOfLeaves =
                    chequeBookRequestDto.NumberOfLeaves;

                _chequeBookRequestService
                    .UpdateChequeBookRequest(
                        existingRequest);

                return RedirectToAction(
                    nameof(Index));
            }

            ViewBag.BankAccounts =
                new List<BankAccount>
                {
                    existingRequest.BankAccount
                };

            return View(chequeBookRequestDto);
        }

        // ============================================================
        // APPROVE
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var request =
                _chequeBookRequestService
                    .GetChequeBookRequestById(id);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Approved";

            _chequeBookRequestService
                .UpdateChequeBookRequest(request);

            var user =
                await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var auditLog = new AuditLog
                {
                    UserId = user.Id,

                    Action = "Cheque Book Approved",

                    Description =
                        $"Approved cheque book request " +
                        $"{request.ChequeBookRequestId}.",

                    CreatedDate = DateTime.Now
                };

                _context.AuditLogs.Add(auditLog);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // REJECT
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var request =
                _chequeBookRequestService
                    .GetChequeBookRequestById(id);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Rejected";

            _chequeBookRequestService
                .UpdateChequeBookRequest(request);

            var user =
                await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var auditLog = new AuditLog
                {
                    UserId = user.Id,

                    Action = "Cheque Book Rejected",

                    Description =
                        $"Rejected cheque book request " +
                        $"{request.ChequeBookRequestId}.",

                    CreatedDate = DateTime.Now
                };

                _context.AuditLogs.Add(auditLog);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // DELETE - GET
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var request =
                _chequeBookRequestService
                    .GetChequeBookRequestById(id);

            if (request == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(
                    user,
                    "Admin");

            if (!isAdmin)
            {
                var customer =
                    _customerService
                        .GetAllCustomers()
                        .FirstOrDefault(c =>
                            c.ApplicationUserId == user.Id);

                if (customer == null ||
                    request.BankAccount == null ||
                    request.BankAccount.CustomerId !=
                    customer.CustomerId)
                {
                    return NotFound();
                }

                if (!request.Status.Equals(
                        "Pending",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(
                        "Only pending cheque book requests can be deleted.");
                }
            }

            var requestDto =
                _mapper.Map<ChequeBookRequestDto>(
                    request);

            return View(requestDto);
        }

        // ============================================================
        // DELETE - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id,
            bool confirm)
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            if (!confirm)
            {
                return BadRequest();
            }

            var request =
                _chequeBookRequestService
                    .GetChequeBookRequestById(id);

            if (request == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(
                    user,
                    "Admin");

            if (!isAdmin)
            {
                var customer =
                    _customerService
                        .GetAllCustomers()
                        .FirstOrDefault(c =>
                            c.ApplicationUserId == user.Id);

                if (customer == null ||
                    request.BankAccount == null ||
                    request.BankAccount.CustomerId !=
                    customer.CustomerId)
                {
                    return NotFound();
                }

                if (!request.Status.Equals(
                        "Pending",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(
                        "Only pending cheque book requests can be deleted.");
                }
            }

            _chequeBookRequestService
                .DeleteChequeBookRequest(request);

            return RedirectToAction(nameof(Index));
        }
    }
}