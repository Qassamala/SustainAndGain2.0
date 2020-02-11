using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SustainAndGain.Models;
using SustainAndGain.Models.ModelViews;
using SustainAndGain.Controllers;

namespace SustainAndGain.Controllers
{
    public class UserController : Controller
    {
        private readonly UsersService service;
        private readonly CompetitionsService competitionsService;

        public UserController(UsersService service, CompetitionsService competitionsService)
        {
            this.service = service;
            this.competitionsService = competitionsService;
        }

        [HttpGet]
        [Route("")]
        [Route("/login")]
        public IActionResult Index()
        {
            return View();
        }

       
        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Index(UserLoginVM vm)
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

        [Route("/Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Route("/Register")]
        [HttpPost]
        public async Task<IActionResult> Register(UsersRegisterVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result = await service.TryCreateUserAsync(vm);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Errors.First().Description);
                return View(vm);
            }

            return RedirectToAction(nameof(UserLayout));
        }

        [Route("/UserLayout")]
        public IActionResult UserLayout()
        {

            var result = competitionsService.DisplayCompetitions();

            return View(result);
        }

        [HttpPost]
        [Route("User/InsertAjax")]
        public IActionResult InsertAjax([FromBody]CompetitionVM obj)
        {
            service.AddUsersInComp(obj);
            return Ok();

        }









    }
}