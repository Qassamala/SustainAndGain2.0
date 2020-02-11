using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SustainAndGain.Models;
using SustainAndGain.Models.Entities;
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

       [AllowAnonymous]
       [Route("Admin")]
       public IActionResult Admin()
       {
           //competitionsService.DisplayCompetitions();
           //competitionsService.AddCompetition();
           //service.AddSustainProp();
           //service.AddHistDataStocks();
           //service.AddStaticStockData();
           //service.GetCompanyDescription();

           // Test reset 1
           //service.AddStocksInComp();

           return View();
       }

        [Route("/chart/{id}")]
        public IActionResult Chart(int id)
        {
            var viewmodel = service.GetHistoricalTransactionData(id);

           return PartialView("Chart", viewmodel);
       }

       [Route("/financial/{id}")]
       public IActionResult FinancialChart(int id)
       {
           var viewmodel = service.GetHistoricalTransactionData(id);

           return PartialView("FinancialChart", viewmodel);
       }



    }
}