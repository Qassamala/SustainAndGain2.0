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
        private readonly StocksService service;

        public UserController(StocksService service)
        {
            this.service = service;
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}