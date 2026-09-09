using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Repositories.Interfaces
{
    public interface IBillPaymentRepository
    {
        BillPayment GetBillPaymentById(int billPaymentId);

        List<BillPayment> GetAllBillPayments();

        List<BillPayment> GetBillPaymentsByBankAccountId(int bankAccountId);

        void AddBillPayment(BillPayment billPayment);
    }
}