using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Services;
using OnlineBankingApplication.ViewModels;

using ApplicationUser = OnlineBankingApplication.Models.ApplicationUser;

namespace OnlineBankingApplication.Controllers
{
    [Authorize]
    public class CustomerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ICustomerService _customerService;
        private readonly IBankAccountService _bankAccountService;
        private readonly ITransactionService _transactionService;
        private readonly ICardService _cardService;
        private readonly ISubscriptionService _subscriptionService;

        public CustomerDashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ICustomerService customerService,
            IBankAccountService bankAccountService,
            ITransactionService transactionService,
            ICardService cardService,
            ISubscriptionService subscriptionService)
        {
            _context = context;
            _userManager = userManager;
            _customerService = customerService;
            _bankAccountService = bankAccountService;
            _transactionService = transactionService;
            _cardService = cardService;
            _subscriptionService = subscriptionService;
        }

        public async Task<IActionResult> Index()
        {
            // ==========================================
            // GET LOGGED-IN USER
            // ==========================================

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage(
                    "/Account/Login",
                    new
                    {
                        area = "Identity"
                    });
            }


            // ==========================================
            // FIND CUSTOMER
            // ==========================================

            var customer = _customerService
                .GetAllCustomers()
                .FirstOrDefault(c =>
                    c.ApplicationUserId == user.Id);

            if (customer == null)
            {
                var emptyViewModel =
                    new CustomerDashboardViewModel
                    {
                        CustomerName =
                            user.FullName ??
                            user.Email ??
                            "Customer"
                    };

                return View(emptyViewModel);
            }


            // ==========================================
            // GET BANK ACCOUNTS
            // ==========================================

            var accounts =
                _bankAccountService
                    .GetBankAccountsByCustomerId(
                        customer.CustomerId);

            var accountIds =
                accounts
                    .Select(a => a.BankAccountId)
                    .ToList();


            // ==========================================
            // ACCOUNT SUMMARY
            // ==========================================

            var totalBalance =
                accounts.Sum(a => a.Balance);

            var activeAccountCount =
                accounts.Count(a => a.IsActive);


            // ==========================================
            // GET TRANSACTIONS
            // ==========================================

            var allTransactions =
                _transactionService
                    .GetAllTransactions();

            var accountNumbers =
                accounts
                    .Select(a => a.AccountNumber)
                    .ToList();

            var customerTransactions =
                allTransactions
                    .Where(t =>
                        accountIds.Contains(
                            t.FromBankAccountId)
                        ||
                        accountNumbers.Contains(
                            t.ToAccountNumber))
                    .OrderByDescending(
                        t => t.TransactionDate)
                    .ToList();


            // ==========================================
            // RECENT TRANSACTIONS
            // ==========================================

            var recentTransactions =
                customerTransactions
                    .Take(5)
                    .ToList();


            // ==========================================
            // BENEFICIARIES
            // ==========================================

            var beneficiaries =
                await _context.Beneficiaries
                    .Where(b =>
                        b.CustomerId ==
                        customer.CustomerId)
                    .OrderBy(b =>
                        b.BeneficiaryName)
                    .ToListAsync();


            // ==========================================
            // CARDS
            // ==========================================

            var cards = new List<Card>();

            foreach (var account in accounts)
            {
                var accountCards =
                    _cardService
                        .GetCardsByBankAccountId(
                            account.BankAccountId);

                cards.AddRange(accountCards);
            }


            // ==========================================
            // SUBSCRIPTIONS
            // ==========================================

            var subscriptions =
                _subscriptionService
                    .GetSubscriptionsByCustomerId(
                        customer.CustomerId);


            // ==========================================
            // MONTHLY ACTIVITY
            // ==========================================

            var currentYear = DateTime.Now.Year;

            var monthlyActivity =
                new Dictionary<string, decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var monthTransactions =
                    customerTransactions
                        .Where(t =>
                            t.TransactionDate.Year ==
                                currentYear
                            &&
                            t.TransactionDate.Month ==
                                month)
                        .Sum(t => t.Amount);

                monthlyActivity.Add(
                    new DateTime(
                        currentYear,
                        month,
                        1).ToString("MMM"),
                    monthTransactions);
            }


            // ==========================================
            // BUILD VIEW MODEL
            // ==========================================

            var viewModel =
                new CustomerDashboardViewModel
                {
                    CustomerName =
                        customer.FullName,

                    TotalBalance =
                        totalBalance,

                    AccountCount =
                        accounts.Count,

                    ActiveAccountCount =
                        activeAccountCount,

                    TransactionCount =
                        customerTransactions.Count,

                    BeneficiaryCount =
                        beneficiaries.Count,

                    Accounts =
                        accounts,

                    Beneficiaries =
                        beneficiaries,

                    RecentTransactions =
                        recentTransactions,

                    Cards =
                        cards,

                    Subscriptions =
                        subscriptions,

                    MonthlyActivity =
                        monthlyActivity
                };


            return View(viewModel);
        }
    }
}