using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingApplication.DTOs;
using OnlineBankingApplication.Models;
using OnlineBankingApplication.Services;

using ApplicationUser = OnlineBankingApplication.Models.ApplicationUser;

namespace OnlineBankingApplication.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IBankAccountService _bankAccountService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public SubscriptionController(
            ISubscriptionService subscriptionService,
            IBankAccountService bankAccountService,
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _subscriptionService = subscriptionService;
            _bankAccountService = bankAccountService;
            _customerService = customerService;
            _userManager = userManager;
            _mapper = mapper;
        }

        // ==========================================
        // INDEX
        // ==========================================

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });
            }

            // Admin can see all subscriptions
            if (User.IsInRole("Admin"))
            {
                var allSubscriptions =
                    _subscriptionService.GetAllSubscriptions();

                var subscriptionDtos =
                    _mapper.Map<List<SubscriptionDto>>(
                        allSubscriptions);

                return View(subscriptionDtos);
            }

            // Customer can see only their subscriptions
            var customer = _customerService
                .GetAllCustomers()
                .FirstOrDefault(c =>
                    c.ApplicationUserId == user.Id);

            if (customer == null)
            {
                return View(new List<SubscriptionDto>());
            }

            var subscriptions =
                _subscriptionService
                    .GetSubscriptionsByCustomerId(
                        customer.CustomerId);

            var subscriptionDtoList =
                _mapper.Map<List<SubscriptionDto>>(
                    subscriptions);

            return View(subscriptionDtoList);
        }


        // ==========================================
        // DETAILS
        // ==========================================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription =
                _subscriptionService
                    .GetSubscriptionById(id.Value);

            if (subscription == null)
            {
                return NotFound();
            }

            // Admin can view any subscription
            if (User.IsInRole("Admin"))
            {
                var subscriptionDto =
                    _mapper.Map<SubscriptionDto>(
                        subscription);

                return View(subscriptionDto);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });
            }

            var customer = _customerService
                .GetAllCustomers()
                .FirstOrDefault(c =>
                    c.ApplicationUserId == user.Id);

            if (customer == null ||
                subscription.CustomerId != customer.CustomerId)
            {
                return Forbid();
            }

            var customerSubscriptionDto =
                _mapper.Map<SubscriptionDto>(
                    subscription);

            return View(customerSubscriptionDto);
        }


        // ==========================================
        // CREATE - GET
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.BankAccounts =
                _bankAccountService.GetAllBankAccounts();

            ViewBag.Customers =
                _customerService.GetAllCustomers();

            return View();
        }


        // ==========================================
        // CREATE - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(
            SubscriptionDto subscriptionDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.BankAccounts =
                    _bankAccountService.GetAllBankAccounts();

                ViewBag.Customers =
                    _customerService.GetAllCustomers();

                return View(subscriptionDto);
            }

            var account =
                _bankAccountService
                    .GetBankAccountById(
                        subscriptionDto.BankAccountId);

            if (account == null)
            {
                ModelState.AddModelError(
                    nameof(subscriptionDto.BankAccountId),
                    "Selected bank account does not exist.");

                ViewBag.BankAccounts =
                    _bankAccountService.GetAllBankAccounts();

                ViewBag.Customers =
                    _customerService.GetAllCustomers();

                return View(subscriptionDto);
            }

            if (!account.IsActive)
            {
                ModelState.AddModelError(
                    nameof(subscriptionDto.BankAccountId),
                    "The selected bank account is inactive.");

                ViewBag.BankAccounts =
                    _bankAccountService.GetAllBankAccounts();

                ViewBag.Customers =
                    _customerService.GetAllCustomers();

                return View(subscriptionDto);
            }

            if (subscriptionDto.CustomerId <= 0)
            {
                ModelState.AddModelError(
                    nameof(subscriptionDto.CustomerId),
                    "Customer is required.");

                ViewBag.BankAccounts =
                    _bankAccountService.GetAllBankAccounts();

                ViewBag.Customers =
                    _customerService.GetAllCustomers();

                return View(subscriptionDto);
            }

            if (subscriptionDto.NextPaymentDate == default)
            {
                subscriptionDto.NextPaymentDate =
                    DateTime.Now.AddMonths(1);
            }

            subscriptionDto.CreatedDate =
                DateTime.Now;

            var subscription =
                _mapper.Map<Subscription>(
                    subscriptionDto);

            _subscriptionService
                .AddSubscription(subscription);

            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // EDIT - GET
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription =
                _subscriptionService
                    .GetSubscriptionById(id.Value);

            if (subscription == null)
            {
                return NotFound();
            }

            var subscriptionDto =
                _mapper.Map<SubscriptionDto>(
                    subscription);

            return View(subscriptionDto);
        }


        // ==========================================
        // EDIT - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(
            int id,
            SubscriptionDto subscriptionDto)
        {
            if (id != subscriptionDto.SubscriptionId)
            {
                return NotFound();
            }

            var existingSubscription =
                _subscriptionService
                    .GetSubscriptionById(id);

            if (existingSubscription == null)
            {
                return NotFound();
            }

            // Do not allow changing ownership/account
            subscriptionDto.CustomerId =
                existingSubscription.CustomerId;

            subscriptionDto.BankAccountId =
                existingSubscription.BankAccountId;

            subscriptionDto.CreatedDate =
                existingSubscription.CreatedDate;

            ModelState.Remove(
                nameof(SubscriptionDto.CustomerId));

            ModelState.Remove(
                nameof(SubscriptionDto.BankAccountId));

            ModelState.Remove(
                nameof(SubscriptionDto.CreatedDate));

            if (!ModelState.IsValid)
            {
                return View(subscriptionDto);
            }

            var subscription =
                _mapper.Map<Subscription>(
                    subscriptionDto);

            _subscriptionService
                .UpdateSubscription(subscription);

            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // DELETE - GET
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription =
                _subscriptionService
                    .GetSubscriptionById(id.Value);

            if (subscription == null)
            {
                return NotFound();
            }

            var subscriptionDto =
                _mapper.Map<SubscriptionDto>(
                    subscription);

            return View(subscriptionDto);
        }


        // ==========================================
        // DELETE - POST
        // ==========================================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var subscription =
                _subscriptionService
                    .GetSubscriptionById(id);

            if (subscription == null)
            {
                return NotFound();
            }

            _subscriptionService
                .DeleteSubscription(subscription);

            return RedirectToAction(nameof(Index));
        }
    }
}