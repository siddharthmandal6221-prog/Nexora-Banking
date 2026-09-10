using AutoMapper;
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
    public class BankAccountControllerTests
    {
        private readonly Mock<IBankAccountService> _bankAccountServiceMock;
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly IMapper _mapper;

        public BankAccountControllerTests()
        {
            _bankAccountServiceMock = new Mock<IBankAccountService>();
            _customerServiceMock = new Mock<ICustomerService>();
            _userManagerMock = TestHelper.CreateMockUserManager();
            _mapper = TestHelper.CreateMapper();
        }

        private BankAccountController CreateController(string role = "Admin", string userId = "admin-1")
        {
            var controller = new BankAccountController(
                _bankAccountServiceMock.Object,
                _customerServiceMock.Object,
                _userManagerMock.Object,
                _mapper);

            TestHelper.SetupControllerContext(controller, userId: userId, role: role);
            return controller;
        }

        [Fact]
        public async Task Index_AdminUser_ReturnsViewWithAllBankAccounts()
        {
            // Arrange
            var controller = CreateController(role: "Admin");
            var user = new ApplicationUser { Id = "admin-1" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsInRoleAsync(user, "Admin"))
                .ReturnsAsync(true);

            var accounts = new List<BankAccount>
            {
                new BankAccount { BankAccountId = 1, AccountNumber = "1234567890", AccountType = "Savings", Balance = 5000 },
                new BankAccount { BankAccountId = 2, AccountNumber = "0987654321", AccountType = "Checking", Balance = 2500 }
            };
            _bankAccountServiceMock.Setup(s => s.GetAllBankAccounts())
                .Returns(accounts);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<BankAccountDto>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Details_AccountNotFound_ReturnsNotFound()
        {
            // Arrange
            var controller = CreateController(role: "Admin");
            var user = new ApplicationUser { Id = "admin-1" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _bankAccountServiceMock.Setup(s => s.GetBankAccountById(99))
                .Returns((BankAccount)null!);

            // Act
            var result = await controller.Details(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ExistingAccount_AdminUser_ReturnsViewWithDto()
        {
            // Arrange
            var controller = CreateController(role: "Admin");
            var user = new ApplicationUser { Id = "admin-1" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsInRoleAsync(user, "Admin"))
                .ReturnsAsync(true);

            var account = new BankAccount
            {
                BankAccountId = 1,
                AccountNumber = "100200300400",
                AccountType = "Savings",
                Balance = 15000,
                CustomerId = 1
            };
            _bankAccountServiceMock.Setup(s => s.GetBankAccountById(1))
                .Returns(account);

            // Act
            var result = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<BankAccountDto>(viewResult.Model);
            Assert.Equal("100200300400", model.AccountNumber);
            Assert.Equal(15000, model.Balance);
        }

        [Fact]
        public async Task Create_InvalidModelState_ReturnsViewWithDto()
        {
            // Arrange
            var controller = CreateController(role: "Admin");
            var user = new ApplicationUser { Id = "admin-1" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsInRoleAsync(user, "Admin"))
                .ReturnsAsync(true);

            controller.ModelState.AddModelError("AccountNumber", "Account number must be between 9 and 20 digits.");

            var dto = new BankAccountDto
            {
                AccountNumber = "123", // invalid
                AccountType = "Savings",
                Balance = 100,
                CustomerId = 1
            };

            // Act
            var result = await controller.Create(dto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<BankAccountDto>(viewResult.Model);
            Assert.Equal("123", model.AccountNumber);
            _bankAccountServiceMock.Verify(s => s.AddBankAccount(It.IsAny<BankAccount>()), Times.Never);
        }

        [Fact]
        public async Task Create_AdminUser_ValidModel_AddsAccountAndRedirectsToIndex()
        {
            // Arrange
            var controller = CreateController(role: "Admin");
            var user = new ApplicationUser { Id = "admin-1" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsInRoleAsync(user, "Admin"))
                .ReturnsAsync(true);

            var dto = new BankAccountDto
            {
                AccountNumber = "123456789012",
                AccountType = "Savings",
                Balance = 5000,
                CustomerId = 1
            };

            // Act
            var result = await controller.Create(dto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(BankAccountController.Index), redirectResult.ActionName);
            _bankAccountServiceMock.Verify(s => s.AddBankAccount(It.Is<BankAccount>(b =>
                b.AccountNumber == "123456789012" &&
                b.Balance == 5000)), Times.Once);
        }
    }
}
