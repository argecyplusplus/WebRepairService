using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace WebRepairService.Models
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Создание ролей
            string[] roleNames = { "Admin", "Operator", "Engineer" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Создание администратора
            var adminUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                FullName = "Administrator"
            };

            string adminPassword = "Admin123!";
            var user = await userManager.FindByEmailAsync(adminUser.Email);

            if (user == null)
            {
                var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Создание оператора
            var operatorUser = new ApplicationUser
            {
                UserName = "operator@example.com",
                Email = "operator@example.com",
                FullName = "Operator"
            };

            string operatorPassword = "Operator123!";
            user = await userManager.FindByEmailAsync(operatorUser.Email);

            if (user == null)
            {
                var createOperator = await userManager.CreateAsync(operatorUser, operatorPassword);
                if (createOperator.Succeeded)
                {
                    await userManager.AddToRoleAsync(operatorUser, "Operator");
                }
            }

            // Создание инженера
            var engineerUser = new ApplicationUser
            {
                UserName = "engineer@example.com",
                Email = "engineer@example.com",
                FullName = "Engineer"
            };

            string engineerPassword = "Engineer123!";
            user = await userManager.FindByEmailAsync(engineerUser.Email);

            if (user == null)
            {
                var createEngineer = await userManager.CreateAsync(engineerUser, engineerPassword);
                if (createEngineer.Succeeded)
                {
                    await userManager.AddToRoleAsync(engineerUser, "Engineer");
                }
            }
        }
    }
}
