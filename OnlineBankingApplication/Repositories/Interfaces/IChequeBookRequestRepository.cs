using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Repositories.Interfaces
{
    public interface IChequeBookRequestRepository
    {
        ChequeBookRequest GetChequeBookRequestById(int chequeBookRequestId);

        List<ChequeBookRequest> GetAllChequeBookRequests();

        List<ChequeBookRequest> GetChequeBookRequestsByBankAccountId(int bankAccountId);

        void AddChequeBookRequest(ChequeBookRequest chequeBookRequest);

        void UpdateChequeBookRequest(ChequeBookRequest chequeBookRequest);

        void DeleteChequeBookRequest(ChequeBookRequest chequeBookRequest);
    }
}