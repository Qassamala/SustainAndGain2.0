using Microsoft.AspNetCore.Http;
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
				.FirstOrDefault();


			var lastupdatedAvailableForInvestment = context.UsersInCompetition.
				Where(o => ((o.CompId == compId) && (o.UserId == userId)))
				.Max(o => o.LastUpdatedAvailableForInvestment);

			var availableForInvestment = context.UsersInCompetition
				.Where(o => o.LastUpdatedAvailableForInvestment == lastupdatedAvailableForInvestment)
				.Select(v => v.AvailableForInvestment)
				.FirstOrDefault();

			// Calculate current invested capital by getting the holdings market cap
			var holdings = GetHoldings(compId);

			var investedCapital = holdings.Select(h => h.MarketValue).Sum();

			PortfolioVM portfolioData = new PortfolioVM
			{
				CurrentValue = (decimal)currentValue,
				AvailableCapital = (decimal)availableForInvestment,
				InvestedCapital = investedCapital,
				CompetitionId = compId
			};

			return portfolioData;
		}

		internal object GetHighScoreForCompetition(int compId)
		{
			
			var MaxHighScore = context.UsersInCompetition
				.Where(a => a.CurrentValue > 0 && a.CompId == compId)
				.Select(n => new HighscoreVM
				{

					CurrentValue = n.CurrentValue,

					User = n.User,



				})
				.OrderByDescending(s => s.CurrentValue).Take(10).ToArray();

			int counter = 1;
			for (int i = 0; i < MaxHighScore.Length; i++)
			{
				MaxHighScore[i].Nr = counter++;
			}



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

			// Add entry into usersInComp with updated availableForInvestment value

			var lastupdatedCurrentValue = context.UsersInCompetition
				.Where(o => ((o.CompId == order.CompetitionId) && (o.UserId == userId)))
				.Max(o => o.LastUpdatedCurrentValue);

			var currentValue = context.UsersInCompetition
				.Where(o => o.LastUpdatedCurrentValue == lastupdatedCurrentValue)
				.Select(v => v.CurrentValue)
				.FirstOrDefault();

			var lastupdatedAvailableForInvestment = context.UsersInCompetition.
				Where(o => ((o.CompId == order.CompetitionId) && (o.UserId == userId)))
				.Max(o => o.LastUpdatedAvailableForInvestment);

			var availableForInvestment = context.UsersInCompetition
				.Where(o => o.LastUpdatedAvailableForInvestment == lastupdatedAvailableForInvestment)
				.Select(v => v.AvailableForInvestment)
				.FirstOrDefault();

			UsersInCompetition availableForInvestmentEntry = new UsersInCompetition
			{
				UserId = userId,
				CurrentValue = currentValue,
				AvailableForInvestment = availableForInvestment-(order.OrderValue),
				LastUpdatedAvailableForInvestment = DateTime.Now,
				LastUpdatedCurrentValue = lastupdatedCurrentValue,
				CompId = order.CompetitionId,
			};

			context.UsersInCompetition.Add(availableForInvestmentEntry);

			context.SaveChanges();
		}

		internal void DeleteOrder(OrderVM order)
		{

			Order orderToBeDeleted = (Order)context.Order.Where(o => o.Id == order.OrderId);
			context.Order.Remove(orderToBeDeleted);
		}

		internal void AddSellOrder(OrderVM order, int compId)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			context.Order.Add(new Order
			{
				StockId = context.StaticStockData
					.Where(s => s.Symbol == order.Symbol)
					.Select(i => i.Id)
					.FirstOrDefault(),
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
			string userId = user.GetUserId(accessor.HttpContext.User);


			var lastupdatedAvailableForInvestment = context.UsersInCompetition.
				Where(o => ((o.CompId == compId) && (o.UserId == userId)))
				.Max(o => o.LastUpdatedAvailableForInvestment);

			var availableForInvestment = (decimal)context.UsersInCompetition
				.Where(o => o.LastUpdatedAvailableForInvestment == lastupdatedAvailableForInvestment)
				.Select(v => v.AvailableForInvestment)
				.FirstOrDefault();

			var latestPriceTime = context.HistDataStocks.Where(s => s.Symbol == symbol).Max(d => d.DateTime);
			var lastPrice = (decimal)context.HistDataStocks.Where(o => ((o.Symbol == symbol) && (o.DateTime == latestPriceTime)))
						.Select(o => o.CurrentPrice)
						.FirstOrDefault();

			return new OrderVM
			{
				CompanyName = context.StaticStockData
					.Where(s => s.Symbol == symbol)
					.Select(s => s.CompanyName)
					.FirstOrDefault(),
				Symbol = symbol,
				OrderValue = 0,
				CompetitionId = compId,
				AvailableToInvest = availableForInvestment,
				LastPrice = lastPrice

			};
		}


	
		internal List<CalculatedPriceVM> GetPurchasePrice(int compId)
		{
			//tre databasanrop
			//UsersHistoricalTransactions
			//HistDataStocks
			//StaticStockData

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

				var CoDescSym = context.StaticStockData
					.Where(a => a.Id == item.StockId).Select(a => new CalculatedPriceVM
					{
						CompanyName = a.CompanyName,
						Description = a.Description,
						Symbol = a.Symbol
					});

				var companyName = CoDescSym
					.Select(a => a.CompanyName).FirstOrDefault();
				var symbol = CoDescSym
					.Select(a => a.Symbol).FirstOrDefault();


				var newHolding = new CalculatedPriceVM
				{
					PurchasePrice = purrChasePrice,
					BuyOrSell = item.BuyOrSell,
					TotalQuantity = item.CurrentHoldingsAfterTransaction,
					StockId = item.StockId,
					Quantity = item.Quantity,
					UserId = item.UserId,
					CurrentPrice = Convert.ToDecimal(price),
					TransactionPrice = item.TransactionPrice,
					CompanyName = companyName,
					Symbol = symbol
				};

				holdings.Add(newHolding);
			}

			List<CalculatedPriceVM> trimmedList = new List<CalculatedPriceVM>();

			for (int i = 0; i < holdings.Count; i++)
			{
				var calculatedExists = holdings
					.Find(a => a.StockId == holdings[i].StockId);
				if (!trimmedList.Contains(calculatedExists))
				{
					trimmedList.Add(calculatedExists);
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
						.FirstOrDefault();
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
						.FirstOrDefault();
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
					TimeOfInsertion = o.TimeOfInsertion,
					CompetitionId = compId,
					OrderId = o.Id
				});

			return orders;
		}

		internal void ExecuteOrders()
		{
			List<Order> pendingOrders = context.Order.ToList();

			foreach (var item in pendingOrders)
			{
				// get latest date
				var lastUpdated = context.HistDataStocks
						.Where(p => p.StockId == item.StockId)
						.Max(d => d.DateTime);

				// get last price
				var transactionPrice = (decimal)context.HistDataStocks
						.Where(p => ((p.StockId == item.StockId) && (p.DateTime == lastUpdated)))
						.Select(p => p.CurrentPrice)
						.FirstOrDefault();
				
				// get the current total holdings in security for user in competition
				var currentHoldings = context.UsersHistoricalTransactions
				.Where(o => o.CompetitionId == item.CompId && o.UserId == item.UserId && o.StockId == item.StockId)
				.Select(o => o.Quantity)
				.Sum();

				// calculate order quantity based on ordervalue
				var quantity = (int)Math.Round(item.OrderValue / transactionPrice);

				// Increase total quantity depending on if Buy or Sell
				if (item.BuyOrSell == "Buy")
					currentHoldings += quantity;
				else if (item.BuyOrSell == "Sell")
					currentHoldings -= quantity;


				// Create execution to be stored in table
				var order = new UsersHistoricalTransactions
				{
					UserId = item.UserId,
					CompetitionId = item.CompId,
					StockId = item.StockId,
					TransactionPrice = transactionPrice,
					DateTimeOfTransaction = DateTime.Now,
					BuyOrSell = item.BuyOrSell,
					Quantity = quantity,
					CurrentHoldingsAfterTransaction = currentHoldings												
				};

				context.UsersHistoricalTransactions.Add(order);

				// add non executed order amount to usersInCompetition availableForInvestment

				var excessOrderAmount = item.OrderValue - (quantity * transactionPrice);

				var lastupdatedCurrentValue = context.UsersInCompetition
				.Where(o => ((o.CompId == item.CompId) && (o.UserId == item.UserId)))
				.Max(o => o.LastUpdatedCurrentValue);

				var lastupdatedAvailableForInvestment = context.UsersInCompetition.
				Where(o => ((o.CompId == item.CompId) && (o.UserId == item.UserId)))
				.Max(o => o.LastUpdatedAvailableForInvestment);

				var availableForInvestment = context.UsersInCompetition
					.Where(o => o.LastUpdatedAvailableForInvestment == lastupdatedAvailableForInvestment)
					.Select(v => v.AvailableForInvestment)
					.FirstOrDefault();

				var currentValue = context.UsersInCompetition
				.Where(o => o.LastUpdatedCurrentValue == lastupdatedCurrentValue)
				.Select(v => v.CurrentValue)
				.FirstOrDefault();

				UsersInCompetition usersInCompetitionAvailableForInvestment = new UsersInCompetition
				{
					UserId = item.UserId,
					CurrentValue = currentValue,
					AvailableForInvestment = availableForInvestment + excessOrderAmount,
					LastUpdatedAvailableForInvestment = DateTime.Now,
					LastUpdatedCurrentValue = lastupdatedCurrentValue,
					CompId = item.CompId,
				};

				context.UsersInCompetition.Add(usersInCompetitionAvailableForInvestment);				

			}
			context.SaveChanges();

			foreach (var item in pendingOrders)
			{
				context.Order.Remove(item);
			}
			context.SaveChanges();
		}
	}
}
