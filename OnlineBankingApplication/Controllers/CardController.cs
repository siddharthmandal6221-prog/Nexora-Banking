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
    public class CardController : Controller
    {
        private readonly ICardService _cardService;
        private readonly IBankAccountService _bankAccountService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CardController(
            ICardService cardService,
            IBankAccountService bankAccountService,
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _cardService = cardService;
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

            // Admin can see all cards
            if (User.IsInRole("Admin"))
            {
                var cards = _cardService.GetAllCards();

                var cardDtos =
                    _mapper.Map<List<CardDto>>(cards);

                return View(cardDtos);
            }

            // Customer can see only their own cards
            var customer = _customerService
                .GetAllCustomers()
                .FirstOrDefault(c =>
                    c.ApplicationUserId == user.Id);

            if (customer == null)
            {
                return View(new List<CardDto>());
            }

            var accounts = _bankAccountService
                .GetBankAccountsByCustomerId(
                    customer.CustomerId);

            var cardsForCustomer = new List<Card>();

            foreach (var account in accounts)
            {
                var accountCards =
                    _cardService.GetCardsByBankAccountId(
                        account.BankAccountId);

                cardsForCustomer.AddRange(accountCards);
            }

            var customerCardDtos =
                _mapper.Map<List<CardDto>>(cardsForCustomer);

            return View(customerCardDtos);
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

            var card =
                _cardService.GetCardById(id.Value);

            if (card == null)
            {
                return NotFound();
            }

            // Admin can view any card
            if (User.IsInRole("Admin"))
            {
                var cardDto =
                    _mapper.Map<CardDto>(card);

                return View(cardDto);
            }

            var user =
                await _userManager.GetUserAsync(User);

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
                card.BankAccount == null ||
                card.BankAccount.CustomerId !=
                    customer.CustomerId)
            {
                return Forbid();
            }

            var customerCardDto =
                _mapper.Map<CardDto>(card);

            return View(customerCardDto);
        }


        // ==========================================
        // CREATE - GET
        // ==========================================

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.BankAccounts =
                _bankAccountService.GetAllBankAccounts();

            return View();
        }


        // ==========================================
        // CREATE - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(CardDto cardDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.BankAccounts =
                    _bankAccountService.GetAllBankAccounts();

                return View(cardDto);
            }

            var account =
                _bankAccountService.GetBankAccountById(
                    cardDto.BankAccountId);

            if (account == null)
            {
                ModelState.AddModelError(
                    nameof(cardDto.BankAccountId),
                    "Selected bank account does not exist.");

                ViewBag.BankAccounts =
                    _bankAccountService.GetAllBankAccounts();

                return View(cardDto);
            }

            if (!account.IsActive)
            {
                ModelState.AddModelError(
                    nameof(cardDto.BankAccountId),
                    "The selected bank account is inactive.");

                ViewBag.BankAccounts =
                    _bankAccountService.GetAllBankAccounts();

                return View(cardDto);
            }

            if (string.IsNullOrWhiteSpace(
                cardDto.CardHolderName))
            {
                cardDto.CardHolderName =
                    account.Customer?.FullName ??
                    "Customer";
            }

            cardDto.CreatedDate = DateTime.Now;

            var card =
                _mapper.Map<Card>(cardDto);

            _cardService.AddCard(card);

            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // EDIT - GET
        // ==========================================

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card =
                _cardService.GetCardById(id.Value);

            if (card == null)
            {
                return NotFound();
            }

            var cardDto =
                _mapper.Map<CardDto>(card);

            return View(cardDto);
        }


        // ==========================================
        // EDIT - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(
            int id,
            CardDto cardDto)
        {
            if (id != cardDto.CardId)
            {
                return NotFound();
            }

            var existingCard =
                _cardService.GetCardById(id);

            if (existingCard == null)
            {
                return NotFound();
            }

            // Keep the original bank account.
            cardDto.BankAccountId =
                existingCard.BankAccountId;

            ModelState.Remove(
                nameof(CardDto.BankAccountId));

            if (!ModelState.IsValid)
            {
                return View(cardDto);
            }

            var card =
                _mapper.Map<Card>(cardDto);

            card.CreatedDate =
                existingCard.CreatedDate;

            _cardService.UpdateCard(card);

            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // DELETE - GET
        // ==========================================

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card =
                _cardService.GetCardById(id.Value);

            if (card == null)
            {
                return NotFound();
            }

            var cardDto =
                _mapper.Map<CardDto>(card);

            return View(cardDto);
        }


        // ==========================================
        // DELETE - POST
        // ==========================================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var card =
                _cardService.GetCardById(id);

            if (card == null)
            {
                return NotFound();
            }

            _cardService.DeleteCard(card);

            return RedirectToAction(nameof(Index));
        }
    }
}