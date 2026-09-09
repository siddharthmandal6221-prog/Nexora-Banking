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
    public class BeneficiaryController : Controller
    {
        private readonly IBeneficiaryService _beneficiaryService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public BeneficiaryController(
            IBeneficiaryService beneficiaryService,
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _beneficiaryService = beneficiaryService;
            _customerService = customerService;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: /Beneficiary
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                var allBeneficiaries =
                    _beneficiaryService.GetAllBeneficiaries();

                var beneficiaryDtos =
                    _mapper.Map<List<BeneficiaryDto>>(allBeneficiaries);

                return View(beneficiaryDtos);
            }

            var customer = _customerService
                .GetAllCustomers()
                .FirstOrDefault(c =>
                    c.ApplicationUserId == user.Id);

            if (customer == null)
            {
                return NotFound();
            }

            var beneficiaries =
                _beneficiaryService
                    .GetBeneficiariesByCustomerId(customer.CustomerId);

            var beneficiaryDtoList =
                _mapper.Map<List<BeneficiaryDto>>(beneficiaries);

            return View(beneficiaryDtoList);
        }

        // GET: /Beneficiary/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var beneficiary =
                _beneficiaryService.GetBeneficiaryById(id);

            if (beneficiary == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                var customer = _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == user.Id);

                if (customer == null ||
                    beneficiary.CustomerId != customer.CustomerId)
                {
                    return NotFound();
                }
            }

            var beneficiaryDto =
                _mapper.Map<BeneficiaryDto>(beneficiary);

            return View(beneficiaryDto);
        }

        // GET: /Beneficiary/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                ViewBag.Customers =
                    _customerService.GetAllCustomers();

                return View();
            }

            var customer = _customerService
                .GetAllCustomers()
                .FirstOrDefault(c =>
                    c.ApplicationUserId == user.Id);

            if (customer == null)
            {
                return NotFound();
            }

            ViewBag.Customers =
                new List<Customer> { customer };

            return View();
        }

        // POST: /Beneficiary/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            BeneficiaryDto beneficiaryDto)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                var customer = _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == user.Id);

                if (customer == null)
                {
                    return NotFound();
                }

                // Never trust CustomerId from browser
                beneficiaryDto.CustomerId =
                    customer.CustomerId;

                ModelState.Remove(
                    nameof(BeneficiaryDto.CustomerId));
            }

            if (ModelState.IsValid)
            {
                var beneficiary =
                    _mapper.Map<Beneficiary>(beneficiaryDto);

                _beneficiaryService
                    .AddBeneficiary(beneficiary);

                return RedirectToAction(nameof(Index));
            }

            if (isAdmin)
            {
                ViewBag.Customers =
                    _customerService.GetAllCustomers();
            }
            else
            {
                var customer = _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == user.Id);

                if (customer != null)
                {
                    ViewBag.Customers =
                        new List<Customer> { customer };
                }
            }

            return View(beneficiaryDto);
        }

        // GET: /Beneficiary/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var beneficiary =
                _beneficiaryService.GetBeneficiaryById(id);

            if (beneficiary == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                var customer = _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == user.Id);

                if (customer == null ||
                    beneficiary.CustomerId != customer.CustomerId)
                {
                    return NotFound();
                }

                ViewBag.Customers =
                    new List<Customer> { customer };
            }
            else
            {
                ViewBag.Customers =
                    _customerService.GetAllCustomers();
            }

            var beneficiaryDto =
                _mapper.Map<BeneficiaryDto>(beneficiary);

            return View(beneficiaryDto);
        }

        // POST: /Beneficiary/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            BeneficiaryDto beneficiaryDto)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            if (id != beneficiaryDto.BeneficiaryId)
            {
                return NotFound();
            }

            var existingBeneficiary =
                _beneficiaryService.GetBeneficiaryById(id);

            if (existingBeneficiary == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                var customer = _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == user.Id);

                if (customer == null ||
                    existingBeneficiary.CustomerId !=
                    customer.CustomerId)
                {
                    return NotFound();
                }

                beneficiaryDto.CustomerId =
                    customer.CustomerId;
            }
            else
            {
                // Preserve existing owner
                beneficiaryDto.CustomerId =
                    existingBeneficiary.CustomerId;
            }

            ModelState.Remove(
                nameof(BeneficiaryDto.CustomerId));

            if (ModelState.IsValid)
            {
                var beneficiary =
                    _mapper.Map<Beneficiary>(beneficiaryDto);

                _beneficiaryService
                    .UpdateBeneficiary(beneficiary);

                return RedirectToAction(nameof(Index));
            }

            if (isAdmin)
            {
                ViewBag.Customers =
                    _customerService.GetAllCustomers();
            }
            else
            {
                var customer = _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == user.Id);

                if (customer != null)
                {
                    ViewBag.Customers =
                        new List<Customer> { customer };
                }
            }

            return View(beneficiaryDto);
        }

        // GET: /Beneficiary/Delete/1
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var beneficiary =
                _beneficiaryService.GetBeneficiaryById(id);

            if (beneficiary == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                var customer = _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == user.Id);

                if (customer == null ||
                    beneficiary.CustomerId !=
                    customer.CustomerId)
                {
                    return NotFound();
                }
            }

            var beneficiaryDto =
                _mapper.Map<BeneficiaryDto>(beneficiary);

            return View(beneficiaryDto);
        }

        // POST: /Beneficiary/Delete/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id,
            bool confirm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new
                {
                    area = "Identity"
                });
            }

            var beneficiary =
                _beneficiaryService.GetBeneficiaryById(id);

            if (beneficiary == null)
            {
                return NotFound();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                var customer = _customerService
                    .GetAllCustomers()
                    .FirstOrDefault(c =>
                        c.ApplicationUserId == user.Id);

                if (customer == null ||
                    beneficiary.CustomerId !=
                    customer.CustomerId)
                {
                    return NotFound();
                }
            }

            _beneficiaryService
                .DeleteBeneficiary(beneficiary);

            return RedirectToAction(nameof(Index));
        }
    }
}