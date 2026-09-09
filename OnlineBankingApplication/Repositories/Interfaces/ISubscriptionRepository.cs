using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Repositories.Interfaces
{
    public interface ISubscriptionRepository
    {
        Subscription GetSubscriptionById(int subscriptionId);

        List<Subscription> GetAllSubscriptions();

        List<Subscription> GetSubscriptionsByCustomerId(int customerId);

        List<Subscription> GetSubscriptionsByBankAccountId(int bankAccountId);

        void AddSubscription(Subscription subscription);

        void UpdateSubscription(Subscription subscription);

        void DeleteSubscription(Subscription subscription);
    }
}