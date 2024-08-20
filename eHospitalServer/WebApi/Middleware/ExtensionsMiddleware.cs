using Entities.Concrete;
using Entities.Enums;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Middleware;

public static class ExtensionsMiddleware
{
    public static void CreateFirstUser(WebApplication app)
    {
        using(var scoped = app.Services.CreateScope())
        {
            var userManager = scoped.ServiceProvider.GetRequiredService<UserManager<User>>();

            if(!userManager.Users.Any(p=> p.UserName == "admin"))
            {
                User user = new()
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    FirstName = "Sibel",
                    LastName = "Öztürk",
                    IdentityNumber = "11111111111",
                    Address = "İstanbul",
                    EmailConfirmed = true,
                    IsActive = true,
                    IsDeleted = false,
                    BloodType = "0-",
                    UserType = UserType.Admin
                };
                userManager.CreateAsync(user, "123456").Wait();
            }
        }
    }
}