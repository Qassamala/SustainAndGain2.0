﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SustainAndGain.Models;
using SustainAndGain.Models.ModelViews;

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
            //competitionsService.DisplayCompetitions();
            competitionsService.AddCompetition();
            //service.AddHistDataStocks();
            //service.AddHistDataStocks();
            //service.AddStaticStockData();

            // Test reset 1
            //service.AddStocksInComp();

            return View();
        }
        [Route("/UserLayout")]
        public IActionResult UserLayout()
        {
            var result = competitionsService.DisplayCompetitions();
            return View(result);
        }
        [Route("Portfolio/{id}")]
        public IActionResult Portfolio(int id)
        {

           
            return View();
        }
        [HttpPost]
        public bool InsertAjax(string userId, int compId)
        {
           bool value = service.AddUsersInComp(userId, compId);
            return value;
            
        }



        [Route("/chart{id}")]
        public IActionResult Chart(int id)
        {
            var viewmodel = service.GetHistoricalTransactionData(id);
           
            return PartialView(viewmodel);
        }

    }
}