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
        private readonly PortfoliosService pservice;

        public StocksController(StocksService service, CompetitionsService competitionsService, PortfoliosService pservice)
        {
            this.service = service;
            this.competitionsService = competitionsService;
            this.pservice = pservice;
        }

       [AllowAnonymous]
       [Route("Admin")]
       public IActionResult Admin()
       {
            service.AddHistDataStocks();
            pservice.UpdateCurrentValues();
           //competitionsService.DisplayCompetitions();
           //competitionsService.AddCompetition();
           //service.AddSustainProp();
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