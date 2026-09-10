using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineBankingApplication.Controllers;
using OnlineBankingApplication.DTOs;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Services;
using OnlineBankingApplication.Tests.Helpers;
using Xunit;

namespace OnlineBankingApplication.Tests.Controllers
{
    public class PaymentControllerTests
    {
        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        public PaymentControllerTests()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _userManagerMock = TestHelper.CreateMockUserManager();
        }

        private PaymentController CreateController(string userId = "user-1")
        {
            var controller = new PaymentController(
                _paymentServiceMock.Object,
                _userManagerMock.Object);

            TestHelper.SetupControllerContext(controller, userId: userId);
            return controller;
        }

        [Fact]
        public async Task MakePayment_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController();
            controller.ModelState.AddModelError("Amount", "Amount must be between 1 and 10,000,000.");

            var paymentDto = new PaymentDto
            {
                FromBankAccountId = 1,
                ToAccountNumber = "123456789012",
                Amount = -50
            };

            // Act
            var result = await controller.MakePayment(paymentDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _paymentServiceMock.Verify(s => s.MakePaymentAsync(It.IsAny<PaymentDto>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task MakePayment_UserNotFound_ReturnsUnauthorized()
        {
            // Arrange
            var controller = CreateController();
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((ApplicationUser?)null);

            var paymentDto = new PaymentDto
            {
                FromBankAccountId = 1,
                ToAccountNumber = "123456789012",
                Amount = 500
            };

            // Act
            var result = await controller.MakePayment(paymentDto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task MakePayment_PaymentFails_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController(userId: "user-123");
            var user = new ApplicationUser { Id = "user-123" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var paymentDto = new PaymentDto
            {
                FromBankAccountId = 1,
                ToAccountNumber = "123456789012",
                Amount = 1000
            };

            _paymentServiceMock.Setup(s => s.MakePaymentAsync(paymentDto, "user-123"))
                .ReturnsAsync(new PaymentResultDto
                {
                    Success = false,
                    Message = "Insufficient balance."
                });

            // Act
            var result = await controller.MakePayment(paymentDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var resultDto = Assert.IsType<PaymentResultDto>(badRequestResult.Value);
            Assert.False(resultDto.Success);
            Assert.Equal("Insufficient balance.", resultDto.Message);
        }

        [Fact]
        public async Task MakePayment_PaymentSucceeds_ReturnsOkResult()
        {
            // Arrange
            var controller = CreateController(userId: "user-123");
            var user = new ApplicationUser { Id = "user-123" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var paymentDto = new PaymentDto
            {
                FromBankAccountId = 1,
                ToAccountNumber = "123456789012",
                Amount = 500
            };

            _paymentServiceMock.Setup(s => s.MakePaymentAsync(paymentDto, "user-123"))
                .ReturnsAsync(new PaymentResultDto
                {
                    Success = true,
                    Message = "Payment successful.",
                    TransactionId = 101,
                    FromAccountNumber = "999888777666",
                    ToAccountNumber = "123456789012",
                    Amount = 500
                });

            // Act
            var result = await controller.MakePayment(paymentDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultDto = Assert.IsType<PaymentResultDto>(okResult.Value);
            Assert.True(resultDto.Success);
            Assert.Equal(101, resultDto.TransactionId);
        }
    }
}
