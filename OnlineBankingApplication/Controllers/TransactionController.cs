using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.DTOs;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Services;
using OnlineBankingApplication.ViewModels;

using ApplicationUser = OnlineBankingApplication.Models.ApplicationUser;

namespace OnlineBankingApplication.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IBankAccountService _bankAccountService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TransactionController(
            ITransactionService transactionService,
            IBankAccountService bankAccountService,
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _transactionService = transactionService;
            _bankAccountService = bankAccountService;
            _customerService = customerService;
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        // ============================================
        // TRANSACTION HISTORY
        // ============================================

        public async Task<IActionResult> Index(
            string? transactionType,
            string? status,
            DateTime? fromDate,
            DateTime? toDate)
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

            List<Transaction> transactions;

            if (isAdmin)
            {
                transactions =
                    _transactionService
                        .GetAllTransactions()
                        .ToList();
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

                var bankAccounts =
                    _bankAccountService
                        .GetBankAccountsByCustomerId(
                            customer.CustomerId);

                var accountIds =
                    bankAccounts
                        .Select(a => a.BankAccountId)
                        .ToList();

                var accountNumbers =
                    bankAccounts
                        .Select(a => a.AccountNumber)
                        .ToList();

                transactions =
                    _transactionService
                        .GetAllTransactions()
                        .Where(t =>
                            accountIds.Contains(
                                t.FromBankAccountId)
                            ||
                            accountNumbers.Contains(
                                t.ToAccountNumber))
                        .ToList();
            }

            if (!string.IsNullOrWhiteSpace(transactionType))
            {
                transactions =
                    transactions
                        .Where(t =>
                            t.TransactionType.Equals(
                                transactionType,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                transactions =
                    transactions
                        .Where(t =>
                            t.Status.Equals(
                                status,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }

            if (fromDate.HasValue)
            {
                var startDate = fromDate.Value.Date;

                transactions =
                    transactions
                        .Where(t =>
                            t.TransactionDate >= startDate)
                        .ToList();
            }

            if (toDate.HasValue)
            {
                var endDate =
                    toDate.Value.Date.AddDays(1);

                transactions =
                    transactions
                        .Where(t =>
                            t.TransactionDate < endDate)
                        .ToList();
            }

            transactions =
                transactions
                    .OrderByDescending(t =>
                        t.TransactionDate)
                    .ToList();

            decimal totalIncoming = 0;
            decimal totalOutgoing = 0;

            if (!isAdmin)
            {
                var customer =
                    _customerService
                        .GetAllCustomers()
                        .FirstOrDefault(c =>
                            c.ApplicationUserId == user.Id);

                if (customer != null)
                {
                    var customerAccounts =
                        _bankAccountService
                            .GetBankAccountsByCustomerId(
                                customer.CustomerId);

                    var accountIds =
                        customerAccounts
                            .Select(a => a.BankAccountId)
                            .ToList();

                    var accountNumbers =
                        customerAccounts
                            .Select(a => a.AccountNumber)
                            .ToList();

                    totalOutgoing =
                        transactions
                            .Where(t =>
                                accountIds.Contains(
                                    t.FromBankAccountId))
                            .Sum(t => t.Amount);

                    totalIncoming =
                        transactions
                            .Where(t =>
                                accountNumbers.Contains(
                                    t.ToAccountNumber)
                                &&
                                !accountIds.Contains(
                                    t.FromBankAccountId))
                            .Sum(t => t.Amount);
                }
            }
            else
            {
                totalOutgoing =
                    transactions.Sum(t => t.Amount);
            }

            var viewModel =
                new TransactionHistoryViewModel
                {
                    Transactions = transactions,
                    TransactionType = transactionType,
                    Status = status,
                    FromDate = fromDate,
                    ToDate = toDate,
                    TotalOutgoing = totalOutgoing,
                    TotalIncoming = totalIncoming,
                    TransactionCount = transactions.Count
                };

            return View(viewModel);
        }

        // ============================================
        // TRANSACTION DETAILS
        // ============================================

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });
            }

            var transaction =
                _transactionService
                    .GetTransactionById(id);

            if (transaction == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
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

                var customerAccounts =
                    _bankAccountService
                        .GetBankAccountsByCustomerId(
                            customer.CustomerId);

                var accountIds =
                    customerAccounts
                        .Select(a => a.BankAccountId)
                        .ToList();

                var accountNumbers =
                    customerAccounts
                        .Select(a => a.AccountNumber)
                        .ToList();

                var isOwnTransaction =
                    accountIds.Contains(
                        transaction.FromBankAccountId)
                    ||
                    accountNumbers.Contains(
                        transaction.ToAccountNumber);

                if (!isOwnTransaction)
                {
                    return NotFound();
                }
            }

            var transactionDto =
                _mapper.Map<TransactionDto>(transaction);

            return View(transactionDto);
        }

        // ============================================
        // ACCOUNT STATEMENT
        // ============================================

        [HttpGet]
        public async Task<IActionResult> Statement(
            int? accountId,
            DateTime? fromDate,
            DateTime? toDate)
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

            List<BankAccount> availableAccounts;

            if (isAdmin)
            {
                availableAccounts =
                    _bankAccountService
                        .GetAllBankAccounts()
                        .ToList();
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

                availableAccounts =
                    _bankAccountService
                        .GetBankAccountsByCustomerId(
                            customer.CustomerId)
                        .ToList();
            }

            if (!accountId.HasValue)
            {
                ViewBag.BankAccounts =
                    availableAccounts;

                return View(
                    new AccountStatementViewModel());
            }

            var account =
                availableAccounts
                    .FirstOrDefault(a =>
                        a.BankAccountId == accountId.Value);

            if (account == null)
            {
                return NotFound();
            }

            var allTransactions =
                _transactionService
                    .GetAllTransactions()
                    .Where(t =>
                        t.FromBankAccountId ==
                            account.BankAccountId
                        ||
                        t.ToAccountNumber ==
                            account.AccountNumber)
                    .OrderBy(t =>
                        t.TransactionDate)
                    .ToList();

            decimal totalCredits =
                allTransactions
                    .Where(t =>
                        t.ToAccountNumber ==
                            account.AccountNumber
                        &&
                        t.FromBankAccountId !=
                            account.BankAccountId)
                    .Sum(t => t.Amount);

            decimal totalDebits =
                allTransactions
                    .Where(t =>
                        t.FromBankAccountId ==
                            account.BankAccountId)
                    .Sum(t => t.Amount);

            decimal openingBalance =
                account.Balance
                - totalCredits
                + totalDebits;

            var statementTransactions =
                allTransactions;

            if (fromDate.HasValue)
            {
                statementTransactions =
                    statementTransactions
                        .Where(t =>
                            t.TransactionDate >=
                            fromDate.Value.Date)
                        .ToList();
            }

            if (toDate.HasValue)
            {
                var endDate =
                    toDate.Value.Date.AddDays(1);

                statementTransactions =
                    statementTransactions
                        .Where(t =>
                            t.TransactionDate < endDate)
                        .ToList();
            }

            decimal filteredCredits =
                statementTransactions
                    .Where(t =>
                        t.ToAccountNumber ==
                            account.AccountNumber
                        &&
                        t.FromBankAccountId !=
                            account.BankAccountId)
                    .Sum(t => t.Amount);

            decimal filteredDebits =
                statementTransactions
                    .Where(t =>
                        t.FromBankAccountId ==
                            account.BankAccountId)
                    .Sum(t => t.Amount);

            var viewModel =
                new AccountStatementViewModel
                {
                    Account = account,

                    Transactions =
                        statementTransactions
                            .OrderByDescending(t =>
                                t.TransactionDate)
                            .ToList(),

                    FromDate = fromDate,

                    ToDate = toDate,

                    TotalCredits =
                        filteredCredits,

                    TotalDebits =
                        filteredDebits,

                    CurrentBalance =
                        account.Balance,

                    TransactionCount =
                        statementTransactions.Count
                };

            ViewBag.BankAccounts =
                availableAccounts;

            ViewBag.OpeningBalance =
                openingBalance;

            return View(viewModel);
        }

        // ============================================
        // CREATE TRANSACTION - GET
        // ============================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                ViewBag.BankAccounts =
                    _bankAccountService
                        .GetAllBankAccounts();

                return View();
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

            ViewBag.BankAccounts =
                _bankAccountService
                    .GetBankAccountsByCustomerId(
                        customer.CustomerId);

            return View();
        }

        // ============================================
        // CREATE TRANSACTION - POST
        // ============================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            TransactionDto transactionDto)
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            Customer? customer = null;

            if (!isAdmin)
            {
                customer =
                    _customerService
                        .GetAllCustomers()
                        .FirstOrDefault(c =>
                            c.ApplicationUserId == user.Id);

                if (customer == null)
                {
                    return NotFound();
                }
            }

            var fromAccount =
                _bankAccountService
                    .GetBankAccountById(
                        transactionDto.FromBankAccountId);

            if (fromAccount == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "From account not found.");
            }
            else
            {
                if (!isAdmin &&
                    fromAccount.CustomerId !=
                        customer!.CustomerId)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "You can only transfer money from your own account.");
                }

                if (!fromAccount.IsActive)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "The source account is inactive.");
                }

                if (transactionDto.Amount <= 0)
                {
                    ModelState.AddModelError(
                        nameof(transactionDto.Amount),
                        "Amount must be greater than zero.");
                }

                if (transactionDto.Amount >
                    fromAccount.Balance)
                {
                    ModelState.AddModelError(
                        nameof(transactionDto.Amount),
                        "Insufficient balance.");
                }
            }

            var toAccount =
                _bankAccountService
                    .GetAllBankAccounts()
                    .FirstOrDefault(a =>
                        a.AccountNumber ==
                        transactionDto.ToAccountNumber);

            Beneficiary? beneficiary = null;

            // Destination validation
            if (toAccount == null)
            {
                if (!isAdmin)
                {
                    beneficiary =
                        _context.Beneficiaries
                            .FirstOrDefault(b =>
                                b.AccountNumber ==
                                    transactionDto.ToAccountNumber
                                &&
                                b.CustomerId ==
                                    customer!.CustomerId);

                    if (beneficiary == null)
                    {
                        ModelState.AddModelError(
                            nameof(transactionDto.ToAccountNumber),
                            "Destination account or beneficiary not found.");
                    }
                }
                else
                {
                    ModelState.AddModelError(
                        nameof(transactionDto.ToAccountNumber),
                        "Destination bank account not found.");
                }
            }

            if (toAccount != null)
            {
                if (!toAccount.IsActive)
                {
                    ModelState.AddModelError(
                        nameof(transactionDto.ToAccountNumber),
                        "Destination account is inactive.");
                }

                if (fromAccount != null &&
                    fromAccount.AccountNumber ==
                        toAccount.AccountNumber)
                {
                    ModelState.AddModelError(
                        nameof(transactionDto.ToAccountNumber),
                        "You cannot transfer money to the same account.");
                }
            }

            if (!ModelState.IsValid)
            {
                if (isAdmin)
                {
                    ViewBag.BankAccounts =
                        _bankAccountService
                            .GetAllBankAccounts();
                }
                else
                {
                    ViewBag.BankAccounts =
                        _bankAccountService
                            .GetBankAccountsByCustomerId(
                                customer!.CustomerId);
                }

                return View(transactionDto);
            }

            using var dbTransaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                fromAccount!.Balance -=
                    transactionDto.Amount;

                if (toAccount != null)
                {
                    toAccount.Balance +=
                        transactionDto.Amount;

                    _bankAccountService
                        .UpdateBankAccount(toAccount);
                }

                transactionDto.Status =
                    "Success";

                transactionDto.TransactionDate =
                    DateTime.Now;

                _bankAccountService
                    .UpdateBankAccount(fromAccount);

                var transaction =
                    _mapper.Map<Transaction>(
                        transactionDto);

                _transactionService
                    .AddTransaction(transaction);

                string destinationDescription;

                if (toAccount != null)
                {
                    destinationDescription =
                        $"bank account {toAccount.AccountNumber}";
                }
                else if (beneficiary != null)
                {
                    destinationDescription =
                        $"beneficiary {beneficiary.BeneficiaryName} " +
                        $"({beneficiary.AccountNumber})";
                }
                else
                {
                    destinationDescription =
                        "unknown destination";
                }

                var auditLog =
                    new AuditLog
                    {
                        UserId = user.Id,

                        Action = "Money Transfer",

                        Description =
                            $"Transferred {transactionDto.Amount:C} " +
                            $"from account {fromAccount.AccountNumber} " +
                            $"to {destinationDescription}.",

                        CreatedDate =
                            DateTime.Now
                    };

                _context.AuditLogs.Add(auditLog);

                await _context.SaveChangesAsync();

                await dbTransaction.CommitAsync();

                return RedirectToAction(
                    nameof(Index));
            }
            catch
            {
                await dbTransaction.RollbackAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Transaction failed. No money was transferred.");

                if (isAdmin)
                {
                    ViewBag.BankAccounts =
                        _bankAccountService
                            .GetAllBankAccounts();
                }
                else
                {
                    ViewBag.BankAccounts =
                        _bankAccountService
                            .GetBankAccountsByCustomerId(
                                customer!.CustomerId);
                }

                return View(transactionDto);
            }
        }
    }
}