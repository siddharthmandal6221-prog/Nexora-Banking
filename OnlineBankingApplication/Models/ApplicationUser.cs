using Microsoft.AspNetCore.Identity;

namespace OnlineBankingApplication.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}