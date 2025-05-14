using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectMiniAccountManagementSystem.Models;
using ProjectMiniAccountManagementSystem.Repositories;

namespace ProjectMiniAccountManagementSystem.Pages.Vouchers
{
    [Authorize]
    public class VoucherListModel : PageModel
    {
        private readonly VoucherRepository _voucherRepository;

        public VoucherListModel(VoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public List<Voucher> Vouchers { get; set; } = new List<Voucher>();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            Vouchers = await _voucherRepository.GetVouchersAsync();
        }

        public async Task<IActionResult> OnGetExportToExcelAsync()
        {
            var vouchers = await _voucherRepository.GetVouchersAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Vouchers");

                // Add header row
                worksheet.Cell(1, 1).Value = "Voucher ID";
                worksheet.Cell(1, 2).Value = "Date";
                worksheet.Cell(1, 3).Value = "Reference No";
                worksheet.Cell(1, 4).Value = "Voucher Type";
                worksheet.Cell(1, 5).Value = "Description";

                // Style header row
                var headerRange = worksheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Add data rows
                int row = 2;
                foreach (var voucher in vouchers)
                {
                    worksheet.Cell(row, 1).Value = voucher.VoucherId;
                    worksheet.Cell(row, 2).Value = voucher.Date.ToShortDateString();
                    worksheet.Cell(row, 3).Value = voucher.ReferenceNo;
                    worksheet.Cell(row, 4).Value = voucher.VoucherType;
                    worksheet.Cell(row, 5).Value = voucher.Description ?? "";

                    row++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Create a memory stream to save the workbook
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    // Return the Excel file
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Vouchers_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
        }
    }
}