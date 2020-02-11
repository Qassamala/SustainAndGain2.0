using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SustainAndGain.Models.Entities;
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
        private readonly SustainGainContext context;

        public UsersService(UserManager<MyIdentityUser> userManager, SignInManager<MyIdentityUser> signInManager, IHttpContextAccessor httpContextAccessor, SustainGainContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
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

        public async void LogOutUser(UserMemberVM vm)
        {
            await signInManager.SignOutAsync();
        }

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

        internal bool AddUsersInComp(CompetitionVM data)
        {
            UsersInCompetition stocks = new UsersInCompetition
            {
                UserId = data.UserId,
                CurrentValue = 10000,
                AvailableForInvestment = 10000,
                LastUpdatedAvailableForInvestment = DateTime.Now,
                LastUpdatedCurrentValue = DateTime.Now,
                CompId = int.Parse(data.CompId),
            };

            BonusDeposit deposit = new BonusDeposit
            {
                UserId = data.UserId,
                CompetitionId = int.Parse(data.CompId),
                Bonus = 0,
            };

            context.BonusDeposit.Add(deposit);
            context.UsersInCompetition.Add(stocks);
            context.SaveChanges();

            return true;
        }

        internal async Task<SignInResult> TryLoginUser(UserLoginVM vm)
        {
            var result = await signInManager.PasswordSignInAsync(vm.UserName, vm.Password, false, false);
            return result;
        }
    }
    }

