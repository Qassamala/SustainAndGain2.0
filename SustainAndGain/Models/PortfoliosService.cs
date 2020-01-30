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
		//internal object GetSustainProcent(int compId)
		//{

		//}

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

		internal void UpdateCurrentValuesAbdi()
		{
			var holdings = context.UsersHistoricalTransactions
				.OrderBy(c => c.CompetitionId)
				.ThenBy(u => u.UserId)
				.ThenBy(s => s.StockId)
				.Select(h => h.CurrentHoldingsAfterTransaction)
				.ToList();

			var trimmedHoldings = context.UsersHistoricalTransactions;

			//foreach (var item in holdings)
			//{
				
			//}

		}

		internal void UpdateCurrentValue(int compId)
		{

			var allStocks = context.HistDataStocks.ToList();
			var userHistoricalTransaction = context.UsersHistoricalTransactions.ToList();
			var usersInCompetition = context.UsersInCompetition.ToList();

			string userId = user.GetUserId(accessor.HttpContext.User);

			List<decimal> Value = new List<decimal>();
			List<HistDataStocks> newPricesList = new List<HistDataStocks>();


			foreach (var item in allStocks)
			{
				var updatedPrice = allStocks
				.Where(a => a.StockId == item.StockId).Last();

				if (!newPricesList.Contains(updatedPrice))
				{
					newPricesList.Add(updatedPrice);
				}
			}


			var userInComp = usersInCompetition
				.Where(a => a.UserId == userId && compId == a.CompId).Last();

			
				var eachPrice = userHistoricalTransaction
					.Where(a => a.UserId == userInComp.UserId && a.CompetitionId == compId).ToList();

				foreach (var price in eachPrice)
				{
					var vs = newPricesList
						.Where(a => a.StockId == price.StockId)
						.Select(a => a.CurrentPrice * price.CurrentHoldingsAfterTransaction).Last();


				Value.Add((decimal)vs);
				}
				//.Select(a => item.CurrentPrice * a.CurrentHoldingsAfterTransaction).Last();


			var CurrentValue = Value.Sum();



			var newuserincomp = new UsersInCompetition
			{
				CurrentValue = CurrentValue + userInComp.AvailableForInvestment,
				CompId = userInComp.CompId,
				AvailableForInvestment = userInComp.AvailableForInvestment,
				UserId = userInComp.UserId,
				LastUpdatedCurrentValue = DateTime.Now,
				LastUpdatedAvailableForInvestment = userInComp.LastUpdatedAvailableForInvestment



			};


			context.UsersInCompetition.Add(newuserincomp);

			context.SaveChanges();


		}

		internal int CheckTotalHoldings(OrderVM order)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			var holding = context.UsersHistoricalTransactions
				.Where(o => o.CompetitionId == order.CompetitionId && o.UserId == userId && o.StockId == order.StockId)
				.Select(o => o.CurrentHoldingsAfterTransaction)
				.Last();

			return holding;
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
				AvailableForInvestment = availableForInvestment - (order.OrderValue),
				LastUpdatedAvailableForInvestment = DateTime.Now,
				LastUpdatedCurrentValue = lastupdatedCurrentValue,
				CompId = order.CompetitionId,
				
			};

			context.UsersInCompetition.Add(availableForInvestmentEntry);

			context.SaveChanges();
		}

		internal SellOrderVM GetOrderEntrySell(string symbol, int compId)
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

			return new SellOrderVM
			{
				CompanyName = context.StaticStockData
					.Where(s => s.Symbol == symbol)
					.Select(s => s.CompanyName)
					.FirstOrDefault(),
				Symbol = symbol,
				CompetitionId = compId,
				Quantity = 0,

			};
		}

		internal Order DeleteOrder(int orderId)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);


			Order orderToBeDeleted = context.Order.Where(o => o.Id == orderId).FirstOrDefault();

			context.Order.Remove(orderToBeDeleted);

			// Add entry into usersInComp with updated availableForInvestment value to remove reserved cash

			if (orderToBeDeleted.BuyOrSell == "Buy")
			{
				var lastupdatedCurrentValue = context.UsersInCompetition
					.Where(o => ((o.CompId == orderToBeDeleted.CompId) && (o.UserId == userId)))
					.Max(o => o.LastUpdatedCurrentValue);

				var currentValue = context.UsersInCompetition
					.Where(o => o.LastUpdatedCurrentValue == lastupdatedCurrentValue)
					.Select(v => v.CurrentValue)
					.FirstOrDefault();

				var lastupdatedAvailableForInvestment = context.UsersInCompetition.
					Where(o => ((o.CompId == orderToBeDeleted.CompId) && (o.UserId == userId)))
					.Max(o => o.LastUpdatedAvailableForInvestment);

				var availableForInvestment = context.UsersInCompetition
					.Where(o => o.LastUpdatedAvailableForInvestment == lastupdatedAvailableForInvestment)
					.Select(v => v.AvailableForInvestment)
					.FirstOrDefault();

				UsersInCompetition availableForInvestmentEntry = new UsersInCompetition
				{
					UserId = userId,
					CurrentValue = currentValue,
					AvailableForInvestment = availableForInvestment + (orderToBeDeleted.OrderValue),
					LastUpdatedAvailableForInvestment = DateTime.Now,
					LastUpdatedCurrentValue = lastupdatedCurrentValue,
					CompId = orderToBeDeleted.CompId,
				};

				context.UsersInCompetition.Add(availableForInvestmentEntry);
			}

			context.SaveChanges();

			return orderToBeDeleted;
		}

		internal void AddSellOrder(OrderVM order)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			context.Order.Add(new Order
			{
				StockId = context.StaticStockData
					.Where(s => s.Symbol == order.Symbol)
					.Select(i => i.Id)
					.FirstOrDefault(),
				TimeOfInsertion = DateTime.Now,
				BuyOrSell = "Sell",
				UserId = userId,
				CompId = order.CompetitionId,
				Quantity = order.Quantity
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
			string userId = user.GetUserId(accessor.HttpContext.User);

			List<CalculatedPriceVM> holdings = new List<CalculatedPriceVM>();

			var userHoldings = context.UsersHistoricalTransactions
				.Where(c => c.CompetitionId == compId && c.UserId == userId && c.BuyOrSell == "Buy")
				.Select(c => new UsersHistoricalTransactions
				{
					StockId = c.StockId,
					TransactionPrice = c.TransactionPrice,
					BuyOrSell = c.BuyOrSell,
					UserId = c.UserId,
					DateTimeOfTransaction = c.DateTimeOfTransaction,
					CurrentHoldingsAfterTransaction = c.CurrentHoldingsAfterTransaction,
					CompetitionId = c.CompetitionId,
					Quantity = c.Quantity,
					CurrentPurchaseAmountForHoldings = c.CurrentPurchaseAmountForHoldings,
					AveragePriceForCurrentHoldings = c.AveragePriceForCurrentHoldings
				
					
				}).ToList();


			var hisdatastocks = context.HistDataStocks
					.Where(a => a.StockId == a.StockId).ToList();


			foreach (var item in userHoldings)
			{
				if (item.BuyOrSell == "Buy")
				{

					var latestPriceDate = context.HistDataStocks
							.Where(o => ((o.StockId == item.StockId))).Max(o => o.DateTime);

					var currentPrice = (decimal)context.HistDataStocks
							.Where(o => ((o.StockId == item.StockId) && (o.DateTime == latestPriceDate)))
							.Select(o => o.CurrentPrice)
							.FirstOrDefault();

					var compDescSymb = context.StaticStockData
						.Where(a => a.Id == item.StockId).Select(a => new CalculatedPriceVM
						{
							CompanyName = a.CompanyName,
							Description = a.Description,
							Symbol = a.Symbol,

						});

					var companyName = compDescSymb
						.Select(a => a.CompanyName).FirstOrDefault();
					var symbol = compDescSymb
						.Select(a => a.Symbol).FirstOrDefault();


					var newHolding = new CalculatedPriceVM
					{
						PurchasePrice = item.AveragePriceForCurrentHoldings,
						BuyOrSell = item.BuyOrSell,
						TotalQuantity = context.UsersHistoricalTransactions
						.Where(o => o.CompetitionId == item.CompetitionId && o.UserId == item.UserId && o.StockId == item.StockId)
						.Select(o => o.Quantity)
						.Sum(),
						StockId = item.StockId,
						Quantity = item.Quantity,
						UserId = item.UserId,
						CurrentPrice = currentPrice,
						TransactionPrice = item.TransactionPrice,
						CompanyName = companyName,
						Symbol = symbol,
						CompetitionId = compId,
					};
					if (newHolding.TotalQuantity > 0)
					{
					holdings.Add(newHolding);

					}
				}
				

			}

			List<CalculatedPriceVM> trimmedList = new List<CalculatedPriceVM>();

			for (int i = 0; i < holdings.Count; i++)
			{
				var calculatedExists = holdings
					.Where(a => a.StockId == holdings[i].StockId).Last();

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
				var latestPriceDate = context.HistDataStocks
						.Where(o => ((o.Symbol == item.Symbol))).Max(o => o.DateTime);

				item.CurrentPrice = (decimal)context.HistDataStocks
						.Where(o => ((o.Symbol == item.Symbol) && (o.DateTime == latestPriceDate)))
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
					OrderValue = (decimal)o.OrderValue,
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

			var transactions = context.UsersHistoricalTransactions;

			foreach (var item in pendingOrders)
			{
				var usersTransactionsInComp = transactions
					.Where(o => o.CompetitionId == item.CompId && o.UserId == item.UserId && o.StockId == item.StockId);

				var listOfHisDataStocks = context.HistDataStocks.ToList();

				// get latest date
				var lastUpdated = listOfHisDataStocks
						.Where(p => p.StockId == item.StockId)
						.Max(d => d.DateTime);

				// get last price
				var transactionPrice = (decimal)listOfHisDataStocks
						.Where(p => ((p.StockId == item.StockId) && (p.DateTime == lastUpdated)))
						.Select(p => p.CurrentPrice)
						.FirstOrDefault();

				// get the current total holdings in security for user in competition
				var currentHoldings = context.UsersHistoricalTransactions
					.Where(o => o.CompetitionId == item.CompId && o.UserId == item.UserId && o.StockId == item.StockId)
					.Select(o => o.Quantity)
					.Sum();

				int quantity = 0;

				if (item.BuyOrSell == "Buy")
				{
					quantity = (int)Math.Round((decimal)item.OrderValue / transactionPrice);

				}

				// calculate order quantity based on ordervalue ( For buy orders only)

				// Increase total quantity depending on if Buy or Sell
				if (item.BuyOrSell == "Buy")
					currentHoldings += quantity;
				else if (item.BuyOrSell == "Sell")
					currentHoldings -= (int)item.Quantity; // Sell quantity is entered by the user

				if (item.BuyOrSell == "Buy")
				{
					//// checking previous transactions for calculation of average price
					//var usersTransactionsInComp = transactions
					//.Where(o => o.CompetitionId == item.CompId && o.UserId == item.UserId && o.StockId == item.StockId);

					// If previous entry exists in table for this user in this competition, execute below if statement, else ignore

					//var usersTransactionsInComp = transactions.Where(t => t.UserId == item.UserId && t.CompetitionId == item.CompId && t.StockId == item.StockId)

					decimal lastPurchaseAmount = 0;

					if (usersTransactionsInComp.Any())
					{
						var lastUpdatedPurchaseAmountDate = usersTransactionsInComp
							.Where(o => o.CompetitionId == item.CompId && o.UserId == item.UserId && o.StockId == item.StockId)
							.Max(o => o.DateTimeOfTransaction);

							lastPurchaseAmount = usersTransactionsInComp
							.Where(o => o.CompetitionId == item.CompId && o.UserId == item.UserId && o.StockId == item.StockId && o.DateTimeOfTransaction == lastUpdatedPurchaseAmountDate)
							.Select(p => p.CurrentPurchaseAmountForHoldings)
							.FirstOrDefault();
					}

					var purchaseAmount = lastPurchaseAmount += (quantity* transactionPrice);



					// Create buy execution to be stored in table
					var order = new UsersHistoricalTransactions
					{
						UserId = item.UserId,
						CompetitionId = item.CompId,
						StockId = item.StockId,
						TransactionPrice = transactionPrice,
						DateTimeOfTransaction = DateTime.Now,
						BuyOrSell = item.BuyOrSell,
						Quantity = quantity,
						CurrentHoldingsAfterTransaction = currentHoldings,
						AveragePriceForCurrentHoldings = purchaseAmount/currentHoldings,
						CurrentPurchaseAmountForHoldings = purchaseAmount

					};
					context.UsersHistoricalTransactions.Add(order);
				}
				else if (item.BuyOrSell == "Sell")
				{

					var lastUpdatedAvgPriceDate = usersTransactionsInComp
					.Where(o => o.CompetitionId == item.CompId && o.UserId == item.UserId && o.StockId == item.StockId)
					.Max(o => o.DateTimeOfTransaction);

					var lastaveragePrice = usersTransactionsInComp
						.Where(o => o.CompetitionId == item.CompId && o.UserId == item.UserId && o.StockId == item.StockId && o.DateTimeOfTransaction == lastUpdatedAvgPriceDate)
						.Select(p => p.AveragePriceForCurrentHoldings)
						.FirstOrDefault();

					var purchaseAmount = lastaveragePrice * currentHoldings;

					var order = new UsersHistoricalTransactions
					{
						UserId = item.UserId,
						CompetitionId = item.CompId,
						StockId = item.StockId,
						TransactionPrice = transactionPrice,
						DateTimeOfTransaction = DateTime.Now,
						BuyOrSell = item.BuyOrSell,
						Quantity = -(int)item.Quantity,
						CurrentHoldingsAfterTransaction = currentHoldings,
						CurrentPurchaseAmountForHoldings = purchaseAmount
					};
					if (currentHoldings == 0) // Whenever sell results in total quantity 0, this executes as attempted to divide by zero would result in exception
					{
						order.AveragePriceForCurrentHoldings = 0;
					}
					else
					{
						order.AveragePriceForCurrentHoldings = purchaseAmount / (currentHoldings);
					}
					context.UsersHistoricalTransactions.Add(order);
				}

				var listOfUsersInCompetition = context.UsersInCompetition.ToList();

				var lastupdatedCurrentValue = listOfUsersInCompetition
					.Where(o => ((o.CompId == item.CompId) && (o.UserId == item.UserId)))
					.Max(o => o.LastUpdatedCurrentValue);

				var lastupdatedAvailableForInvestment = listOfUsersInCompetition
					.Where(o => ((o.CompId == item.CompId) && (o.UserId == item.UserId)))
					.Max(o => o.LastUpdatedAvailableForInvestment);

				var availableForInvestment = listOfUsersInCompetition
					.Where(o => o.LastUpdatedAvailableForInvestment == lastupdatedAvailableForInvestment)
					.Select(v => v.AvailableForInvestment)
					.FirstOrDefault();

				var currentValue = listOfUsersInCompetition
					.Where(o => o.LastUpdatedCurrentValue == lastupdatedCurrentValue)
					.Select(v => v.CurrentValue)
					.FirstOrDefault();


				if (item.BuyOrSell == "Buy")
				{

					var excessOrderAmount = (decimal)(item.OrderValue) - (quantity * transactionPrice);

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
				else if (item.BuyOrSell == "Sell")
				{
					UsersInCompetition usersInCompetitionAvailableForInvestment = new UsersInCompetition
					{
						UserId = item.UserId,
						CurrentValue = currentValue,
						AvailableForInvestment = availableForInvestment + (item.Quantity * transactionPrice),
						LastUpdatedAvailableForInvestment = DateTime.Now,
						LastUpdatedCurrentValue = lastupdatedCurrentValue,
						CompId = item.CompId,
					};
					context.UsersInCompetition.Add(usersInCompetitionAvailableForInvestment);
				}

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
