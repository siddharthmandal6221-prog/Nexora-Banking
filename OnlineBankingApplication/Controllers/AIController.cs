using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingApplication.AI;
using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Controllers
{
    [Authorize]
    public class AIController : Controller
    {
        private readonly IAIService _aiService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AIController(
            IAIService aiService,
            UserManager<ApplicationUser> userManager)
        {
            _aiService = aiService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ask(
            string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return Json(new
                {
                    success = false,
                    response =
                        "Please enter a question."
                });
            }

            if (message.Length > 1000)
            {
                return Json(new
                {
                    success = false,
                    response =
                        "Please keep your question within 1000 characters."
                });
            }

            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    response =
                        "Your session has expired. Please log in again."
                });
            }

            var response =
                await _aiService.GetResponseAsync(
                    message,
                    user.Id);

            return Json(new
            {
                success = true,
                response = response
            });
        }
    }
}