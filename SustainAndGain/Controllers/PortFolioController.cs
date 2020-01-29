using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SustainAndGain.Models;
using SustainAndGain.Models.Entities;
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

        [Route("/Portfolio/{compId}")]
        [HttpGet]
        public IActionResult Portfolio(int compId)
        {
			//TESTING----
			service.UpdateCurrentValue(compId);

			var portfolioData = service.DisplayPortfolioData(compId);
            return View(portfolioData);
        }

		[Route("/Portfolio/FindStocks/{compId}")]
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
			return PartialView("_PendingOrder", pendingOrders);
		}

        [Route("Portfolio/Holdings/{compId}")]
        [HttpGet]
        public IActionResult Holdings(int compId)
        {
           
            //var holdings = service.GetHoldings(compId);


			var gav = service.GetPurchasePrice(compId);

			//var holdings = service.GetHoldings(compId);
			return PartialView("_Holdings", gav);
		}


		[Route("/highscore/{compId}")]
		[HttpGet]
		public IActionResult Highscore(int compId)
		{
			var highscores = service.GetHighScoreForCompetition(compId);
			//där current value är högst i nuvarande tävling
			return PartialView("_highscore", highscores);
		}
		[Route("/procent/{compId}")]
		[HttpGet]
		public IActionResult SustainProcent(int compId)
		{
			var highscores = service.GetHighScoreForCompetition(compId);
			//där current value är högst i nuvarande tävling
			return PartialView("_highscore", highscores);
		}


		[Route("Portfolio/OrderEntry/{symbol}/{compId}")]
		[HttpGet]
		public IActionResult OrderEntry(string symbol, int compId)
		{
			var orderEntry = service.GetOrderEntry(symbol, compId);
			return PartialView("_OrderEntry", orderEntry);
		}

		[Route("Portfolio/OrderEntry/{symbol}/{compId}")]
		[HttpPost]
		public IActionResult OrderEntry(OrderVM order)
		{
			if (!ModelState.IsValid)
				return View(order);

			service.AddBuyOrder(order);

			service.ExecuteOrders();    //Testing


			return RedirectToAction("FindStocks", new { compId = order.CompetitionId });
        }

		[Route("/Portfolio/DeleteOrder/{id}")]
		[HttpPost]
		public IActionResult DeleteOrder(int id)
		{

			Order order = service.DeleteOrder(id);

			return RedirectToAction("Portfolio", new { compId = order.CompId });
		}

		[Route("Portfolio/OrderEntrySell/{symbol}/{compId}")]
		[HttpGet]
		public IActionResult OrderEntrySell(string symbol, int compId)
		{
			var orderEntrySell = service.GetOrderEntrySell(symbol, compId);
			return PartialView("OrderEntrySell", orderEntrySell);
		}

		[Route("Portfolio/OrderEntrySell/{symbol}/{compId}")]
		[HttpPost]
		public IActionResult OrderEntrySell(OrderVM order)
		{
			if (!ModelState.IsValid)
				return View(order);

			service.AddSellOrder(order);

			service.ExecuteOrders();    //Testing, should be executed after GetPrices job

			return RedirectToAction("Portfolio", new { compId = order.CompetitionId });
		}
	}
}