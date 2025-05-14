using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectMiniAccountManagementSystem.Pages.Admin
{
    [Authorize]
    public class RoleModuleAccessModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;

        public RoleModuleAccessModel(
            IConfiguration configuration,
            UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public List<RoleModules> RoleModulesList { get; set; } = new();

        public class RoleModules
        {
            public string RoleName { get; set; }
            public List<string> Modules { get; set; }
        }

        public async Task OnGetAsync()
        {
            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any()) return;

            // DB connection
            var cs = _configuration.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(cs);
            await conn.OpenAsync();

            foreach (var roleName in roles)
            {
                var modules = new List<string>();

                using var cmd = new SqlCommand(@"
                    SELECT m.Name 
                    FROM ModuleRole mr
                    JOIN AspNetRoles r ON r.Id = mr.RoleId
                    JOIN Modules m     ON m.Id = mr.ModuleId
                    WHERE r.Name = @role
                    ORDER BY m.Name", conn);

                cmd.Parameters.AddWithValue("@role", roleName);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    modules.Add(reader.GetString(0));
                }

                await reader.CloseAsync();

                RoleModulesList.Add(new RoleModules
                {
                    RoleName = roleName,
                    Modules = modules
                });
            }
        }
    }
}
