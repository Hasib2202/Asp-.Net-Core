using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProjectMiniAccountManagementSystem.Models;

namespace ProjectMiniAccountManagementSystem.Repositories
{
    public class VoucherRepository
    {
        private readonly string _connectionString;

        public VoucherRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> SaveVoucherAsync(Voucher voucher)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_SaveVoucher", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;


                    // Add VoucherId parameter
                    command.Parameters.AddWithValue("@VoucherId", voucher.VoucherId);

                    // Add parameters for the main voucher
                    command.Parameters.AddWithValue("@Date", voucher.Date);
                    command.Parameters.AddWithValue("@ReferenceNo", voucher.ReferenceNo);
                    command.Parameters.AddWithValue("@VoucherType", voucher.VoucherType);
                    command.Parameters.AddWithValue("@Description", (object)voucher.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedBy", (object)voucher.CreatedBy ?? DBNull.Value);

                    // Create a DataTable for voucher details
                    var detailsTable = new DataTable();
                    detailsTable.Columns.Add("DebitAccountId", typeof(int));
                    detailsTable.Columns.Add("CreditAccountId", typeof(int));
                    detailsTable.Columns.Add("Amount", typeof(decimal));
                    detailsTable.Columns.Add("Description", typeof(string));

                    // Add rows to the details table
                    foreach (var detail in voucher.VoucherDetails)
                    {
                        var row = detailsTable.NewRow();
                        row["DebitAccountId"] = detail.DebitAccountId.HasValue ? (object)detail.DebitAccountId.Value : DBNull.Value;
                        row["CreditAccountId"] = detail.CreditAccountId.HasValue ? (object)detail.CreditAccountId.Value : DBNull.Value;
                        row["Amount"] = detail.Amount;
                        row["Description"] = detail.Description ?? (object)DBNull.Value;
                        detailsTable.Rows.Add(row);
                    }

                    // Add the table parameter
                    var detailsParam = command.Parameters.AddWithValue("@VoucherDetails", detailsTable);
                    detailsParam.SqlDbType = SqlDbType.Structured;
                    detailsParam.TypeName = "VoucherDetailsTableType";

                    // Execute the command and get the new ID
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.GetInt32(0); // Return the new voucher ID
                        }
                    }
                }
            }

            return 0; // If we reach here, something went wrong
        }

        public async Task<List<AccountSelectListItem>> GetAccountsListAsync()
        {
            var accounts = new List<AccountSelectListItem>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(@"
                    SELECT AccountId, AccountName 
                    FROM Accounts 
                    ORDER BY AccountId", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            accounts.Add(new AccountSelectListItem
                            {
                                AccountId = reader.GetInt32(0),
                                AccountName = reader.GetString(1),
                            });
                        }
                    }
                }
            }

            return accounts;
        }

        public async Task<Voucher> GetVoucherByIdAsync(int voucherId)
        {
            Voucher voucher = null;
            var voucherDetails = new List<VoucherDetail>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Get the main voucher
                using (var command = new SqlCommand(@"
                    SELECT VoucherId, Date, ReferenceNo, VoucherType, Description, CreatedBy, CreatedDate
                    FROM Vouchers
                    WHERE VoucherId = @VoucherId", connection))
                {
                    command.Parameters.AddWithValue("@VoucherId", voucherId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            voucher = new Voucher
                            {
                                VoucherId = reader.GetInt32(0),
                                Date = reader.GetDateTime(1),
                                ReferenceNo = reader.GetString(2),
                                VoucherType = reader.GetString(3),
                                Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CreatedBy = reader.IsDBNull(5) ? null : reader.GetString(5),
                                CreatedDate = reader.GetDateTime(6)
                            };
                        }
                    }
                }

                if (voucher != null)
                {
                    // Get the voucher details
                    using (var command = new SqlCommand(@"
                        SELECT vd.VoucherDetailId, vd.DebitAccountId, vd.CreditAccountId, vd.Amount, vd.Description,
                               da.AccountName AS DebitAccountName, ca.AccountName AS CreditAccountName
                        FROM VoucherDetails vd
                        LEFT JOIN Accounts da ON vd.DebitAccountId = da.AccountId
                        LEFT JOIN Accounts ca ON vd.CreditAccountId = ca.AccountId
                        WHERE vd.VoucherId = @VoucherId", connection))
                    {
                        command.Parameters.AddWithValue("@VoucherId", voucherId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                voucherDetails.Add(new VoucherDetail
                                {
                                    VoucherDetailId = reader.GetInt32(0),
                                    VoucherId = voucherId,
                                    DebitAccountId = reader.IsDBNull(1) ? null : (int?)reader.GetInt32(1),
                                    CreditAccountId = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2),
                                    Amount = reader.GetDecimal(3),
                                    Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    DebitAccountName = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    CreditAccountName = reader.IsDBNull(6) ? null : reader.GetString(6)
                                });
                            }
                        }
                    }

                    voucher.VoucherDetails = voucherDetails;
                }
            }

            return voucher;
        }

        public async Task<List<Voucher>> GetVouchersAsync()
        {
            var vouchers = new List<Voucher>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(@"
                    SELECT VoucherId, Date, ReferenceNo, VoucherType, Description
                    FROM Vouchers
                    ORDER BY Date DESC", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            vouchers.Add(new Voucher
                            {
                                VoucherId = reader.GetInt32(0),
                                Date = reader.GetDateTime(1),
                                ReferenceNo = reader.GetString(2),
                                VoucherType = reader.GetString(3),
                                Description = reader.IsDBNull(4) ? null : reader.GetString(4)
                            });
                        }
                    }
                }
            }

            return vouchers;
        }
    }
}
