using System.ComponentModel.DataAnnotations;

namespace OnlineBankingApplication.DTOs
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateTime CreatedDate { get; set; }

        public string ApplicationUserId { get; set; }
    }
}