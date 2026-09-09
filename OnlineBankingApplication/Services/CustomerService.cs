using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public Customer GetCustomerById(int customerId)
        {
            return _customerRepository.GetCustomerById(customerId);
        }

        public Customer GetCustomerByUserId(string userId)
        {
            return _customerRepository.GetCustomerByUserId(userId);
        }

        public List<Customer> GetAllCustomers()
        {
            return _customerRepository.GetAllCustomers();
        }

        public void AddCustomer(Customer customer)
        {
            _customerRepository.AddCustomer(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            _customerRepository.UpdateCustomer(customer);
        }
        public void DeleteCustomer(Customer customer)
        {
            _customerRepository.DeleteCustomer(customer);
        }
    }
}