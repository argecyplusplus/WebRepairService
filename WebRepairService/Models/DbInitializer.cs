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
                    new DeviceType { Name = "ПК/комплектующие" },
                    new DeviceType { Name = "Компьютерная перифирия" },
                    new DeviceType { Name = "Игровая консоль/приставка" },
                    new DeviceType { Name = "Умные часы/браслет" },
                    new DeviceType { Name = "Монитор/ТВ" },
                    new DeviceType { Name = "Принтер/МФУ/Сканер" },
                    new DeviceType { Name = "Роутер/Сетевое оборудование" },
                    new DeviceType { Name = "Дисковый накопитель (HDD/SSD/USB)" },
                    new DeviceType { Name = "Акустика/Наушники" },
                    new DeviceType { Name = "Фотоаппарат/Видеокамера" },
                    new DeviceType { Name = "Дрон/Квадрокоптер" },
                    new DeviceType { Name = "Микроволновая печь" },
                    new DeviceType { Name = "Кофемашина" },
                    new DeviceType { Name = "Робот-пылесос" },
                    new DeviceType { Name = "Другое" }
                );
                await context.SaveChangesAsync();
            }

            //Типы услуг
            if (!context.ServiceTypes.Any())
            {
                context.ServiceTypes.AddRange(
                    new ServiceType { Name = "Комплексная диагностика", MinimalPrice = 500, MinimalWorktime = TimeSpan.FromHours(0.5) },
                    new ServiceType { Name = "Чистка от пыли и загрязнений", MinimalPrice = 1000, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Обновление прошивки/ПО", MinimalPrice = 1000, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Восстановление заводских настроек", MinimalPrice = 700, MinimalWorktime = TimeSpan.FromHours(0.5) },
                    new ServiceType { Name = "Тестирование компонентов (аппаратное)", MinimalPrice = 900, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Чистка разъемов", MinimalPrice = 300, MinimalWorktime = TimeSpan.FromHours(0.5) },
                    new ServiceType { Name = "Ремонт/замена разъемов питания", MinimalPrice = 1500, MinimalWorktime = TimeSpan.FromHours(1.5) },
                    new ServiceType { Name = "Устранение окислений после попадания влаги", MinimalPrice = 2000, MinimalWorktime = TimeSpan.FromHours(2) },
                    new ServiceType { Name = "Ремонт корпуса", MinimalPrice = 1200, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Резервное копирование данных", MinimalPrice = 1000, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Перенос данных на новый носитель", MinimalPrice = 1200, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Удаление вирусов и вредоносного ПО", MinimalPrice = 1500, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Базовая настройка устройства", MinimalPrice = 800, MinimalWorktime = TimeSpan.FromHours(0.5) },
                    new ServiceType { Name = "Оптимизация работы системы", MinimalPrice = 1000, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Настройка подключения к сети", MinimalPrice = 300, MinimalWorktime = TimeSpan.FromHours(0.5) },
                    new ServiceType { Name = "Калибровка сенсоров/экрана", MinimalPrice = 900, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Профилактический осмотр устройства", MinimalPrice = 500, MinimalWorktime = TimeSpan.FromHours(0.5) },
                    new ServiceType { Name = "Смазка механических частей", MinimalPrice = 800, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Замена термопасты/термопрокладок", MinimalPrice = 1200, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Проверка аккумулятора и замена при необходимости", MinimalPrice = 1000, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Калибровка аккумулятора", MinimalPrice = 600, MinimalWorktime = TimeSpan.FromHours(0.5) },
                    new ServiceType { Name = "Замена аккумулятора", MinimalPrice = 1500, MinimalWorktime = TimeSpan.FromHours(1) },
                    new ServiceType { Name = "Диагностика системы питания", MinimalPrice = 800, MinimalWorktime = TimeSpan.FromHours(0.5) },
                    new ServiceType { Name = "Консультация по эксплуатации устройства", MinimalPrice = 300, MinimalWorktime = TimeSpan.FromHours(0.3) },
                    new ServiceType { Name = "Подбор совместимых аксессуаров", MinimalPrice = 500, MinimalWorktime = TimeSpan.FromHours(0.5) },
                    new ServiceType { Name = "Установка защитного стекла/пленки", MinimalPrice = 400, MinimalWorktime = TimeSpan.FromHours(0.3) },
                    new ServiceType { Name = "Другое", MinimalPrice = 0, MinimalWorktime = TimeSpan.Zero }
                );

                await context.SaveChangesAsync();

            }



            //Статусы
            if (!context.Statuses.Any())
            {
                context.Statuses.AddRange(
                    new Status { Name = "Принят оператором" },
                    new Status { Name = "Принят инженером" },
                    new Status { Name = "В процессе" },
                    new Status { Name = "Завершен" },
                    new Status { Name = "Отменен" }
                   );

                await context.SaveChangesAsync();

            }
        }
    }
}
