using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Repositories
{
    public class ChequeBookRequestRepository : IChequeBookRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public ChequeBookRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ChequeBookRequest GetChequeBookRequestById(int chequeBookRequestId)
        {
            return _context.ChequeBookRequests
                .Include(c => c.BankAccount)
                .FirstOrDefault(c =>
                    c.ChequeBookRequestId == chequeBookRequestId);
        }

        public List<ChequeBookRequest> GetAllChequeBookRequests()
        {
            return _context.ChequeBookRequests
                .Include(c => c.BankAccount)
                .OrderByDescending(c => c.RequestDate)
                .ToList();
        }

        public List<ChequeBookRequest> GetChequeBookRequestsByBankAccountId(
            int bankAccountId)
        {
            return _context.ChequeBookRequests
                .Where(c => c.BankAccountId == bankAccountId)
                .OrderByDescending(c => c.RequestDate)
                .ToList();
        }

        public void AddChequeBookRequest(
            ChequeBookRequest chequeBookRequest)
        {
            _context.ChequeBookRequests.Add(chequeBookRequest);
            _context.SaveChanges();
        }

        public void UpdateChequeBookRequest(
            ChequeBookRequest chequeBookRequest)
        {
            _context.ChequeBookRequests.Update(chequeBookRequest);
            _context.SaveChanges();
        }

        public void DeleteChequeBookRequest(
            ChequeBookRequest chequeBookRequest)
        {
            _context.ChequeBookRequests.Remove(chequeBookRequest);
            _context.SaveChanges();
        }
    }
}