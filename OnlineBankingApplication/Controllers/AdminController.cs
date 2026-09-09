using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.ViewModels;

using ApplicationUser = OnlineBankingApplication.Models.ApplicationUser;

namespace OnlineBankingApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalCustomers = await _context.Customers.CountAsync(),

                TotalBankAccounts = await _context.BankAccounts.CountAsync(),

                TotalTransactions = await _context.Transactions.CountAsync(),

                TotalTransactionAmount = await _context.Transactions
                    .Where(t => t.Status == "Success")
                    .SumAsync(t => (decimal?)t.Amount) ?? 0,

                PendingChequeRequests = await _context.ChequeBookRequests
                    .CountAsync(c => c.Status == "Pending"),

                TotalBeneficiaries = await _context.Beneficiaries.CountAsync(),

                TotalBillPayments = await _context.BillPayments.CountAsync(),

                TotalBillPaymentAmount = await _context.BillPayments
                    .Where(b => b.Status == "Success")
                    .SumAsync(b => (decimal?)b.Amount) ?? 0,

                RecentTransactions = await _context.Transactions
                    .Include(t => t.FromBankAccount)
                    .OrderByDescending(t => t.TransactionDate)
                    .Take(5)
                    .ToListAsync(),

                RecentAuditLogs = await _context.AuditLogs
                    .OrderByDescending(a => a.CreatedDate)
                    .Take(5)
                    .ToListAsync(),

                Users = users
            };

            return View(viewModel);
        }

        // POST: /Admin/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsApproved = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsApproved = false;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}