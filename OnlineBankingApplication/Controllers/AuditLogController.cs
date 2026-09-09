using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Data;

namespace OnlineBankingApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var auditLogs = _context.AuditLogs
                .OrderByDescending(a => a.CreatedDate)
                .ToList();

            return View(auditLogs);
        }
    }
}