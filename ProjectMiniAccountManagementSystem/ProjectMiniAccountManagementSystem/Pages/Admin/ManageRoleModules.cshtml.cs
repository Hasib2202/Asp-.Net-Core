using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectMiniAccountManagementSystem.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageRoleModulesModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleMgr;

        public ManageRoleModulesModel(IConfiguration config,
                                      RoleManager<IdentityRole> roleMgr)
        {
            _config = config;
            _roleMgr = roleMgr;
        }

        public class ModuleItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public string Description { get; set; }
            public bool IsActive { get; set; }
        }

        public List<IdentityRole> Roles { get; set; }
        public List<ModuleItem> Modules { get; set; }

        [BindProperty]
        public string SelectedRoleId { get; set; }

        [BindProperty]
        public List<int> SelectedModuleIds { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGetAsync(string selectedRoleId = null)
        {
            Roles = _roleMgr.Roles.ToList();
            Modules = new List<ModuleItem>();
            SelectedModuleIds = new List<int>();

            // Use provided roleId or default to first role
            if (string.IsNullOrEmpty(selectedRoleId) && Roles.Any())
                SelectedRoleId = Roles.First().Id;
            else
                SelectedRoleId = selectedRoleId;

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            // Load modules with additional information
            using (var cmd = new SqlCommand("SELECT Id, Name, Url, Description, IsActive FROM Modules", conn))
            using (var rdr = await cmd.ExecuteReaderAsync())
            {
                while (await rdr.ReadAsync())
                {
                    Modules.Add(new ModuleItem
                    {
                        Id = rdr.GetInt32(0),
                        Name = rdr.GetString(1),
                        Url = rdr.GetString(2),
                        Description = rdr.IsDBNull(3) ? null : rdr.GetString(3),
                        IsActive = rdr.GetBoolean(4)
                    });
                }
            }

            // Load assigned module ids for this role
            if (!string.IsNullOrEmpty(SelectedRoleId))
            {
                using (var cmd = new SqlCommand(
                    "SELECT ModuleId FROM ModuleRole WHERE RoleId = @rid", conn))
                {
                    cmd.Parameters.AddWithValue("@rid", SelectedRoleId);
                    using var rdr = await cmd.ExecuteReaderAsync();
                    while (await rdr.ReadAsync())
                        SelectedModuleIds.Add(rdr.GetInt32(0));
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(SelectedRoleId))
            {
                StatusMessage = "Error: No role selected.";
                return RedirectToPage();
            }

            Roles = _roleMgr.Roles.ToList();
            SelectedModuleIds ??= new List<int>(); // Handle null case

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            // 1. DELETE any existing assignments *not* in SelectedModuleIds
            var csv = (SelectedModuleIds.Any())
                        ? string.Join(",", SelectedModuleIds)
                        : "0";
            using (var del = new SqlCommand(@"
                DELETE FROM ModuleRole
                 WHERE RoleId = @rid
                   AND ModuleId NOT IN (SELECT value FROM STRING_SPLIT(@csv,','))", conn))
            {
                del.Parameters.AddWithValue("@rid", SelectedRoleId);
                del.Parameters.AddWithValue("@csv", csv);
                await del.ExecuteNonQueryAsync();
            }

            // 2. CALL sp_AssignModuleAccess for each checked
            foreach (var mid in SelectedModuleIds)
            {
                using var assign = new SqlCommand("sp_AssignModuleAccess", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                assign.Parameters.AddWithValue("@RoleId", SelectedRoleId);
                assign.Parameters.AddWithValue("@ModuleId", mid);
                await assign.ExecuteNonQueryAsync();
            }

            StatusMessage = "Module access updated successfully.";
            return RedirectToPage(new { selectedRoleId = SelectedRoleId });
        }
    }
}