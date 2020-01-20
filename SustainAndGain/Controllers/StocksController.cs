using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SustainAndGain.Models;

namespace SustainAndGain.Controllers
{
    public class StocksController : Controller
    {
        
       
        private readonly StocksService service;
        private readonly UsersService usersService;

        public StocksController(StocksService service, UsersService usersService)
        {
            this.service = service;
            this.usersService = usersService;
        }
        [Route("")]
        [Route("/Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("List")]
        public IActionResult List()
        {
            var result = service.GetResultAsync();


            return View(result);
        }
        [Route("/UserLayout")]
        public IActionResult UserLayout()
        {
            return View();
        }
        [Route("/{competition}/Portfolio")]
        public IActionResult Portfolio()
        {
            return View();
        }

    }
}