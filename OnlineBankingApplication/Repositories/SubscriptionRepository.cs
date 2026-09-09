using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Subscription GetSubscriptionById(int subscriptionId)
        {
            return _context.Subscriptions
                .Include(s => s.Customer)
                .Include(s => s.BankAccount)
                .FirstOrDefault(s =>
                    s.SubscriptionId == subscriptionId);
        }

        public List<Subscription> GetAllSubscriptions()
        {
            return _context.Subscriptions
                .Include(s => s.Customer)
                .Include(s => s.BankAccount)
                .OrderBy(s => s.NextPaymentDate)
                .ToList();
        }

        public List<Subscription> GetSubscriptionsByCustomerId(
            int customerId)
        {
            return _context.Subscriptions
                .Where(s => s.CustomerId == customerId)
                .OrderBy(s => s.NextPaymentDate)
                .ToList();
        }

        public List<Subscription> GetSubscriptionsByBankAccountId(
            int bankAccountId)
        {
            return _context.Subscriptions
                .Where(s => s.BankAccountId == bankAccountId)
                .OrderBy(s => s.NextPaymentDate)
                .ToList();
        }

        public void AddSubscription(
            Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            _context.SaveChanges();
        }

        public void UpdateSubscription(
            Subscription subscription)
        {
            var existingSubscription =
                _context.Subscriptions
                    .FirstOrDefault(s =>
                        s.SubscriptionId ==
                        subscription.SubscriptionId);

            if (existingSubscription == null)
            {
                return;
            }

            _context.Entry(existingSubscription)
                .CurrentValues
                .SetValues(subscription);

            _context.SaveChanges();
        }

        public void DeleteSubscription(
            Subscription subscription)
        {
            _context.Subscriptions.Remove(subscription);
            _context.SaveChanges();
        }
    }
}