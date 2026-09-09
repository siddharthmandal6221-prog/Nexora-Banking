using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories.Interfaces;

namespace OnlineBankingApplication.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Customer GetCustomerById(int customerId)
        {
            return _context.Customers
                .FirstOrDefault(c => c.CustomerId == customerId);
        }

        public Customer GetCustomerByUserId(string userId)
        {
            return _context.Customers
                .FirstOrDefault(c => c.ApplicationUserId == userId);
        }

        public List<Customer> GetAllCustomers()
        {
            return _context.Customers
                .ToList();
        }

        public void AddCustomer(Customer customer)
        {
            if (customer.CreatedDate == default || customer.CreatedDate.Year < 2000)
            {
                customer.CreatedDate = DateTime.Now;
            }
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        public void UpdateCustomer(Customer customer)
        {
            var existingCustomer = _context.Customers
                .FirstOrDefault(c => c.CustomerId == customer.CustomerId);

            if (existingCustomer == null)
            {
                return;
            }

            _context.Entry(existingCustomer)
                .CurrentValues
                .SetValues(customer);

            _context.SaveChanges();
        }

        public void DeleteCustomer(Customer customer)
        {
            var existingCustomer = _context.Customers
                .FirstOrDefault(c => c.CustomerId == customer.CustomerId);

            if (existingCustomer == null)
            {
                return;
            }

            _context.Customers.Remove(existingCustomer);
            _context.SaveChanges();
        }
    }
}