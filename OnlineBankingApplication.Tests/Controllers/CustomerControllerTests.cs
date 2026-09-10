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
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly IMapper _mapper;

        public CustomerControllerTests()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _userManagerMock = TestHelper.CreateMockUserManager();
            _mapper = TestHelper.CreateMapper();
        }

        private CustomerController CreateController(string role = "Admin", string userId = "user-123")
        {
            var controller = new CustomerController(
                _customerServiceMock.Object,
                _userManagerMock.Object,
                _mapper);

            TestHelper.SetupControllerContext(controller, userId: userId, role: role);
            return controller;
        }

        [Fact]
        public async Task Index_AdminUser_ReturnsViewWithAllCustomers()
        {
            // Arrange
            var controller = CreateController(role: "Admin");
            var user = new ApplicationUser { Id = "admin-1", UserName = "admin@example.com" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsInRoleAsync(user, "Admin"))
                .ReturnsAsync(true);

            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, FullName = "Alice", Email = "alice@example.com", ApplicationUserId = "u1" },
                new Customer { CustomerId = 2, FullName = "Bob", Email = "bob@example.com", ApplicationUserId = "u2" }
            };
            _customerServiceMock.Setup(s => s.GetAllCustomers())
                .Returns(customers);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<CustomerDto>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Details_CustomerNotFound_ReturnsNotFound()
        {
            // Arrange
            var controller = CreateController(role: "Admin");
            var user = new ApplicationUser { Id = "admin-1" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _customerServiceMock.Setup(s => s.GetCustomerById(999))
                .Returns((Customer)null!);

            // Act
            var result = await controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ExistingCustomer_AdminUser_ReturnsViewWithCustomerDto()
        {
            // Arrange
            var controller = CreateController(role: "Admin");
            var user = new ApplicationUser { Id = "admin-1" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.IsInRoleAsync(user, "Admin"))
                .ReturnsAsync(true);

            var customer = new Customer
            {
                CustomerId = 1,
                FullName = "Alice",
                Email = "alice@example.com",
                PhoneNumber = "9876543210",
                ApplicationUserId = "u1"
            };
            _customerServiceMock.Setup(s => s.GetCustomerById(1))
                .Returns(customer);

            // Act
            var result = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CustomerDto>(viewResult.Model);
            Assert.Equal("Alice", model.FullName);
            Assert.Equal("9876543210", model.PhoneNumber);
        }

        [Fact]
        public async Task Create_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var controller = CreateController(role: "Admin");
            controller.ModelState.AddModelError("FullName", "Full Name is required.");

            _customerServiceMock.Setup(s => s.GetAllCustomers())
                .Returns(new List<Customer>());

            var customerDto = new CustomerDto
            {
                ApplicationUserId = "user-1",
                Email = "invalid",
                PhoneNumber = "123" // invalid phone
            };

            // Act
            var result = await controller.Create(customerDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CustomerDto>(viewResult.Model);
            Assert.Equal("123", model.PhoneNumber);
            _customerServiceMock.Verify(s => s.AddCustomer(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task Create_ValidModel_AddsCustomerAndRedirectsToIndex()
        {
            // Arrange
            var controller = CreateController(role: "Admin");
            _customerServiceMock.Setup(s => s.GetAllCustomers())
                .Returns(new List<Customer>());

            var customerDto = new CustomerDto
            {
                FullName = "Jane Doe",
                Email = "janedoe@example.com",
                PhoneNumber = "9876543210",
                Address = "123 Main St",
                ApplicationUserId = "user-jane"
            };

            // Act
            var result = await controller.Create(customerDto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CustomerController.Index), redirectResult.ActionName);
            _customerServiceMock.Verify(s => s.AddCustomer(It.Is<Customer>(c =>
                c.FullName == "Jane Doe" &&
                c.PhoneNumber == "9876543210")), Times.Once);
        }

        [Fact]
        public void Delete_CustomerFound_DeletesCustomerAndRedirectsToIndex()
        {
            // Arrange
            var controller = CreateController(role: "Admin");
            var customer = new Customer
            {
                CustomerId = 5,
                FullName = "To Delete",
                Email = "del@example.com",
                ApplicationUserId = "u5"
            };
            _customerServiceMock.Setup(s => s.GetCustomerById(5))
                .Returns(customer);

            // Act
            var result = controller.Delete(5, confirm: true);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CustomerController.Index), redirectResult.ActionName);
            _customerServiceMock.Verify(s => s.DeleteCustomer(customer), Times.Once);
        }
    }
}
