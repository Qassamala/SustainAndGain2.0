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
			{ CurrentValue = (decimal)currentValue,
				AvailableCapital = (decimal)availableForInvestment,
				InvestedCapital = (decimal)(currentValue - availableForInvestment),
				CompetitionId = compId };

			return portfolioData;
		}

		internal OrderEntryVM GetOrderEntry(string symbol, int compId)
		{
			return new OrderEntryVM
			{ 
				CompanyName = context.StaticStockData
					.Where(s => s.Symbol == symbol)
					.Select(s => s.CompanyName)
					.SingleOrDefault(),
				Symbol = symbol,
				OrderValue = 0
			};
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
