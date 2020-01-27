﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SustainAndGain.Models.Entities;
using SustainAndGain.Models.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models
{
	public class PortfoliosService
	{
		private readonly SustainGainContext context;
		private readonly UserManager<MyIdentityUser> user;
		private readonly IHttpContextAccessor accessor;

		public PortfoliosService(SustainGainContext context, UserManager<MyIdentityUser> user, IHttpContextAccessor accessor)
		{
			this.context = context;
			this.user = user;
			this.accessor = accessor;
		}

		public PortfolioVM DisplayPortfolioData(int compId)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			var lastupdatedCurrentValue = context.UsersInCompetition
				.Where(o => ((o.CompId == compId) && (o.UserId == userId)))
				.Max(o => o.LastUpdatedCurrentValue);

			var currentValue = context.UsersInCompetition
				.Where(o => o.LastUpdatedCurrentValue == lastupdatedCurrentValue)
				.Select(v => v.CurrentValue)
				.SingleOrDefault();


			var lastupdatedAvailableForInvestment = context.UsersInCompetition.
				Where(o => ((o.CompId == compId) && (o.UserId == userId)))
				.Max(o => o.LastUpdatedAvailableForInvestment);

			var availableForInvestment = context.UsersInCompetition
				.Where(o => o.LastUpdatedAvailableForInvestment == lastupdatedAvailableForInvestment)
				.Select(v => v.AvailableForInvestment)
				.FirstOrDefault();


			PortfolioVM portfolioData = new PortfolioVM
			{
				CurrentValue = (decimal)currentValue,
				AvailableCapital = (decimal)availableForInvestment,
				InvestedCapital = (decimal)(currentValue - availableForInvestment),
				CompetitionId = compId
			};

			return portfolioData;
		}

		internal object GetHighScoreForCompetition(int compId)
		{

			var MaxHighScore = context.UsersInCompetition
				.Where(a => a.CurrentValue > 0 && a.CompId == compId)
				.Select(n => new UsersInCompetition
				{
					AvailableForInvestment = n.AvailableForInvestment,
					CompId = n.CompId,
					CurrentValue = n.CurrentValue,
					LastUpdatedAvailableForInvestment = n.LastUpdatedAvailableForInvestment,
					LastUpdatedCurrentValue = n.LastUpdatedCurrentValue,
					UserId = n.UserId,
					User = n.User

				})
				.Take(10).OrderByDescending(s => s.CurrentValue);

			return MaxHighScore;

		}

		internal void AddBuyOrder(OrderVM order)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			context.Order.Add(new Order
			{
				StockId = context.StaticStockData
					.Where(s => s.Symbol == order.Symbol)
					.Select(i => i.Id)
					.SingleOrDefault(),
				OrderValue = order.OrderValue,
				TimeOfInsertion = DateTime.Now,
				BuyOrSell = "Buy",
				UserId = userId,
				CompId = order.CompetitionId
			});
			context.SaveChanges();
		}

		internal void AddSellOrder(OrderVM order, int compId)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			context.Order.Add(new Order
			{
				StockId = context.StaticStockData
					.Where(s => s.Symbol == order.Symbol)
					.Select(i => i.Id)
					.SingleOrDefault(),
				OrderValue = order.OrderValue,
				TimeOfInsertion = DateTime.Now,
				BuyOrSell = "Sell",
				UserId = userId,
				CompId = compId
			});
			context.SaveChanges();
		}


		internal OrderVM GetOrderEntry(string symbol, int compId)
		{
			return new OrderVM
			{
				CompanyName = context.StaticStockData
					.Where(s => s.Symbol == symbol)
					.Select(s => s.CompanyName)
					.SingleOrDefault(),
				Symbol = symbol,
				OrderValue = 0,
				CompetitionId = compId

			};
		}



		internal List<CalculatedPriceVM> GetPurchasePrice(int compId)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			List<CalculatedPriceVM> holdings = new List<CalculatedPriceVM>();

			var calculatePurschasePrice = context.UsersHistoricalTransactions
				.Where(c => c.CompetitionId == compId && c.UserId == userId)
				.Select(c => new UsersHistoricalTransactions
				{
					StockId = c.StockId,
					TransactionPrice = c.TransactionPrice,
					BuyOrSell = c.BuyOrSell,
					UserId = c.UserId,
					DateTimeOfTransaction = c.DateTimeOfTransaction,
					CurrentHoldingsAfterTransaction = c.CurrentHoldingsAfterTransaction,
					CompetitionId = c.CompetitionId,
					Quantity = c.Quantity

				}).ToList();

			var hisdatastocks = context.HistDataStocks
					.Where(a => a.StockId == a.StockId).ToList();

			foreach (var item in calculatePurschasePrice)
			{
				var price = hisdatastocks.Where(a => a.StockId == item.StockId).Select(a => a.CurrentPrice).FirstOrDefault();

				var purchasePricePerStock = calculatePurschasePrice
					.Where(a => a.StockId == item.StockId).Sum(a => a.TransactionPrice * a.Quantity);

				var totalQuantityOfStocks = calculatePurschasePrice
					.Where(a => a.StockId == item.StockId).Sum(a => a.Quantity);

				var purrChasePrice = purchasePricePerStock / totalQuantityOfStocks;

				var newHolding = new CalculatedPriceVM
				{
					PurchasePrice = purrChasePrice,
					BuyOrSell = item.BuyOrSell,
					TotalQuantity = item.CurrentHoldingsAfterTransaction,
					StockId = item.StockId,
					Quantity = item.Quantity,
					UserId = item.UserId,
					CurrentPrice = Convert.ToDecimal(price),
					TransactionPrice = item.TransactionPrice
				};

				holdings.Add(newHolding);
			}

			List<CalculatedPriceVM> trimmedList = new List<CalculatedPriceVM>();

			for (int i = 0; i < holdings.Count; i++)
			{
				var lslalal = holdings
					.Find(a => a.StockId == holdings[i].StockId);
				if (!trimmedList.Contains(lslalal))
				{
					trimmedList.Add(lslalal);
				}
			}

			return trimmedList;


		}

		internal List<HoldingsVM> GetHoldings(int compId)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			var holdings = context.UsersHistoricalTransactions
				.Where(o => o.CompetitionId == compId && o.UserId == userId)
				.Select(o => new HoldingsVM
				{
					Symbol = context.StaticStockData.Where(s => s.Id == o.StockId).Select(s => s.Symbol).SingleOrDefault(),
					CompanyName = context.StaticStockData.Where(s => s.Id == o.StockId).Select(s => s.CompanyName).SingleOrDefault(),
					IsSustainable = context.StaticStockData.Where(s => s.Id == o.StockId).Select(s => s.IsSustainable).SingleOrDefault(),
					BuyOrSell = o.BuyOrSell,
					TotalQuantity = o.CurrentHoldingsAfterTransaction
				}).ToList();

			foreach (var item in holdings)
			{
				var latestPrice = context.HistDataStocks
						.Where(o => ((o.Symbol == item.Symbol))).Max(o => o.DateTime);

				item.CurrentPrice = (decimal)context.HistDataStocks
						.Where(o => ((o.Symbol == item.Symbol) && (o.DateTime == latestPrice)))
						.Select(o => o.CurrentPrice)
						.SingleOrDefault();
			}

			return holdings;
		}

		internal StockInfoVM[] FindStocks(int compId)
		{

			var stocks = context.StaticStockData
				.Select(s => new StockInfoVM
				{
					CompanyName = s.CompanyName,
					IsSustainable = s.IsSustainable,
					Symbol = s.Symbol,
					LastUpdated = context.HistDataStocks
						.Where(o => ((o.Symbol == s.Symbol))).Max(o => o.DateTime),
					Description = s.Description,
					CompetitionId = compId
				})
				.ToArray();

			foreach (var item in stocks)
			{
				item.LastPrice = (decimal)context.HistDataStocks
						.Where(o => ((o.Symbol == item.Symbol) && (o.DateTime == item.LastUpdated)))
						.Select(o => o.CurrentPrice)
						.SingleOrDefault();
			}
			return stocks;
		}

		internal IEnumerable<OrderVM> GetPendingOrders(int compId)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			var orders = context.Order
				.Where(o => o.CompId == compId && o.UserId == userId)
				.Select(o => new OrderVM
				{
					Symbol = context.StaticStockData.Where(s => s.Id == o.StockId).Select(s => s.Symbol).SingleOrDefault(),
					OrderValue = o.OrderValue,
					BuyOrSell = o.BuyOrSell,
					TimeOfInsertion = o.TimeOfInsertion
				});

			return orders;
		}
	}
}
