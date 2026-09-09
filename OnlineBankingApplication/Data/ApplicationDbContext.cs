using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<OnlineBankingApplication.Models.ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==========================================
        // BANKING TABLES
        // ==========================================

        public DbSet<Customer> Customers { get; set; }

        public DbSet<BankAccount> BankAccounts { get; set; }

        public DbSet<Beneficiary> Beneficiaries { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<BillPayment> BillPayments { get; set; }

        public DbSet<ChequeBookRequest> ChequeBookRequests { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }


        // ==========================================
        // CUSTOMER DASHBOARD TABLES
        // ==========================================

        public DbSet<Card> Cards { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }


        // ==========================================
        // DATABASE RELATIONSHIPS
        // ==========================================

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // ==========================================
            // SUBSCRIPTION → CUSTOMER
            // ==========================================
            //
            // Prevent multiple cascade paths in SQL Server.
            //
            // Customer
            //    ↓
            // Subscription
            //
            // Customer
            //    ↓
            // BankAccount
            //    ↓
            // Subscription
            //
            // Using NoAction on the direct Customer
            // relationship removes the second cascade path.

            builder.Entity<Subscription>()
                .HasOne(s => s.Customer)
                .WithMany()
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);


            // ==========================================
            // SUBSCRIPTION → BANK ACCOUNT
            // ==========================================

            builder.Entity<Subscription>()
                .HasOne(s => s.BankAccount)
                .WithMany()
                .HasForeignKey(s => s.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade);


            // ==========================================
            // CARD → BANK ACCOUNT
            // ==========================================

            builder.Entity<Card>()
                .HasOne(c => c.BankAccount)
                .WithMany()
                .HasForeignKey(c => c.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}