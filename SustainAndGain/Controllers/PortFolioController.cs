﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SustainAndGain.Models;
using SustainAndGain.Models.ModelViews;

namespace SustainAndGain.Controllers
{
    public class PortFolioController : Controller
    {
        private readonly PortfoliosService service;

        public PortFolioController(PortfoliosService service)
        {
            this.service = service;
        }

        [Route("Portfolio/{compId}")]
        [HttpGet]
        public IActionResult Portfolio(int compId)
        {
            var portfolioData = service.DisplayPortfolioData(compId);
            
            return View(portfolioData);
        }

        [Route("Portfolio/FindStocks/{compId}")]
        [HttpGet]
        public IActionResult FindStocks(int compId)
        {
            var stockData = service.FindStocks(compId);

            return View(stockData);
        }



        [Route("Portfolio/Orders/{compId}")]
        [HttpGet]
        public IActionResult Orders(int compId)
        {
            var pendingOrders = service.GetPendingOrders(compId);

            return PartialView("_Order", pendingOrders);
        }

        [Route("Portfolio/Holdings/{compId}")]
        [HttpGet]
        public IActionResult Holdings(int compId)
        {
            var holdings = service.GetHoldings(compId);

            return PartialView("_Holdings", holdings);
        }



        [Route("highscore")]
        [HttpGet]
        public IActionResult Highscore(int compId)
        {
            var holdings = service.GetHoldings(compId);

            return PartialView("_highscore", holdings);
        }



    }
}