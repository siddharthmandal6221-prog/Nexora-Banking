using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Services
{
    public interface ISubscriptionService
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