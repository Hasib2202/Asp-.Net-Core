using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectMiniAccountManagementSystem.Models
{
    public class Voucher
    {
        public int VoucherId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Voucher Date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Reference Number is required")]
        [StringLength(50, ErrorMessage = "Reference Number cannot exceed 50 characters")]
        [Display(Name = "Reference No")]
        public string ReferenceNo { get; set; }

        [Required(ErrorMessage = "Voucher Type is required")]
        [Display(Name = "Voucher Type")]
        public string VoucherType { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<VoucherDetail> VoucherDetails { get; set; } = new List<VoucherDetail>();
    }

    public class VoucherDetail
    {
        public int VoucherDetailId { get; set; }

        public int VoucherId { get; set; }

        [Display(Name = "Debit Account")]
        public int? DebitAccountId { get; set; }

        [Display(Name = "Credit Account")]
        public int? CreditAccountId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        public string Description { get; set; }

        // Navigation properties for display purposes
        public string DebitAccountName { get; set; }
        public string CreditAccountName { get; set; }
    }

    public class VoucherEntryViewModel
    {
        public Voucher Voucher { get; set; } = new Voucher();

        public List<AccountSelectListItem> AccountsList { get; set; } = new List<AccountSelectListItem>();
    }

    public class AccountSelectListItem
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }

        public string DisplayText => $"{AccountCode} - {AccountName}";
    }
}