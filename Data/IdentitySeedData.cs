using System.Linq;
using DotNetTeacherBot.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetTeacherBot.Data
{
    public static class IdentitySeedData
    {
        private static string _adminUser;
        private static string _adminPassword;

        public static async void EnsurePopulated(IApplicationBuilder app, IConfiguration config)
        {
            _adminUser = config["AdminUser"];
            _adminPassword = config["AdminPassword"];
            AppIdentityDbContext context = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<AppIdentityDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            UserManager<IdentityUser> userManager = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            IdentityUser user = await userManager.FindByIdAsync(_adminUser);
            if (user == null)
            {
                user = new IdentityUser(_adminUser);
                user.Email = "admin@example.com";
                user.PhoneNumber = "555-1234";
                await userManager.CreateAsync(user, _adminPassword);
            }
        }
         
        
    }
}