using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SustainAndGain.Models;
using SustainAndGain.Models.ModelViews;
using SustainAndGain.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace SustainAndGain.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UsersService service;

        public UserController(UsersService service)
        {
            this.service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        [Route("/login")]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(UserLoginVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);


            var result = await service.TryLoginUser(vm);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Couldn't sign in.");
                return View(vm);
            }

            return RedirectToAction("UserLayout", "Stocks");
        }

        [AllowAnonymous]
        [Route("/Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("/Register")]
        [HttpPost]
        public async Task<IActionResult> Register(UsersRegisterVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result = await service.TryCreateUserAsync(vm);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(UsersRegisterVM.UserName), result.Errors.First().Description);

                ModelState.AddModelError(string.Empty, result.Errors.First().Description);
                return View(vm);
            }

            return RedirectToAction(nameof(Login));
        }

        [Route("Logout")]
        [HttpPost]
        public async Task<IActionResult> Logout(IFormCollection form)
        {
            await service.LogoutUserAsync();

            return RedirectToAction(nameof(Login));
        }









    }
}