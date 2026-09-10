using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineBankingApplication.AI;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.DTOs;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Repositories;
using OnlineBankingApplication.Repositories.Interfaces;
using OnlineBankingApplication.Services;
using Scalar.AspNetCore;

using ApplicationUser = OnlineBankingApplication.Models.ApplicationUser;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ============================================================
        // Add services to the container
        // ============================================================

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        // OpenAPI
        builder.Services.AddOpenApi();

        // ============================================================
        // Database
        // ============================================================

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? builder.Configuration.GetConnectionString("Azurecon");

        builder.Services.AddDbContext<ApplicationDbContext>(
                   options => options.UseSqlServer(
                   connectionString,
                   sqlOptions =>
                   {
                       sqlOptions.EnableRetryOnFailure(
                          maxRetryCount: 5,
                          maxRetryDelay: TimeSpan.FromSeconds(10),
                          errorNumbersToAdd: null);
                   }));

        // ============================================================
        // ASP.NET Core Identity
        // ============================================================

        builder.Services.AddDefaultIdentity<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // ============================================================
        // Customer
        // ============================================================

        builder.Services.AddScoped<
            ICustomerRepository,
            CustomerRepository>();

        builder.Services.AddScoped<
            ICustomerService,
            CustomerService>();

        // ============================================================
        // Bank Account
        // ============================================================

        builder.Services.AddScoped<
            IBankAccountRepository,
            BankAccountRepository>();

        builder.Services.AddScoped<
            IBankAccountService,
            BankAccountService>();

        // ============================================================
        // Beneficiary
        // ============================================================

        builder.Services.AddScoped<
            IBeneficiaryRepository,
            BeneficiaryRepository>();

        builder.Services.AddScoped<
            IBeneficiaryService,
            BeneficiaryService>();

        // ============================================================
        // Transaction
        // ============================================================

        builder.Services.AddScoped<
            ITransactionRepository,
            TransactionRepository>();

        builder.Services.AddScoped<
            ITransactionService,
            TransactionService>();

        // ============================================================
        // Payment API
        // ============================================================

        builder.Services.AddScoped<
            IPaymentRepository,
            PaymentRepository>();

        builder.Services.AddScoped<
            IPaymentService,
            PaymentService>();

        // ============================================================
        // Bill Payment
        // ============================================================

        builder.Services.AddScoped<
            IBillPaymentRepository,
            BillPaymentRepository>();

        builder.Services.AddScoped<
            IBillPaymentService,
            BillPaymentService>();

        // ============================================================
        // Cheque Book Request
        // ============================================================

        builder.Services.AddScoped<
            IChequeBookRequestRepository,
            ChequeBookRequestRepository>();

        builder.Services.AddScoped<
            IChequeBookRequestService,
            ChequeBookRequestService>();

        // ============================================================
        // Card
        // ============================================================

        builder.Services.AddScoped<
            ICardRepository,
            CardRepository>();

        builder.Services.AddScoped<
            ICardService,
            CardService>();

        // ============================================================
        // Subscription
        // ============================================================

        builder.Services.AddScoped<
            ISubscriptionRepository,
            SubscriptionRepository>();

        builder.Services.AddScoped<
            ISubscriptionService,
            SubscriptionService>();

        // ============================================================
        // AI Banking Assistant
        // ============================================================

        builder.Services.AddScoped<
            IAIService,
            GeminiService>();

        // ============================================================
        // AutoMapper
        // ============================================================

        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        // ============================================================
        // Build Application
        // ============================================================

        var app = builder.Build();

        // ============================================================
        // Initialize database roles and default users
        // ============================================================

        using (var scope = app.Services.CreateScope())
        {
            var roleManager =
                scope.ServiceProvider
                    .GetRequiredService<
                        RoleManager<IdentityRole>>();

            var userManager =
                scope.ServiceProvider
                    .GetRequiredService<
                        UserManager<ApplicationUser>>();

            await DbInitializer.InitializeAsync(
                userManager,
                roleManager);
        }

        // ============================================================
        // Configure HTTP request pipeline
        // ============================================================

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        // Razor Pages
        app.MapRazorPages();

        // Static files
        app.MapStaticAssets();

        // OpenAPI
        app.MapOpenApi();

        // Scalar
        app.MapScalarApiReference();

        // Attribute-routed controllers
        app.MapControllers();

        // Conventional MVC routing
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}