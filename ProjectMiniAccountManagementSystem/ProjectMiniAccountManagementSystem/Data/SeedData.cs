using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectMiniAccountManagementSystem.Data;

namespace ProjectMiniAccountManagementSystem.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure DB is created
            context.Database.Migrate();

            // Connect to DB to execute stored procedure
            var connectionString = config.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string[] roles = { "Admin", "Accountant", "Viewer" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    using var command = new SqlCommand("sp_ManageRoles", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Action", "Create");
                    command.Parameters.AddWithValue("@RoleName", roleName);
                    command.Parameters.AddWithValue("@RoleId", DBNull.Value); // not needed for Create

                    await command.ExecuteNonQueryAsync();
                }
            }

            // Create default Admin user
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@1234!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }


            //// Seed Modules
            //string[] modules = { "Chart of Accounts", "Voucher Entry" };

            //foreach (var moduleName in modules)
            //{
            //    var checkModuleCmd = new SqlCommand("SELECT COUNT(*) FROM Modules WHERE Name = @name", connection);
            //    checkModuleCmd.Parameters.AddWithValue("@name", moduleName);

            //    var moduleExists = (int)await checkModuleCmd.ExecuteScalarAsync();

            //    if (moduleExists == 0)
            //    {
            //        var insertModuleCmd = new SqlCommand("INSERT INTO Modules (Name) VALUES (@name)", connection);
            //        insertModuleCmd.Parameters.AddWithValue("@name", moduleName);
            //        await insertModuleCmd.ExecuteNonQueryAsync();
            //    }
            //}

            //// Assign all modules to Admin Role using sp_AssignModuleAccess
            //var adminRole = await roleManager.FindByNameAsync("Admin");

            //if (adminRole != null)
            //{
            //    var getModuleIdsCmd = new SqlCommand("SELECT Id FROM Modules", connection);
            //    var reader = await getModuleIdsCmd.ExecuteReaderAsync();

            //    var moduleIds = new List<int>();
            //    while (await reader.ReadAsync())
            //    {
            //        moduleIds.Add(reader.GetInt32(0));
            //    }
            //    await reader.CloseAsync();

            //    foreach (var moduleId in moduleIds)
            //    {
            //        var assignCmd = new SqlCommand("sp_AssignModuleAccess", connection)
            //        {
            //            CommandType = System.Data.CommandType.StoredProcedure
            //        };

            //        assignCmd.Parameters.AddWithValue("@RoleId", adminRole.Id);
            //        assignCmd.Parameters.AddWithValue("@ModuleId", moduleId);

            //        await assignCmd.ExecuteNonQueryAsync();
            //    }
            //}


        }
    }
}
