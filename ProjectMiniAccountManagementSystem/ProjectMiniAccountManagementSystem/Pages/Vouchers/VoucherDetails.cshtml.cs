using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectMiniAccountManagementSystem.Models;
using ProjectMiniAccountManagementSystem.Repositories;

namespace ProjectMiniAccountManagementSystem.Pages.Vouchers
{
    [Authorize]
    public class VoucherDetailModel : PageModel
    {
        private readonly VoucherRepository _voucherRepository;

        public VoucherDetailModel(VoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public Voucher Voucher { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Voucher = await _voucherRepository.GetVoucherByIdAsync(id);

            if (Voucher == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}