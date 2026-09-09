using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Services
{
    public interface IBeneficiaryService
    {
        Beneficiary GetBeneficiaryById(int beneficiaryId);

        List<Beneficiary> GetAllBeneficiaries();

        List<Beneficiary> GetBeneficiariesByCustomerId(int customerId);

        void AddBeneficiary(Beneficiary beneficiary);

        void UpdateBeneficiary(Beneficiary beneficiary);

        void DeleteBeneficiary(Beneficiary beneficiary);
    }
}