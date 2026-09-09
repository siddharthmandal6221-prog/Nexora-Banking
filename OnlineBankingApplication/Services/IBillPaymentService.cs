using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Services
{
    public interface IBillPaymentService
    {
        BillPayment GetBillPaymentById(int billPaymentId);

        List<BillPayment> GetAllBillPayments();

        List<BillPayment> GetBillPaymentsByBankAccountId(int bankAccountId);

        void AddBillPayment(BillPayment billPayment);
    }
}