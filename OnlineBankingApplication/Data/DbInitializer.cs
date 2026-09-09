using Microsoft.AspNetCore.Identity;
using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            UserManager<OnlineBankingApplication.Models.ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Create Admin role if it does not exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(
                    new IdentityRole("Admin"));
            }

            // Admin credentials
            var adminEmail = "admin@gmail.com";
            var adminPassword = "Admin@12345";

            // Check whether Admin user already exists
            var adminUser = await userManager
                .FindByEmailAsync(adminEmail);

            // Create Admin user if it does not exist
            if (adminUser == null)
            {
                adminUser = new OnlineBankingApplication.Models.ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Administrator",
                    IsApproved = true
                };

                var result = await userManager
                    .CreateAsync(adminUser, adminPassword);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        "Failed to create Admin user: " +
                        string.Join(", ",
                            result.Errors.Select(e => e.Description)));
                }
            }

            // Make sure Admin is approved
            if (!adminUser.IsApproved)
            {
                adminUser.IsApproved = true;

                await userManager.UpdateAsync(adminUser);
            }

            // Make sure Admin has Admin role
            if (!await userManager.IsInRoleAsync(
                    adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(
                    adminUser, "Admin");
            }
        }
    }
}