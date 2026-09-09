using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Services
{
    public interface ICardService
    {
        Card GetCardById(int cardId);

        List<Card> GetAllCards();

        List<Card> GetCardsByBankAccountId(int bankAccountId);

        void AddCard(Card card);

        void UpdateCard(Card card);

        void DeleteCard(Card card);
    }
}