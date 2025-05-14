using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;


[Authorize(Roles = "Admin")]
public class ManageUsersModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public List<UserViewModel> Users { get; set; } = new();
    public List<IdentityRole> Roles { get; set; } = new();

    public class UserViewModel
    {
        public IdentityUser User { get; set; }
        public List<string> UserRoles { get; set; }
    }

    public ManageUsersModel(UserManager<IdentityUser> userManager,
                          RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task OnGetAsync()
    {
        Roles = await _roleManager.Roles.ToListAsync();

        foreach (var user in _userManager.Users.ToList())
        {
            Users.Add(new UserViewModel
            {
                User = user,
                UserRoles = (await _userManager.GetRolesAsync(user)).ToList()
            });
        }
    }

    public async Task<IActionResult> OnPostAssignRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.AddToRoleAsync(user, roleName);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.RemoveFromRoleAsync(user, roleName);
        return RedirectToPage();
    }
}