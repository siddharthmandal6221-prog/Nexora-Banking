using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Services
{
    public class BeneficiaryService : IBeneficiaryService
    {
        private readonly IBeneficiaryRepository _beneficiaryRepository;

        public BeneficiaryService(IBeneficiaryRepository beneficiaryRepository)
        {
            _beneficiaryRepository = beneficiaryRepository;
        }

        public Beneficiary GetBeneficiaryById(int beneficiaryId)
        {
            return _beneficiaryRepository.GetBeneficiaryById(beneficiaryId);
        }

        public List<Beneficiary> GetAllBeneficiaries()
        {
            return _beneficiaryRepository.GetAllBeneficiaries();
        }

        public List<Beneficiary> GetBeneficiariesByCustomerId(int customerId)
        {
            return _beneficiaryRepository.GetBeneficiariesByCustomerId(customerId);
        }

        public void AddBeneficiary(Beneficiary beneficiary)
        {
            _beneficiaryRepository.AddBeneficiary(beneficiary);
        }

        public void UpdateBeneficiary(Beneficiary beneficiary)
        {
            _beneficiaryRepository.UpdateBeneficiary(beneficiary);
        }

        public void DeleteBeneficiary(Beneficiary beneficiary)
        {
            _beneficiaryRepository.DeleteBeneficiary(beneficiary);
        }
    }
}