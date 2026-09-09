using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingApplication.Data;

namespace OnlineBankingApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Dashboard
        public IActionResult Index()
        {
            var totalCustomers = _context.Customers.Count();

            var totalBankAccounts = _context.BankAccounts.Count();

            var totalBalance = _context.BankAccounts
                .Sum(a => a.Balance);

            var totalTransactions = _context.Transactions.Count();

            var totalBillPayments = _context.BillPayments.Count();

            var pendingChequeBookRequests = _context.ChequeBookRequests
                .Count(c => c.Status == "Pending");

            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.TotalBankAccounts = totalBankAccounts;
            ViewBag.TotalBalance = totalBalance;
            ViewBag.TotalTransactions = totalTransactions;
            ViewBag.TotalBillPayments = totalBillPayments;
            ViewBag.PendingChequeBookRequests = pendingChequeBookRequests;

            return View();
        }
    }
}