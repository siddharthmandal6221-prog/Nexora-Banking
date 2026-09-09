using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository
            _subscriptionRepository;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository =
                subscriptionRepository;
        }

        public Subscription GetSubscriptionById(
            int subscriptionId)
        {
            return _subscriptionRepository
                .GetSubscriptionById(subscriptionId);
        }

        public List<Subscription> GetAllSubscriptions()
        {
            return _subscriptionRepository
                .GetAllSubscriptions();
        }

        public List<Subscription>
            GetSubscriptionsByCustomerId(int customerId)
        {
            return _subscriptionRepository
                .GetSubscriptionsByCustomerId(customerId);
        }

        public List<Subscription>
            GetSubscriptionsByBankAccountId(int bankAccountId)
        {
            return _subscriptionRepository
                .GetSubscriptionsByBankAccountId(bankAccountId);
        }

        public void AddSubscription(
            Subscription subscription)
        {
            _subscriptionRepository
                .AddSubscription(subscription);
        }

        public void UpdateSubscription(
            Subscription subscription)
        {
            _subscriptionRepository
                .UpdateSubscription(subscription);
        }

        public void DeleteSubscription(
            Subscription subscription)
        {
            _subscriptionRepository
                .DeleteSubscription(subscription);
        }
    }
}