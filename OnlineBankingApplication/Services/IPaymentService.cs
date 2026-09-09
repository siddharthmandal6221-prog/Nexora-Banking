using OnlineBankingApplication.DTOs;

namespace OnlineBankingApplication.Services
{
    public interface IPaymentService
    {
        Task<PaymentResultDto> MakePaymentAsync(
            PaymentDto paymentDto,
            string userId);
    }
}