using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SustainAndGain.Models.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models
{
    public class UsersService
    {
        private readonly UserManager<MyIdentityUser> userManager;
        private readonly SignInManager<MyIdentityUser> signInManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UsersService(UserManager<MyIdentityUser> userManager, SignInManager<MyIdentityUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        internal async Task<UserMemberVM> GetLoggedInUser()
        {
            string userId = userManager.GetUserId(httpContextAccessor.HttpContext.User);
            MyIdentityUser user = await userManager.FindByIdAsync(userId);
            UserMemberVM vm = new UserMemberVM
            {               
                Email = user.Email,
                UserName = user.UserName
            };

            return vm;
        }

        internal async Task LogoutUserAsync()
        {
            await signInManager.SignOutAsync();
        }

        //public async void LogOutUser(UserMemberVM vm)
        //{
        //    string userId = userManager.GetUserId(httpContextAccessor.HttpContext.User);
        //    MyIdentityUser user = await userManager.FindByIdAsync(userId);
        //    var result = await userManager.RemoveLoginAsync(user, "hej", "hej");
        //}

        internal async Task<IdentityResult> TryCreateUserAsync(UsersRegisterVM vm)
        {
            var result = await userManager.CreateAsync(new MyIdentityUser
            {
                
                UserName = vm.UserName,
                Email = vm.Email
            }, vm.Password);
            if (result.Succeeded)
                await signInManager.PasswordSignInAsync(vm.UserName, vm.Password, false, false);
            return result;
        }

        internal async Task<SignInResult> TryLoginUser(UserLoginVM vm)
        {
            var result = await signInManager.PasswordSignInAsync(vm.UserName, vm.Password, false, false);
            return result;
        }
    }
    }

