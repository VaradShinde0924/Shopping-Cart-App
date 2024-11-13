using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using ShoppingCartApp.Models;
using System;

[assembly: OwinStartup(typeof(ShoppingCartApp.Startup))]

namespace ShoppingCartApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            CreateRolesAndAdminUser();
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout"),
                ExpireTimeSpan = TimeSpan.FromMinutes(30),
                SlidingExpiration = true
            });
        }

        private void CreateRolesAndAdminUser()
        {
            using (var context = new ApplicationDbContext())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                // Create Admin Role
                if (!roleManager.RoleExists("Admin"))
                {
                    var role = new IdentityRole { Name = "Admin" };
                    var roleResult = roleManager.Create(role);
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception("Failed to create Admin role: " + string.Join(", ", roleResult.Errors));
                    }
                }

                // Create Admin User
                var existingUser = userManager.FindByName("admin");
                if (existingUser == null)
                {
                    var user = new ApplicationUser { UserName = "admin", Email = "admin@example.com", EmailConfirmed = true };
                    string password = "Admin@123";
                    var userResult = userManager.Create(user, password);

                    if (userResult.Succeeded)
                    {
                        userManager.AddToRole(user.Id, "Admin");
                    }
                    else
                    {
                        throw new Exception("Failed to create Admin user: " + string.Join(", ", userResult.Errors));
                    }
                }
            }
        }

        // Reset Admin Password
        public void ResetAdminPassword()
        {
            using (var context = new ApplicationDbContext())
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                var adminUser = userManager.FindByName("admin");
                if (adminUser != null)
                {
                    string newPassword = "Admin@1234";
                    var passwordResetResult = userManager.RemovePassword(adminUser.Id);
                    if (passwordResetResult.Succeeded)
                    {
                        var addPasswordResult = userManager.AddPassword(adminUser.Id, newPassword);
                        if (!addPasswordResult.Succeeded)
                        {
                            throw new Exception("Failed to set new password: " + string.Join(", ", addPasswordResult.Errors));
                        }
                    }
                    else
                    {
                        throw new Exception("Failed to remove old password: " + string.Join(", ", passwordResetResult.Errors));
                    }
                }
            }
        }
    }
}