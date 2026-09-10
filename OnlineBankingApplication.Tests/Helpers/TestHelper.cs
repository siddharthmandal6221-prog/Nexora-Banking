using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OnlineBankingApplication.DTOs;
using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Tests.Helpers
{
    public static class TestHelper
    {
        public static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null!, null!, null!, null!, null!, null!, null!, null!);
        }

        public static void SetupControllerContext(
            ControllerBase controller,
            string userId = "test-user-id",
            string email = "test@example.com",
            string role = "Customer")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        public static IMapper CreateMapper()
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddLogging();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<IMapper>();
        }
    }
}
