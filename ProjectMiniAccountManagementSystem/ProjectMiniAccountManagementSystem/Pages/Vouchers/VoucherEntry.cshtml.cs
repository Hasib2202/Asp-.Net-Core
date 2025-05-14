using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using ProjectMiniAccountManagementSystem.Models;
using ProjectMiniAccountManagementSystem.Repositories;

namespace ProjectMiniAccountManagementSystem.Pages.Vouchers
{
    [Authorize]
    public class VoucherEntryModel : PageModel
    {
        private readonly VoucherRepository _voucherRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<VoucherEntryModel> _logger;

        public VoucherEntryModel(VoucherRepository voucherRepository, UserManager<IdentityUser> userManager, ILogger<VoucherEntryModel> logger)
        {
            _voucherRepository = voucherRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public VoucherEntryViewModel VoucherVM { get; set; } = new VoucherEntryViewModel();

        public SelectList VoucherTypes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            try
            {

                // Always initialize the SelectList and accounts list first
                VoucherTypes = new SelectList(new[] { "Journal", "Payment", "Receipt" });
                VoucherVM.AccountsList = await _voucherRepository.GetAccountsListAsync();

                if (id.HasValue)
                {
                    // Edit existing voucher
                    var voucher = await _voucherRepository.GetVoucherByIdAsync(id.Value);
                    if (voucher == null)
                    {
                        return NotFound();
                    }

                    VoucherVM.Voucher = voucher;

                    // Set the selected value in the drop-down
                    VoucherTypes = new SelectList(new[] { "Journal", "Payment", "Receipt" }, voucher.VoucherType);
                }
                else
                {
                    // New voucher
                    VoucherVM.Voucher = new Voucher
                    {
                        Date = DateTime.Today,
                        VoucherType = "Journal",
                        VoucherDetails = new List<VoucherDetail>
                        {
                            new VoucherDetail() { Amount = 0 }
                        }
                    };

                    // Default selection
                    VoucherTypes = new SelectList(new[] { "Journal", "Payment", "Receipt" }, "Journal");
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading voucher entry page");
                StatusMessage = "Error loading page: " + ex.Message;
                return RedirectToPage("VoucherList");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Always initialize these first regardless of validation state
                VoucherVM.AccountsList = await _voucherRepository.GetAccountsListAsync();

                // Set the SelectList with the posted value to maintain selection
                string selectedType = VoucherVM.Voucher?.VoucherType ?? "Journal";
                VoucherTypes = new SelectList(new[] { "Journal", "Payment", "Receipt" }, selectedType);

                _logger.LogInformation($"POST received - VoucherType: {selectedType}");
                _logger.LogInformation($"POST data - VoucherDetails count: {VoucherVM.Voucher?.VoucherDetails?.Count ?? 0}");

                // Check overall model state
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid at initial validation");

                    // Log specific model errors for troubleshooting
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogWarning($"Model error for {state.Key}: {error.ErrorMessage}");
                        }
                    }
                }

                // Ensure voucher details is initialized
                if (VoucherVM.Voucher.VoucherDetails == null)
                {
                    VoucherVM.Voucher.VoucherDetails = new List<VoucherDetail>();
                    ModelState.AddModelError("", "No voucher details received.");
                    return Page();
                }

                // Log all received details for debugging
                foreach (var detail in VoucherVM.Voucher.VoucherDetails)
                {
                    _logger.LogInformation($"Raw detail: DebitAccount={detail.DebitAccountId}, " +
                        $"CreditAccount={detail.CreditAccountId}, Amount={detail.Amount}");
                }

                // Remove any null details or empty rows
                VoucherVM.Voucher.VoucherDetails = VoucherVM.Voucher.VoucherDetails
                    .Where(d => d != null)
                    .ToList();

                // Validate each detail individually
                foreach (var detail in VoucherVM.Voucher.VoucherDetails.ToList())
                {
                    // Validate amount
                    if (detail.Amount <= 0)
                    {
                        ModelState.AddModelError("", "Amount must be greater than 0 for all entries.");
                        return Page();
                    }

                    // Validate debit/credit setup
                    if (detail.DebitAccountId.HasValue && detail.CreditAccountId.HasValue)
                    {
                        ModelState.AddModelError("", "Each line must have either a debit account or a credit account, but not both.");
                        return Page();
                    }

                    if (!detail.DebitAccountId.HasValue && !detail.CreditAccountId.HasValue)
                    {
                        ModelState.AddModelError("", "Each line must have either a debit account or a credit account specified.");
                        return Page();
                    }
                }

                // Calculate debit and credit totals
                decimal totalDebit = VoucherVM.Voucher.VoucherDetails
                    .Where(x => x.DebitAccountId.HasValue)
                    .Sum(x => x.Amount);

                decimal totalCredit = VoucherVM.Voucher.VoucherDetails
                    .Where(x => x.CreditAccountId.HasValue)
                    .Sum(x => x.Amount);

                _logger.LogInformation($"Calculated totals - Debit: {totalDebit:N2}, Credit: {totalCredit:N2}");

                // Check balance with rounding to handle potential floating point precision issues
                if (Math.Round(totalDebit, 2) != Math.Round(totalCredit, 2))
                {
                    _logger.LogWarning($"Balance mismatch: Debit = {totalDebit:N2}, Credit = {totalCredit:N2}");
                    ModelState.AddModelError("", $"Total Debit ({totalDebit:N2}) must equal Total Credit ({totalCredit:N2}).");
                    return Page();
                }

                // Set creator information
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    VoucherVM.Voucher.CreatedBy = user.Id;
                }

                // Save the voucher if validation passed
                int voucherId = await _voucherRepository.SaveVoucherAsync(VoucherVM.Voucher);

                if (voucherId > 0)
                {
                    _logger.LogInformation($"Voucher saved successfully with ID: {voucherId}");
                    StatusMessage = "Voucher saved successfully.";
                    return RedirectToPage("VoucherList");
                }
                else
                {
                    _logger.LogWarning("Failed to save voucher - repository returned error");
                    ModelState.AddModelError("", "Failed to save voucher. Please check your data and try again.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while processing voucher");
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return Page();
            }
        }


    }
}