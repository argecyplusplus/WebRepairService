using Microsoft.AspNetCore.Identity;

namespace WebRepairService.Components
{
    public static class UserManagerExtensions
    {
        public static async Task<bool> HasNoRolesAsync<TUser>(this UserManager<TUser> userManager, TUser user)
            where TUser : class
        {
            var roles = await userManager.GetRolesAsync(user);
            return roles.Count == 0;
        }
    }
}
