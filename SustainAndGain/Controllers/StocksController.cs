using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SustainAndGain.Models;

namespace SustainAndGain.Controllers
{
    [Authorize]
    public class StocksController : Controller
    {
        private readonly StocksService service;

        public StocksController(StocksService service)
        {
            this.service = service;
        }
       

        [Route("List")]
        public IActionResult List()
        {
            service.AddStaticStockData();

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