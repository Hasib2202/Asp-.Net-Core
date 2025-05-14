using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectMiniAccountManagementSystem.Pages.Accounts
{
//role based access
    [Authorize(Roles = "Admin,Accountant,Viewer")]
    public class ChartOfAccountsModel : PageModel
    {
        private readonly IConfiguration _config;
        public ChartOfAccountsModel(IConfiguration config) => _config = config;

        public class AccountItem
        {
            public int AccountId { get; set; }
            public string AccountName { get; set; }
            public int? ParentAccountId { get; set; }
            public string AccountType { get; set; }
            public bool IsActive { get; set; }
            public List<AccountItem> Children { get; set; } = new();
        }

        public List<AccountItem> Tree { get; set; } = new();
        public List<AccountItem> FlatList { get; set; } = new();
        public bool EditMode { get; set; } = false;

        [BindProperty] public int FormAccountId { get; set; }
        [BindProperty] public string FormAccountName { get; set; }
        [BindProperty] public int? FormParentAccountId { get; set; }
        [BindProperty] public string FormAccountType { get; set; }
        [BindProperty] public bool FormIsActive { get; set; } = true;
        [TempData] public string StatusMessage { get; set; }

        public async Task OnGetAsync(int? id)
        {
            await LoadFlatAsync();
            BuildTree();

            if (id.HasValue)
            {
                EditMode = true;
                var account = FlatList.FirstOrDefault(a => a.AccountId == id.Value);
                if (account != null)
                {
                    FormAccountId = account.AccountId;
                    FormAccountName = account.AccountName;
                    FormParentAccountId = account.ParentAccountId;
                    FormAccountType = account.AccountType;
                    FormIsActive = account.IsActive;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync([FromForm] string Action)
        {
            // Ensure we always have an Action
            var action = Action ?? "Create";

            var cs = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(cs);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_ManageChartOfAccounts", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", action);

            switch (action)
            {
                case "Create":
                    cmd.Parameters.AddWithValue("@AccountId", DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountName", FormAccountName);
                    cmd.Parameters.AddWithValue("@ParentAccountId", FormParentAccountId.HasValue ? (object)FormParentAccountId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountType", FormAccountType);
                    cmd.Parameters.AddWithValue("@IsActive", true);
                    StatusMessage = "Account created successfully.";
                    break;

                case "Update":
                    cmd.Parameters.AddWithValue("@AccountId", FormAccountId);
                    cmd.Parameters.AddWithValue("@AccountName", FormAccountName);
                    cmd.Parameters.AddWithValue("@ParentAccountId", FormParentAccountId.HasValue ? (object)FormParentAccountId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountType", FormAccountType);
                    cmd.Parameters.AddWithValue("@IsActive", FormIsActive);
                    StatusMessage = "Account updated successfully.";
                    break;

                case "Delete":
                    cmd.Parameters.AddWithValue("@AccountId", FormAccountId);
                    cmd.Parameters.AddWithValue("@AccountName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ParentAccountId", DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountType", DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", DBNull.Value);
                    StatusMessage = "Account deleted successfully.";
                    break;
            }

            await cmd.ExecuteNonQueryAsync();

            return RedirectToPage();
        }

        private async Task LoadFlatAsync()
        {
            FlatList.Clear();
            var cs = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(cs);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(
                "SELECT AccountId, AccountName, ParentAccountId, AccountType, IsActive FROM ChartOfAccounts",
                conn);
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                FlatList.Add(new AccountItem
                {
                    AccountId = rdr.GetInt32(0),
                    AccountName = rdr.GetString(1),
                    ParentAccountId = rdr.IsDBNull(2) ? null : rdr.GetInt32(2),
                    AccountType = rdr.GetString(3),
                    IsActive = rdr.GetBoolean(4)
                });
            }
        }

        private void BuildTree()
        {
            var lookup = FlatList.ToDictionary(a => a.AccountId);
            Tree.Clear();
            foreach (var acct in FlatList)
            {
                if (acct.ParentAccountId.HasValue && lookup.TryGetValue(acct.ParentAccountId.Value, out var parent))
                    parent.Children.Add(acct);
                else
                    Tree.Add(acct);
            }
        }
    }
}
