using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Repositories
{
    public class BillPaymentRepository : IBillPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public BillPaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public BillPayment GetBillPaymentById(int billPaymentId)
        {
            return _context.BillPayments
                .Include(b => b.BankAccount)
                .FirstOrDefault(b => b.BillPaymentId == billPaymentId);
        }

        public List<BillPayment> GetAllBillPayments()
        {
            return _context.BillPayments
                .Include(b => b.BankAccount)
                .OrderByDescending(b => b.PaymentDate)
                .ToList();
        }

        public List<BillPayment> GetBillPaymentsByBankAccountId(int bankAccountId)
        {
            return _context.BillPayments
                .Where(b => b.BankAccountId == bankAccountId)
                .OrderByDescending(b => b.PaymentDate)
                .ToList();
        }

        public void AddBillPayment(BillPayment billPayment)
        {
            _context.BillPayments.Add(billPayment);
            _context.SaveChanges();
        }
    }
}