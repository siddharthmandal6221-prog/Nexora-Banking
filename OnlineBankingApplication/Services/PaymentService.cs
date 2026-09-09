using Microsoft.AspNetCore.Identity;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.DTOs;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<OnlineBankingApplication.Models.ApplicationUser> _userManager;

        public PaymentService(
            IPaymentRepository paymentRepository,
            ApplicationDbContext context,
            UserManager<OnlineBankingApplication.Models.ApplicationUser> userManager)
        {
            _paymentRepository = paymentRepository;
            _context = context;
            _userManager = userManager;
        }

        public async Task<PaymentResultDto> MakePaymentAsync(
            PaymentDto paymentDto,
            string userId)
        {
            // =========================================================
            // GET CURRENT LOGGED-IN USER
            // =========================================================

            var user =
                await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new PaymentResultDto
                {
                    Success = false,
                    Message = "Authenticated user not found."
                };
            }

            // =========================================================
            // CHECK ADMIN ROLE
            // =========================================================

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            // =========================================================
            // FIND SOURCE ACCOUNT
            // =========================================================

            var fromAccount =
                _paymentRepository.GetBankAccountById(
                    paymentDto.FromBankAccountId);

            if (fromAccount == null)
            {
                return new PaymentResultDto
                {
                    Success = false,
                    Message = "Source bank account not found."
                };
            }

            // =========================================================
            // OWNERSHIP CHECK
            // CUSTOMER CAN USE ONLY THEIR OWN ACCOUNT
            // =========================================================

            if (!isAdmin)
            {
                var customer =
                    _context.Customers
                        .FirstOrDefault(c =>
                            c.ApplicationUserId == userId);

                if (customer == null)
                {
                    return new PaymentResultDto
                    {
                        Success = false,
                        Message = "Customer profile not found."
                    };
                }

                if (fromAccount.CustomerId != customer.CustomerId)
                {
                    return new PaymentResultDto
                    {
                        Success = false,
                        Message =
                            "You can only make payments from your own bank account."
                    };
                }
            }

            // =========================================================
            // SOURCE ACCOUNT VALIDATION
            // =========================================================

            if (!fromAccount.IsActive)
            {
                return new PaymentResultDto
                {
                    Success = false,
                    Message = "Source bank account is inactive."
                };
            }

            // =========================================================
            // FIND DESTINATION ACCOUNT
            // =========================================================

            var toAccount =
                _paymentRepository.GetBankAccountByNumber(
                    paymentDto.ToAccountNumber);

            if (toAccount == null)
            {
                return new PaymentResultDto
                {
                    Success = false,
                    Message = "Destination bank account not found."
                };
            }

            // =========================================================
            // DESTINATION ACCOUNT VALIDATION
            // =========================================================

            if (!toAccount.IsActive)
            {
                return new PaymentResultDto
                {
                    Success = false,
                    Message = "Destination bank account is inactive."
                };
            }

            // =========================================================
            // SAME ACCOUNT CHECK
            // =========================================================

            if (fromAccount.AccountNumber ==
                toAccount.AccountNumber)
            {
                return new PaymentResultDto
                {
                    Success = false,
                    Message =
                        "You cannot transfer money to the same account."
                };
            }

            // =========================================================
            // AMOUNT VALIDATION
            // =========================================================

            if (paymentDto.Amount <= 0)
            {
                return new PaymentResultDto
                {
                    Success = false,
                    Message = "Amount must be greater than zero."
                };
            }

            // =========================================================
            // BALANCE VALIDATION
            // =========================================================

            if (paymentDto.Amount > fromAccount.Balance)
            {
                return new PaymentResultDto
                {
                    Success = false,
                    Message = "Insufficient balance."
                };
            }

            // =========================================================
            // DATABASE TRANSACTION
            // =========================================================

            using var dbTransaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Debit source account
                fromAccount.Balance -= paymentDto.Amount;

                // Credit destination account
                toAccount.Balance += paymentDto.Amount;

                // Create transaction record
                var transaction = new Transaction
                {
                    FromBankAccountId =
                        fromAccount.BankAccountId,

                    ToAccountNumber =
                        toAccount.AccountNumber,

                    Amount =
                        paymentDto.Amount,

                    TransactionType =
                        paymentDto.TransactionType,

                    TransactionDate =
                        DateTime.Now,

                    Status = "Success"
                };

                _paymentRepository.AddTransaction(transaction);

                _paymentRepository.SaveChanges();

                await dbTransaction.CommitAsync();

                // =====================================================
                // SUCCESS RESPONSE
                // =====================================================

                return new PaymentResultDto
                {
                    Success = true,

                    Message =
                        "Payment completed successfully.",

                    TransactionId =
                        transaction.TransactionId,

                    FromAccountNumber =
                        fromAccount.AccountNumber,

                    ToAccountNumber =
                        toAccount.AccountNumber,

                    Amount =
                        paymentDto.Amount,

                    RemainingBalance =
                        fromAccount.Balance,

                    TransactionDate =
                        transaction.TransactionDate
                };
            }
            catch
            {
                await dbTransaction.RollbackAsync();

                return new PaymentResultDto
                {
                    Success = false,
                    Message =
                        "Payment failed. No money was transferred."
                };
            }
        }
    }
}