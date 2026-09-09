using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Repositories.Interfaces
{
    public interface IBeneficiaryRepository
    {
        Beneficiary GetBeneficiaryById(int beneficiaryId);

        List<Beneficiary> GetAllBeneficiaries();

        List<Beneficiary> GetBeneficiariesByCustomerId(int customerId);

        void AddBeneficiary(Beneficiary beneficiary);

        void UpdateBeneficiary(Beneficiary beneficiary);

        void DeleteBeneficiary(Beneficiary beneficiary);
    }
}