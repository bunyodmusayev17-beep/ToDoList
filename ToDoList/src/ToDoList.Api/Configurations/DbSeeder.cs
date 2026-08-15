using Microsoft.EntityFrameworkCore;
using ToDoList.Application.Abstractions;
using ToDoList.Domain.Entities;
using ToDoList.Infrastructure.Persistence;

namespace ToDoList.Api.Configurations;

public static class DbSeeder
{
    private const string DefaultPassword = "Password@123";

    /// <summary>
    /// One-time sample data seeder: 10 users (1 super-admin, 2 admins, 7 regular users)
    /// and 30 to-do items, all with Uzbek names/titles.
    /// Safe to leave in place — it skips seeding if any users already exist.
    /// </summary>
    public static async Task SeedSampleDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        var db = services.GetRequiredService<AppDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasherService>();

        //if (true || await db.Users.AnyAsync())
        //{
        //    logger.LogInformation("Users already exist — skipping sample data seeding.");
        //    return;
        //}

        var now = DateTime.UtcNow;

        User CreateUser(string userName, string firstName, string lastName, UserRole role)
        {
            var (hash, salt) = passwordHasher.Hasher(DefaultPassword);
            return new User
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                Email = $"{userName}@todolist.uz",
                Password = hash,
                Salt = salt,
                Role = role,
                EmailConfirmed = true,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        // ---- 10 users: 1 super-admin, 2 admins, 7 regular users ----
        var users = new List<User>
        {
            CreateUser("akmal", "Akmal", "Karimov", UserRole.SuperAdmin),
            CreateUser("dilnoza", "Dilnoza", "Rashidova", UserRole.Admin),
            CreateUser("jasur", "Jasur", "Yusupov", UserRole.Admin),
            CreateUser("nodira", "Nodira", "Toshmatova", UserRole.User),
            CreateUser("sardor", "Sardor", "Abdullayev", UserRole.User),
            CreateUser("gulnora", "Gulnora", "Ergasheva", UserRole.User),
            CreateUser("bekzod", "Bekzod", "Raximov", UserRole.User),
            CreateUser("malika", "Malika", "Sultonova", UserRole.User),
            CreateUser("ulugbek", "Ulug'bek", "Nazarov", UserRole.User),
            CreateUser("feruza", "Feruza", "Islomova", UserRole.User),
        };

        await db.Users.AddRangeAsync(users);
        await db.SaveChangesAsync();

        // ---- 30 to-do items (Uzbek titles), distributed across all users ----
        var tasks = new (string Title, string Description)[]
        {
            ("Non sotib olish", "Do'kondan non va sut xarid qilish"),
            ("Ishga borish", "Ertalab soat 9 da ofisga yetib borish"),
            ("Kitob o'qish", "\"O'tkan kunlar\" romanini o'qishni tugatish"),
            ("Uy vazifasini bajarish", "Matematika mashqlarini yechish"),
            ("Ota-onani yo'qlash", "Qishloqdagi ota-onaga qo'ng'iroq qilish"),
            ("Mashinani yuvish", "Avtomobilni tashqi va ichki tozalash"),
            ("Hisobotni tayyorlash", "Oylik moliyaviy hisobotni yakunlash"),
            ("Yig'ilishga tayyorgarlik", "Ertangi uchrashuv uchun taqdimot tayyorlash"),
            ("Sport zaliga borish", "Kechqurun mashg'ulotga qatnashish"),
            ("Dori sotib olish", "Dorixonadan retsept bo'yicha dori olish"),
            ("Kommunal to'lovlarni to'lash", "Gaz, suv va elektr uchun to'lov qilish"),
            ("Do'stlar bilan uchrashuv", "Choyxonada do'stlar bilan ko'rishish"),
            ("Loyihani yakunlash", "Veb-sayt loyihasini muddatida topshirish"),
            ("Elektron pochtani tekshirish", "Yangi xatlarga javob yozish"),
            ("Bozorga borish", "Haftalik oziq-ovqat mahsulotlarini olish"),
            ("Tug'ilgan kunga sovg'a", "Singlimga tug'ilgan kun sovg'asini tanlash"),
            ("Ingliz tilini o'rganish", "Yangi 20 ta so'zni yodlash"),
            ("Shifokorga yozilish", "Tish shifokoriga navbat olish"),
            ("Uyni yig'ishtirish", "Mehmonlar kelishidan oldin uyni tozalash"),
            ("Bank ishini hal qilish", "Plastik kartani yangilash"),
            ("Sayohatni rejalashtirish", "Samarqandga borish uchun chiptalar olish"),
            ("Kompyuterni yangilash", "Dasturlarni so'nggi versiyaga yangilash"),
            ("Maqola yozish", "Blog uchun yangi maqola tayyorlash"),
            ("Gullarni sug'orish", "Uydagi gullarni sug'orish"),
            ("Velosipedni ta'mirlash", "G'ildirakni almashtirish"),
            ("Onlayn kurs ko'rish", "Dasturlash bo'yicha darsni tomosha qilish"),
            ("Fotosuratlarni saralash", "Sayohat rasmlarini albomga joylash"),
            ("Mebel yig'ish", "Yangi kitob javonini o'rnatish"),
            ("Nonushta tayyorlash", "Oila uchun ertalabki taom pishirish"),
            ("Kunlik reja tuzish", "Ertangi kun uchun vazifalarni belgilash"),
        };

        var priorities = new[]
        {
            PriorityLevel.Low,
            PriorityLevel.Medium,
            PriorityLevel.High,
            PriorityLevel.Critical
        };

        var items = new List<ToDoItem>();
        for (var i = 0; i < tasks.Length; i++)
        {
            var owner = users[i % users.Count];           // round-robin → 3 items per user
            var priority = priorities[i % priorities.Length];
            var isCompleted = i % 3 == 0;                 // every 3rd item is completed

            items.Add(new ToDoItem
            {
                Title = tasks[i].Title,
                Description = tasks[i].Description,
                Priority = priority,
                IsCompleted = isCompleted,
                IsDeleted = false,
                CompletedAt = isCompleted ? now.AddDays(-(i % 5)) : null,
                DueDate = now.AddDays((i % 10) + 1),
                ReminderAt = now.AddDays(i % 7),
                UserId = owner.UserId,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        await db.ToDoItems.AddRangeAsync(items);
        await db.SaveChangesAsync();

        logger.LogInformation(
            "Seeded {UserCount} users and {ItemCount} to-do items. Default password for all users: {Password}",
            users.Count, items.Count, DefaultPassword);
    }
}
