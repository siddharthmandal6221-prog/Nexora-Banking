using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;

        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public Card GetCardById(int cardId)
        {
            return _cardRepository.GetCardById(cardId);
        }

        public List<Card> GetAllCards()
        {
            return _cardRepository.GetAllCards();
        }

        public List<Card> GetCardsByBankAccountId(int bankAccountId)
        {
            return _cardRepository.GetCardsByBankAccountId(bankAccountId);
        }

        public void AddCard(Card card)
        {
            _cardRepository.AddCard(card);
        }

        public void UpdateCard(Card card)
        {
            _cardRepository.UpdateCard(card);
        }

        public void DeleteCard(Card card)
        {
            _cardRepository.DeleteCard(card);
        }
    }
}