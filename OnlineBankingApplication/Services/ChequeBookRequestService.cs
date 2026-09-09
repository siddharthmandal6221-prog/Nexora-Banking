using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Services
{
    public class ChequeBookRequestService : IChequeBookRequestService
    {
        private readonly IChequeBookRequestRepository _chequeBookRequestRepository;

        public ChequeBookRequestService(
            IChequeBookRequestRepository chequeBookRequestRepository)
        {
            _chequeBookRequestRepository = chequeBookRequestRepository;
        }

        public ChequeBookRequest GetChequeBookRequestById(int chequeBookRequestId)
        {
            return _chequeBookRequestRepository
                .GetChequeBookRequestById(chequeBookRequestId);
        }

        public List<ChequeBookRequest> GetAllChequeBookRequests()
        {
            return _chequeBookRequestRepository
                .GetAllChequeBookRequests();
        }

        public List<ChequeBookRequest> GetChequeBookRequestsByBankAccountId(
            int bankAccountId)
        {
            return _chequeBookRequestRepository
                .GetChequeBookRequestsByBankAccountId(bankAccountId);
        }

        public void AddChequeBookRequest(
            ChequeBookRequest chequeBookRequest)
        {
            _chequeBookRequestRepository
                .AddChequeBookRequest(chequeBookRequest);
        }

        public void UpdateChequeBookRequest(
            ChequeBookRequest chequeBookRequest)
        {
            _chequeBookRequestRepository
                .UpdateChequeBookRequest(chequeBookRequest);
        }

        public void DeleteChequeBookRequest(
            ChequeBookRequest chequeBookRequest)
        {
            _chequeBookRequestRepository
                .DeleteChequeBookRequest(chequeBookRequest);
        }
    }
}