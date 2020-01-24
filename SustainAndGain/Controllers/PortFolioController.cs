using System;
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

        [Route("Portfolio/Orders/{compId}")]
        [HttpGet]
        public IActionResult Orders(int compId)
        {
            var pendingOrders = service.GetPendingOrders(compId);

            return PartialView("_Order", pendingOrders);
        }
    }
}