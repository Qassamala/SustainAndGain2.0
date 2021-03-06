﻿using System;
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
	[Authorize]
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

			var gav = service.GetPurchasePrice(compId);

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
		[Route("/percentage/{compId}")]
		[HttpGet]
		public IActionResult SustainablePercentage(int compId)
		{
			var percent = service.GetSustainablePercentage(compId);
			return Json(percent*100);
		}


		[Route("Portfolio/OrderEntry/{symbol}/{compId}")]
		[HttpGet]
		public IActionResult OrderEntry(string symbol, int compId)
		{
			var orderEntry = service.GetOrderEntry(symbol, compId);
			return PartialView("OrderEntry", orderEntry);
		}

		[Route("Portfolio/OrderEntry/{symbol}/{compId}")]
		[HttpPost]
		public IActionResult OrderEntry(OrderVM order)
		{
			if (!ModelState.IsValid)
				return View(order);

			var availableForInvestment = service.CheckTotalAvailableToInvestForStockBuy(order);

			if (order.OrderValue > availableForInvestment || order.OrderValue <= 0)
			{
				ModelState.AddModelError(nameof(OrderVM.OrderValue), $"You only have {availableForInvestment} to purchase stocks for.");
				return View(order);
			}

			service.AddBuyOrder(order);

			service.ExecuteOrders();    //Testing for demonstration


			return RedirectToAction("FindStocks", new { compId = order.CompetitionId });
        }

		[Route("/Portfolio/DeleteOrder/{id}")]
		[HttpPost]
		public IActionResult DeleteOrder(int id)
		{

			Order order = service.DeleteOrder(id);

			return RedirectToAction("Portfolio", new { compId = order.CompId });
		}

		[Route("Portfolio/OrderEntrySell/{symbol}/{compId}/{stockId}")]
		[HttpGet]
		public IActionResult OrderEntrySell(string symbol, int compId, int stockId)
		{
			var orderEntrySell = service.GetOrderEntrySell(symbol, compId, stockId);
			return PartialView("OrderEntrySell", orderEntrySell);
		}

		[Route("Portfolio/OrderEntrySell/{symbol}/{compId}/{stockId}")]
		[HttpPost]
		public IActionResult OrderEntrySell(SellOrderVM order)
		{
			if (!ModelState.IsValid)
				return View(order);

			var totalHolding = service.CheckTotalHoldingsForStockSell(order);

			if (order.Quantity > totalHolding || order.Quantity < 1)
			{
				ModelState.AddModelError(nameof(SellOrderVM.Quantity), $"Enter a quantity no more than {totalHolding}.");
				return View(order);
			}

			service.AddSellOrder(order);

			service.ExecuteOrders();    //Testing, should be executed after GetPrices job

			return RedirectToAction("Portfolio", new { compId = order.CompetitionId });
		}
	}
}