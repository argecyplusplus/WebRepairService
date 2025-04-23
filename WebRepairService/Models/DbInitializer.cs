using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;


namespace WebRepairService.Models
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

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

            //Создание других полей сущностей



            //Типы устройств

            if (!context.DeviceTypes.Any())
            {
                context.DeviceTypes.AddRange(
                    new DeviceType { Name = "Смартфон" },
                    new DeviceType { Name = "Планшет" },
                    new DeviceType { Name = "Ноутбук" },
                    new DeviceType { Name = "Стационарный компьютер" },
                    new DeviceType { Name = "Моноблок" },
                    new DeviceType { Name = "Игровая консоль" },
                    new DeviceType { Name = "Умные часы" },
                    new DeviceType { Name = "Фитнес-браслет" },
                    new DeviceType { Name = "Фотоаппарат" },
                    new DeviceType { Name = "Видеокамера" },
                    new DeviceType { Name = "Телевизор" },
                    new DeviceType { Name = "Монитор" },
                    new DeviceType { Name = "Принтер" },
                    new DeviceType { Name = "Сканер" },
                    new DeviceType { Name = "МФУ" },
                    new DeviceType { Name = "Роутер" },
                    new DeviceType { Name = "Внешний жесткий диск" },
                    new DeviceType { Name = "SSD накопитель" },
                    new DeviceType { Name = "USB флеш-накопитель" },
                    new DeviceType { Name = "Аудиосистема" },
                    new DeviceType { Name = "Другое" }
                );
                await context.SaveChangesAsync();
            }

            //Типы услуг
            if (!context.ServiceTypes.Any())
            {
                context.ServiceTypes.AddRange(
                    new ServiceType { Name = "", MinimalPrice = , MinimalWorktime = },

                );

                await context.SaveChangesAsync();

            }



            //Статусы
            if (!context.Statuses.Any())
            {
                context.Statuses.AddRange(
                    new Status { Name = "" },

                );

                await context.SaveChangesAsync();

            }
        }
    }
}
