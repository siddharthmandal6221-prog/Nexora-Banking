using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Customer GetCustomerById(int customerId);

        Customer GetCustomerByUserId(string userId);

        List<Customer> GetAllCustomers();

        void AddCustomer(Customer customer);

        void UpdateCustomer(Customer customer);

        void DeleteCustomer(Customer customer);
    }
}