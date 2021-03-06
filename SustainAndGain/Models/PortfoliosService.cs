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

			var usersInCompetitionList = context.UsersInCompetition.ToList();

			var lastupdatedCurrentValue = GetLatestCurrentValueDate(compId, userId, usersInCompetitionList);

			Decimal? currentValue = GetLatestCurrentValue(usersInCompetitionList, lastupdatedCurrentValue);

			DateTime? lastupdatedAvailableForInvestment = GetLatestAvailableForInvestmenDate(compId, userId, usersInCompetitionList);

			decimal? availableForInvestment = GetLatestAvailableForInvestment(usersInCompetitionList, lastupdatedAvailableForInvestment);

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
		
		internal void UpdateCurrentValues()
		{
			var allEntries = context.UsersInCompetition.ToList();



			var activeUsers = allEntries
				.GroupBy(s => new { s.CompId, s.UserId });

			List<UsersInCompetition> latestEntries = new List<UsersInCompetition>();

			foreach (var grouping in activeUsers)
			{
				var lastEntry = grouping.Last();
				latestEntries.Add(lastEntry);
			}



			foreach (var item in latestEntries)
			{
				var holdings = GetHoldingsForUpdateCurrentValues(item.CompId, item.UserId);
				var marketValueHoldings = holdings
				.Select(m => m.MarketValue)
				.Sum();

				var entry = new UsersInCompetition
				{
					CurrentValue = marketValueHoldings + (decimal)item.AvailableForInvestment,
					CompId = item.CompId,
					AvailableForInvestment = item.AvailableForInvestment,
					UserId = item.UserId,
					LastUpdatedCurrentValue = DateTime.Now,
					LastUpdatedAvailableForInvestment = item.LastUpdatedAvailableForInvestment
				};


				context.UsersInCompetition.Add(entry);
			}

			context.SaveChanges();
		}

		private static decimal? GetLatestAvailableForInvestment(List<UsersInCompetition> usersInCompetitionList, DateTime? lastupdatedAvailableForInvestment)
		{
			return usersInCompetitionList
				.Where(o => o.LastUpdatedAvailableForInvestment == lastupdatedAvailableForInvestment)
				.Select(v => v.AvailableForInvestment)
				.FirstOrDefault();
		}

		internal decimal GetSustainablePercentage(int compId)
		{
			var holdings = GetHoldings(compId);
			var marketValueInSustainableStocks = holdings
				.Where(s => s.IsSustainable == true)
				.Select(m => m.MarketValue)
				.Sum();

			string userId = user.GetUserId(accessor.HttpContext.User);

			var usersInCompetitionList = context.UsersInCompetition.ToList();

			int lastupdatedCurrentValue = GetLatestCurrentValueDate(compId, userId, usersInCompetitionList);

			Decimal? currentValue = GetLatestCurrentValue(usersInCompetitionList, lastupdatedCurrentValue);

			var sustainablePercentage = marketValueInSustainableStocks / (decimal)currentValue;

			return sustainablePercentage;

		}

		private static DateTime? GetLatestAvailableForInvestmenDate(int compId, string userId, List<UsersInCompetition> usersInCompetitionList)
		{
			return usersInCompetitionList
				.Where(o => ((o.CompId == compId) && (o.UserId == userId)))
				.Max(o => o.LastUpdatedAvailableForInvestment);
		}

		private static decimal? GetLatestCurrentValue(List<UsersInCompetition> usersInCompetitionList, int lastupdatedCurrentValue)
		{
			return usersInCompetitionList
				.Where(o => o.Id == lastupdatedCurrentValue)
				.Select(v => v.CurrentValue)
				.FirstOrDefault();
		}

		private static int GetLatestCurrentValueDate(int compId, string userId, List<UsersInCompetition> usersInCompetitionList)
		{
			return usersInCompetitionList
				.Where(o => ((o.CompId == compId) && (o.UserId == userId)))
				.Select(i => i.Id)
				.Last();
				
		}

		internal HighscoreVM[] GetHighScoreForCompetition(int compId)
		{
			var maxHighScore = context.UsersInCompetition.ToList();

			var listOfUsers = context.AspNetUsers.ToList();

			var groupedHighScoreList = maxHighScore	
				.Where(a => a.CurrentValue > 0 && a.CompId == compId)
				.GroupBy(u => u.UserId);

			var listOfHighScores = new List<HighscoreVM>();

			foreach (var item in groupedHighScoreList)
			{
				var score = item.Last();

				listOfHighScores.Add(new HighscoreVM
				{
					CurrentValue = score.CurrentValue,
					User = score.User
				});
			
			}			
			var arrayOfHighScoreVM = listOfHighScores.OrderByDescending(s => s.CurrentValue).Take(10).ToArray();

			int counter = 1;
			for (int i = 0; i < arrayOfHighScoreVM.Length; i++)
			{
				arrayOfHighScoreVM[i].Nr = counter++;
			}

			return arrayOfHighScoreVM;
		}

		internal decimal CheckTotalAvailableToInvestForStockBuy(OrderVM order)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			var usersInCompetitionList = context.UsersInCompetition.ToList();

			DateTime? lastupdatedAvailableForInvestment = GetLatestAvailableForInvestmenDate(order.CompetitionId, userId, usersInCompetitionList);

			decimal? availableForInvestment = GetLatestAvailableForInvestment(usersInCompetitionList, lastupdatedAvailableForInvestment);


			return (decimal)availableForInvestment;
		}

		internal int CheckTotalHoldingsForStockSell(SellOrderVM order)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			var holding = context.UsersHistoricalTransactions
				.Where(o => o.CompetitionId == order.CompetitionId && o.UserId == userId && o.StockId == order.StockId)
				.Select(o => o.Quantity)
				.Sum();

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

			var usersInCompetitionList = context.UsersInCompetition.ToList();

			int lastupdatedCurrentValueId = GetLatestCurrentValueDate(order.CompetitionId, userId, usersInCompetitionList);

			Decimal? currentValue = GetLatestCurrentValue(usersInCompetitionList, lastupdatedCurrentValueId);

			DateTime? lastupdatedAvailableForInvestment = GetLatestAvailableForInvestmenDate(order.CompetitionId, userId, usersInCompetitionList);

			decimal? availableForInvestment = GetLatestAvailableForInvestment(usersInCompetitionList, lastupdatedAvailableForInvestment);

			DateTime latestDateTimeCurrentValue = (DateTime)usersInCompetitionList
				.Where(i => i.Id == lastupdatedCurrentValueId)
				.Select(d => d.LastUpdatedCurrentValue)
				.FirstOrDefault();

			UsersInCompetition availableForInvestmentEntry = new UsersInCompetition
			{
				UserId = userId,
				CurrentValue = currentValue,
				AvailableForInvestment = availableForInvestment - (order.OrderValue),
				LastUpdatedAvailableForInvestment = DateTime.Now,
				LastUpdatedCurrentValue = latestDateTimeCurrentValue,
				CompId = order.CompetitionId,				
			};

			context.UsersInCompetition.Add(availableForInvestmentEntry);

			context.SaveChanges();
		}

		internal SellOrderVM GetOrderEntrySell(string symbol, int compId, int stockId)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			var usersInCompetitionList = context.UsersInCompetition.ToList();

			DateTime? lastupdatedAvailableForInvestment = GetLatestAvailableForInvestmenDate(compId, userId, usersInCompetitionList);

			decimal? availableForInvestment = GetLatestAvailableForInvestment(usersInCompetitionList, lastupdatedAvailableForInvestment);

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
				StockId = stockId

			};
		}

		internal Order DeleteOrder(int orderId)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);


			Order orderToBeDeleted = context.Order.Where(o => o.Id == orderId).FirstOrDefault();

			context.Order.Remove(orderToBeDeleted);

			var usersInCompetitionList = context.UsersInCompetition.ToList();

			// Add entry into usersInComp with updated availableForInvestment value to remove reserved cash

			if (orderToBeDeleted.BuyOrSell == "Buy")
			{
				int lastupdatedCurrentValueId = GetLatestCurrentValueDate(orderToBeDeleted.CompId, userId, usersInCompetitionList);

				Decimal? currentValue = GetLatestCurrentValue(usersInCompetitionList, lastupdatedCurrentValueId);

				DateTime? lastupdatedAvailableForInvestment = GetLatestAvailableForInvestmenDate(orderToBeDeleted.CompId, userId, usersInCompetitionList);

				decimal? availableForInvestment = GetLatestAvailableForInvestment(usersInCompetitionList, lastupdatedAvailableForInvestment);

				DateTime latestDateTimeCurrentValue = (DateTime)usersInCompetitionList
				.Where(i => i.Id == lastupdatedCurrentValueId)
				.Select(d => d.LastUpdatedCurrentValue)
				.FirstOrDefault();

				UsersInCompetition availableForInvestmentEntry = new UsersInCompetition
				{
					UserId = userId,
					CurrentValue = currentValue,
					AvailableForInvestment = availableForInvestment + (orderToBeDeleted.OrderValue),
					LastUpdatedAvailableForInvestment = DateTime.Now,
					LastUpdatedCurrentValue = latestDateTimeCurrentValue,
					CompId = orderToBeDeleted.CompId,
				};

				context.UsersInCompetition.Add(availableForInvestmentEntry);
			}

			context.SaveChanges();

			return orderToBeDeleted;
		}

		internal void AddSellOrder(SellOrderVM order)
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

			var usersInCompetitionList = context.UsersInCompetition.ToList();

			DateTime? lastupdatedAvailableForInvestment = GetLatestAvailableForInvestmenDate(compId, userId, usersInCompetitionList);

			decimal? availableForInvestment = GetLatestAvailableForInvestment(usersInCompetitionList, lastupdatedAvailableForInvestment);

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
				AvailableToInvest = (decimal)availableForInvestment,
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
					AveragePriceForCurrentHoldings = c.AveragePriceForCurrentHoldings,					
					
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
						IsSustainable = context.StaticStockData.Where(s => s.Id == item.StockId).Select(s => s.IsSustainable).SingleOrDefault(),
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
					TotalQuantity = o.Quantity
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

		internal List<HoldingsVM> GetHoldingsForUpdateCurrentValues(int compId, string userId)
		{

			var holdings = context.UsersHistoricalTransactions
				.Where(o => o.CompetitionId == compId && o.UserId == userId)
				.Select(o => new HoldingsVM
				{
					Symbol = context.StaticStockData.Where(s => s.Id == o.StockId).Select(s => s.Symbol).SingleOrDefault(),
					CompanyName = context.StaticStockData.Where(s => s.Id == o.StockId).Select(s => s.CompanyName).SingleOrDefault(),
					IsSustainable = context.StaticStockData.Where(s => s.Id == o.StockId).Select(s => s.IsSustainable).SingleOrDefault(),
					BuyOrSell = o.BuyOrSell,
					TotalQuantity = o.Quantity
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
					Symbol = context.StaticStockData.Where(s => s.Id == o.StockId).Select(s => s.Symbol).FirstOrDefault(),
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

					// If previous entry exists in table for this user in this competition, execute below if statement, else ignore


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

				int lastupdatedCurrentValueId = GetLatestCurrentValueDate(item.CompId, item.UserId, listOfUsersInCompetition);

				Decimal? currentValue = GetLatestCurrentValue(listOfUsersInCompetition, lastupdatedCurrentValueId);

				DateTime? lastupdatedAvailableForInvestment = GetLatestAvailableForInvestmenDate(item.CompId, item.UserId, listOfUsersInCompetition);

				decimal? availableForInvestment = GetLatestAvailableForInvestment(listOfUsersInCompetition, lastupdatedAvailableForInvestment);

				DateTime latestDateTimeCurrentValue = (DateTime)listOfUsersInCompetition
				.Where(i => i.Id == lastupdatedCurrentValueId)
				.Select(d => d.LastUpdatedCurrentValue)
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
						LastUpdatedCurrentValue = latestDateTimeCurrentValue,
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
						LastUpdatedCurrentValue = latestDateTimeCurrentValue,
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
