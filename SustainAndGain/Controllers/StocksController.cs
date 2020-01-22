﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SustainAndGain.Models;

namespace SustainAndGain.Controllers
{
    //[Authorize]
    public class StocksController : Controller
    {
        private readonly StocksService service;
        private readonly CompetitionsService competitionsService;

        public StocksController(StocksService service, CompetitionsService competitionsService)
        {
            this.service = service;
            this.competitionsService = competitionsService;
        }
       

        [Route("List")]
        public IActionResult List()
        {

            //competitionsService.AddCompetition();
            service.AddHistDataStocks();
            //service.AddHistDataStocks();
            //service.AddHistDataStocks();
            //service.AddStaticStockData();

            // Test reset 1


            return View();
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