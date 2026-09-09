using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Repositories
{
    public class BeneficiaryRepository : IBeneficiaryRepository
    {
        private readonly ApplicationDbContext _context;

        public BeneficiaryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Beneficiary GetBeneficiaryById(int beneficiaryId)
        {
            return _context.Beneficiaries
                .Include(b => b.Customer)
                .FirstOrDefault(b => b.BeneficiaryId == beneficiaryId);
        }

        public List<Beneficiary> GetAllBeneficiaries()
        {
            return _context.Beneficiaries
                .Include(b => b.Customer)
                .ToList();
        }

        public List<Beneficiary> GetBeneficiariesByCustomerId(int customerId)
        {
            return _context.Beneficiaries
                .Where(b => b.CustomerId == customerId)
                .ToList();
        }

        public void AddBeneficiary(Beneficiary beneficiary)
        {
            _context.Beneficiaries.Add(beneficiary);
            _context.SaveChanges();
        }

        public void UpdateBeneficiary(Beneficiary beneficiary)
        {
            var existingBeneficiary = _context.Beneficiaries
                .FirstOrDefault(b =>
                    b.BeneficiaryId == beneficiary.BeneficiaryId);

            if (existingBeneficiary == null)
            {
                return;
            }

            _context.Entry(existingBeneficiary)
                .CurrentValues
                .SetValues(beneficiary);

            _context.SaveChanges();
        }

        public void DeleteBeneficiary(Beneficiary beneficiary)
        {
            var existingBeneficiary = _context.Beneficiaries
                .FirstOrDefault(b =>
                    b.BeneficiaryId == beneficiary.BeneficiaryId);

            if (existingBeneficiary == null)
            {
                return;
            }

            _context.Beneficiaries.Remove(existingBeneficiary);
            _context.SaveChanges();
        }
    }
}