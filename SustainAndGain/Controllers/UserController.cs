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
        private readonly CompetitionsService competitionsService;

        public UserController(UsersService service, CompetitionsService competitionsService)
        {
            this.service = service;
            this.competitionsService = competitionsService;
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

            return RedirectToAction(nameof(UserLayout));
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

            return RedirectToAction(nameof(UserLayout));
        }

        [Route("/UserLayout")]
        [HttpGet]
        public IActionResult UserLayout()
        {

            var result = competitionsService.DisplayCompetitions();

            return View(result);
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