using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Services
{
    public class BillPaymentService : IBillPaymentService
    {
        private readonly IBillPaymentRepository _billPaymentRepository;

        public BillPaymentService(
            IBillPaymentRepository billPaymentRepository)
        {
            _billPaymentRepository = billPaymentRepository;
        }

        public BillPayment GetBillPaymentById(int billPaymentId)
        {
            return _billPaymentRepository.GetBillPaymentById(billPaymentId);
        }

        public List<BillPayment> GetAllBillPayments()
        {
            return _billPaymentRepository.GetAllBillPayments();
        }

        public List<BillPayment> GetBillPaymentsByBankAccountId(int bankAccountId)
        {
            return _billPaymentRepository
                .GetBillPaymentsByBankAccountId(bankAccountId);
        }

        public void AddBillPayment(BillPayment billPayment)
        {
            _billPaymentRepository.AddBillPayment(billPayment);
        }
    }
}