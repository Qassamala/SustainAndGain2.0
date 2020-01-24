using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SustainAndGain.Models;

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
    }
}