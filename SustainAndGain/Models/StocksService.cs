using Newtonsoft.Json;
using RestSharp;
using SustainAndGain.Models.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.Web;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using SustainAndGain.Models.Entities;

using System.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SustainAndGain.Models
{
	public class StocksService
	{

		private SustainGainContext context;
		private readonly UserManager<MyIdentityUser> user;
		private readonly IHttpContextAccessor accessor;
		private readonly IConfiguration configuration;

		public StocksService(SustainGainContext context, UserManager<MyIdentityUser> user, IHttpContextAccessor accessor, IConfiguration configuration)
		{
			this.context = context;
			this.user = user;
			this.accessor = accessor;
			this.configuration = configuration;
		}


		public void AddHistDataStocks()
		{
			var stockData = context.StaticStockData
					.Select(g => g.Symbol)
					.ToArray();

			for (int i = 0; i < 8; i++)
			{
				GetPricesForStocks(stockData, i);
				Thread.Sleep(800);
			}

			context.SaveChanges();
		}


		private void GetPricesForStocks(string[] stockData, int i)
		{
			var result = stockData
				.Skip(i * 99)
			   .Take(99)
			   .ToArray();

			// Construct URL with max 99 stocks and performs call to Yahoo API
			var response = ConstructURLWithStocksAndGetStockInfoFromYahoo(result);

			var rootObject = JsonConvert.DeserializeObject<RootObject>(response.Content);

			// Uses the Rootobject result list to find correct symbol and write to HistDataSTocks
			WriteStockInfoToHistoricalDataStocks(rootObject);
		}


		//internal bool AddUsersInComp(CompetitionVM data)
		//{
		//	UsersInCompetition stocks = new UsersInCompetition
		//	{
		//		UserId = data.UserId,
		//		CurrentValue = 10000,
		//		AvailableForInvestment = 10000,
		//		LastUpdatedAvailableForInvestment = DateTime.Now,
		//		LastUpdatedCurrentValue = DateTime.Now,
		//		CompId = int.Parse(data.CompId),
		//	};

		//	BonusDeposit deposit = new BonusDeposit
		//	{
		//		UserId = data.UserId,
		//		CompetitionId = int.Parse(data.CompId),
		//		Bonus = 0,
		//	};

		//	context.BonusDeposit.Add(deposit);
		//	context.UsersInCompetition.Add(stocks);
		//	context.SaveChanges();

		//	return true;
		//}


		public void AddSustainProp()
		{
			string sustainPath = @"C:\Users\Abdi G\source\repos\SustainAndGain\SustainAndGain\wwwroot\SustainBolag.txt";
			string[] sustainList = File.ReadAllLines(sustainPath);
			var stockList = context.StaticStockData.ToList();
			
			foreach (var item in sustainList)
			{
				string[] lines = item.Split('.');
				var sustainStock = stockList
					.Find(s => s.Symbol.ToLower().Trim().Contains(item.ToLower().Trim()[0]));
				if (sustainStock == null)
				{

				}
				else
					sustainStock.IsSustainable = true;
			}
			context.SaveChanges();
		}


			public void AddStaticStockData()
			{
				string path = @"C:\Users\abdig\source\repos\SustainAndGain2.0\SustainAndGain\Models\yahoo.txt";

				string[] inputFileStocks = File.ReadAllLines(path);

				foreach (var item in inputFileStocks)
				{
					string[] lines = item.Split('\t');

					string symbol = lines[0];
					string companyName = lines[1];

					StaticStockData staticStockData = new StaticStockData { Symbol = symbol, CompanyName = companyName };					

					context.StaticStockData.Add(staticStockData);
				}
				context.SaveChanges();
			}		


		public void GetCompanyDescription()
		{
			Encoding unicode = Encoding.UTF7;
			string descriptionFile = @"C:\Users\abdig\source\repos\SustainAndGain2.0\SustainAndGain\Models\testStockTickers.json";

			var companies = JsonConvert.DeserializeObject<DescriptionClass[]>(File.ReadAllText(descriptionFile, unicode));
			var listOfCompanies = companies.Select(a => new DescriptionClass
			{
				description = a.description,
				company = a.company + ".ST"
			}).ToArray();

			foreach (var dbItem in context.StaticStockData)
			{
				foreach (var companyItem in listOfCompanies)
				{
					//companyItem.company.Where(a => a.Equals(dbItem.Symbol + ".ST"));
					if (dbItem.Symbol == companyItem.company)
					{
						dbItem.Description = companyItem.description;
					}
				}
			}
			context.SaveChanges();
		}


		public void WriteStockInfoToHistoricalDataStocks(RootObject rootObject)
		{
			foreach (var item in rootObject.quoteResponse.result)
			{
				StaticStockData stock = context.StaticStockData.SingleOrDefault(s => s.Symbol == item.symbol);

				HistDataStocks historicalDataForStock = new HistDataStocks { DateTime = DateTime.Now, CurrentPrice = item.regularMarketPrice, StockId = stock.Id, Symbol = item.symbol };

				context.HistDataStocks.Add(historicalDataForStock);
			}
		}


		public IRestResponse ConstructURLWithStocksAndGetStockInfoFromYahoo(string[] result)
		{
			string url = "https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/get-quotes?region=ST&lang=en&symbols=";

			foreach (var item in result)
			{
				url = url + item + ",";
			}
			var client = new RestClient(url);
			var request = new RestRequest(Method.GET);
			request.AddHeader("x-rapidapi-host", "apidojo-yahoo-finance-v1.p.rapidapi.com");
			request.AddHeader("x-rapidapi-key", configuration["x-rapidapi-key"]);
			IRestResponse response = client.Execute(request);
			return response;
		}


		public List<UsersInCompetition> GetHistoricalTransactionData(int compId)
		{
			string userId = user.GetUserId(accessor.HttpContext.User);

			List<UsersInCompetition> historicalTransactions = new List<UsersInCompetition>();

			foreach (var transactionData in context.UsersInCompetition)
			{
				if (transactionData.UserId == userId && transactionData.CompId == compId)
				{
					UsersInCompetition transactions = new UsersInCompetition
					{
						CurrentValue = transactionData.CurrentValue,
						LastUpdatedCurrentValue = transactionData.LastUpdatedCurrentValue,
						AvailableForInvestment = transactionData.AvailableForInvestment,
						CompId = transactionData.CompId,
						Id = transactionData.Id,
						LastUpdatedAvailableForInvestment = transactionData.LastUpdatedAvailableForInvestment,
						UserId = transactionData.UserId
					};
					historicalTransactions.Add(transactions);
				}
			}
			return historicalTransactions;
		}
	}
}

