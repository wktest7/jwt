using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTokenow.Data
{
    public class SeedUser
    {
        private readonly RoleManager<IdentityRole> _roleMgr;
        private readonly UserManager<User> _userMgr;

        public SeedUser(UserManager<User> userMgr, RoleManager<IdentityRole> roleMgr)
        {
            _userMgr = userMgr;
            _roleMgr = roleMgr;
        }

        public async Task Seed()
        {
            var admin = await _userMgr.FindByNameAsync("aaaaBBBB");
            // add admin
            if (admin == null)
            {
                if (!(await _roleMgr.RoleExistsAsync("Admin")))
                {
                    var role = new IdentityRole("Admin");
                    await _roleMgr.CreateAsync(role);
                    //role.Claims.Add(new IdentityRoleClaim<string>() { ClaimType = "IsAdmin", ClaimValue = "True" });
                    //await _roleMgr.CreateAsync(role);
                }

                admin = new User()
                {
                    FirstName = "aaaa",
                    LastName = "BBBB",
                    UserName = "aaaaBBBB"
                };

                var userResult = await _userMgr.CreateAsync(admin, "masmix");
                var roleResult = await _userMgr.AddToRoleAsync(admin, "Admin");
                //var claimResult = await _userMgr.AddClaimAsync(user, new Claim("SuperUser", "True"));

                if (!userResult.Succeeded || !roleResult.Succeeded /*|| !claimResult.Succeeded*/)
                {
                    throw new InvalidOperationException("Failed to build user and roles");
                }

            }

            var user = await _userMgr.FindByNameAsync("ccccDDDD");
            //add user
            if (user == null)
            {
                //if (!(await _roleMgr.RoleExistsAsync("User")))
                //{
                //    var role = new IdentityRole("User");
                //    await _roleMgr.CreateAsync(role);
                //    //role.Claims.Add(new IdentityRoleClaim<string>() { ClaimType = "IsAdmin", ClaimValue = "True" });
                //    //await _roleMgr.CreateAsync(role);
                //}

                user = new User()
                {
                    FirstName = "cccc",
                    LastName = "dddd",
                    UserName = "ccccdddd"
                };

                var userResult = await _userMgr.CreateAsync(user, "masmix");
                //var roleResult = await _userMgr.AddToRoleAsync(user, "User");
                //var claimResult = await _userMgr.AddClaimAsync(user, new Claim("SuperUser", "True"));

                if (!userResult.Succeeded /*|| !roleResult.Succeeded *//*|| !claimResult.Succeeded*/)
                {
                    throw new InvalidOperationException("Failed to build user and roles");
                }

            }

        }
    }
}
