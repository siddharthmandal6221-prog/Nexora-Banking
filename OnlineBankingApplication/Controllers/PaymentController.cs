using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingApplication.DTOs;
using OnlineBankingApplication.Services;

namespace OnlineBankingApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly UserManager<OnlineBankingApplication.Models.ApplicationUser> _userManager;

        public PaymentController(
            IPaymentService paymentService,
            UserManager<OnlineBankingApplication.Models.ApplicationUser> userManager)
        {
            _paymentService = paymentService;
            _userManager = userManager;
        }

        // POST: /api/payment
        [HttpPost]
        public async Task<IActionResult> MakePayment(
            [FromBody] PaymentDto paymentDto)
        {
            // =========================================================
            // REQUEST VALIDATION
            // =========================================================

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // =========================================================
            // GET LOGGED-IN USER
            // =========================================================

            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "User authentication is required."
                });
            }

            // =========================================================
            // MAKE PAYMENT
            // =========================================================

            var result =
                await _paymentService.MakePaymentAsync(
                    paymentDto,
                    user.Id);

            // =========================================================
            // PAYMENT FAILED
            // =========================================================

            if (!result.Success)
            {
                return BadRequest(result);
            }

            // =========================================================
            // PAYMENT SUCCESS
            // =========================================================

            return Ok(result);
        }
    }
}