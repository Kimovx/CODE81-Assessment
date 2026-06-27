using CODE81_Assessment.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CODE81_Assessment.Infrastructure
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            // Seed Roles
            string[] roles = ["Admin", "Librarian", "Staff"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new AppRole { Name = role });
            }

            // Seed Admin User
            if (await userManager.FindByNameAsync("admin") == null)
            {
                var admin = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@library.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin@123456");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
