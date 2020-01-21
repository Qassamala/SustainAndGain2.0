using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SustainAndGain.Models;

namespace SustainAndGain.Controllers
{
    public class UserController : Controller
    {
        private readonly UsersService service;

        public UserController(UsersService service)
        {
            this.service = service;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/Register")]
        public IActionResult Register()
        {
            return View();
        }
    }
}