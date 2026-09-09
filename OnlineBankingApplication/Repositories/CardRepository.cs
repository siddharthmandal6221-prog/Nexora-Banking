using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly ApplicationDbContext _context;

        public CardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Card GetCardById(int cardId)
        {
            return _context.Cards
                .Include(c => c.BankAccount)
                .ThenInclude(b => b.Customer)
                .FirstOrDefault(c => c.CardId == cardId);
        }

        public List<Card> GetAllCards()
        {
            return _context.Cards
                .Include(c => c.BankAccount)
                .ThenInclude(b => b.Customer)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }

        public List<Card> GetCardsByBankAccountId(int bankAccountId)
        {
            return _context.Cards
                .Where(c => c.BankAccountId == bankAccountId)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }

        public void AddCard(Card card)
        {
            _context.Cards.Add(card);
            _context.SaveChanges();
        }

        public void UpdateCard(Card card)
        {
            var existingCard = _context.Cards
                .FirstOrDefault(c => c.CardId == card.CardId);

            if (existingCard == null)
            {
                return;
            }

            _context.Entry(existingCard)
                .CurrentValues
                .SetValues(card);

            _context.SaveChanges();
        }

        public void DeleteCard(Card card)
        {
            _context.Cards.Remove(card);
            _context.SaveChanges();
        }
    }
}