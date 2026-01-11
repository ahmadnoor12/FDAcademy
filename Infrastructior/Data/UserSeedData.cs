using Domain;
using Domain.Entities;
using Domain.Entities.Enum;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{

    public class UserSeedData
    {
        public static async Task InitializeAsync(FDAcademyDbContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.AddRange
                    (
                    new Role { Name = FDAConst.ADMIN_ROLE, Code = RoleEnum.Admin },
                    new Role { Name = FDAConst.STUDENT_ROLE, Code = RoleEnum.Student }
                    );
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                var passwordHasher = new PasswordHasher<User>();
                var adminRoleId = context.Roles.First(a => a.Code == RoleEnum.Admin).Id;

                var admin = new User
                {

                    Name = "System Admin",
                    Email = "admin@fda.com",
                    PhoneNumber = "0797894562",
                    RoleId = adminRoleId
                };
                admin.Password = passwordHasher.HashPassword(admin, "Admin@123");

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
