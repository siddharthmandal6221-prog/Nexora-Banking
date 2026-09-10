using System.ComponentModel.DataAnnotations;
using OnlineBankingApplication.Areas.Identity.Pages.Account;
using OnlineBankingApplication.DTOs;
using OnlineBankingApplication.Models;
using Xunit;

namespace OnlineBankingApplication.Tests.Validation
{
    public class ValidationTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            return results;
        }

        [Theory]
        [InlineData("12345678901")] // 11 digits - should fail
        [InlineData("1234567890123")] // 13 digits - should fail
        [InlineData("123456789")] // 9 digits - should fail
        [InlineData("12345abcde")] // Letters - should fail
        [InlineData("")] // Empty - should fail
        public void RegisterInputModel_InvalidPhoneNumber_FailsValidation(string invalidPhone)
        {
            // Arrange
            var input = new RegisterModel.InputModel
            {
                FullName = "John Doe",
                Email = "johndoe@example.com",
                PhoneNumber = invalidPhone,
                Address = "123 Main Street",
                Password = "Password@123",
                ConfirmPassword = "Password@123"
            };

            // Act
            var results = ValidateModel(input);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterModel.InputModel.PhoneNumber)));
        }

        [Fact]
        public void RegisterInputModel_Valid10DigitPhoneNumber_PassesValidation()
        {
            // Arrange
            var input = new RegisterModel.InputModel
            {
                FullName = "John Doe",
                Email = "johndoe@example.com",
                PhoneNumber = "9876543210",
                Address = "123 Main Street",
                Password = "Password@123",
                ConfirmPassword = "Password@123"
            };

            // Act
            var results = ValidateModel(input);

            // Assert
            Assert.DoesNotContain(results, r => r.MemberNames.Contains(nameof(RegisterModel.InputModel.PhoneNumber)));
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("98765432100")] // 11 digits
        [InlineData("98765")] // 5 digits
        [InlineData("abcdefghij")] // Non-numeric
        public void CustomerDto_InvalidPhoneNumber_FailsValidation(string invalidPhone)
        {
            // Arrange
            var dto = new CustomerDto
            {
                FullName = "John Doe",
                Email = "johndoe@example.com",
                PhoneNumber = invalidPhone,
                ApplicationUserId = "user-123"
            };

            // Act
            var results = ValidateModel(dto);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CustomerDto.PhoneNumber)));
        }

        [Fact]
        public void CustomerDto_Valid10DigitPhoneNumber_PassesValidation()
        {
            // Arrange
            var dto = new CustomerDto
            {
                FullName = "John Doe",
                Email = "johndoe@example.com",
                PhoneNumber = "9876543210",
                ApplicationUserId = "user-123"
            };

            // Act
            var results = ValidateModel(dto);

            // Assert
            Assert.DoesNotContain(results, r => r.MemberNames.Contains(nameof(CustomerDto.PhoneNumber)));
        }

        [Theory]
        [InlineData("123")] // Too short (<9 digits)
        [InlineData("123456789012345678901")] // Too long (>20 digits)
        [InlineData("12345678ABCD")] // Non-numeric
        public void BankAccountDto_InvalidAccountNumber_FailsValidation(string invalidAccount)
        {
            // Arrange
            var dto = new BankAccountDto
            {
                AccountNumber = invalidAccount,
                AccountType = "Savings",
                Balance = 1000,
                CustomerId = 1
            };

            // Act
            var results = ValidateModel(dto);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(BankAccountDto.AccountNumber)));
        }

        [Fact]
        public void BankAccountDto_ValidAccountNumber_PassesValidation()
        {
            // Arrange
            var dto = new BankAccountDto
            {
                AccountNumber = "123456789012",
                AccountType = "Savings",
                Balance = 1000,
                CustomerId = 1
            };

            // Act
            var results = ValidateModel(dto);

            // Assert
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("12")] // Not 4 digits
        [InlineData("12345")] // 5 digits
        [InlineData("abcd")] // Letters
        public void CardDto_InvalidLastFourDigits_FailsValidation(string invalidDigits)
        {
            // Arrange
            var dto = new CardDto
            {
                LastFourDigits = invalidDigits,
                CardHolderName = "John Doe",
                CardNetwork = "VISA",
                CardType = "Debit Card",
                ExpiryMonth = 5,
                ExpiryYear = 2027,
                BankAccountId = 1
            };

            // Act
            var results = ValidateModel(dto);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CardDto.LastFourDigits)));
        }
    }
}
