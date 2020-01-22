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

namespace SustainAndGain.Models
{
	public class StocksService
	{
		private SustainGainContext context;

		public StocksService(SustainGainContext context)
		{
			this.context = context;
		}

        public void AddHistDataStocks()
        {
            string url = "https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/get-quotes?region=ST&lang=en&symbols=";

            var result = context.StaticStockData
               .Take(99)
               .Select(g => g.Symbol)
               .ToArray();

			foreach (var item in result)
			{
				url = url + item + ",";

            }
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "apidojo-yahoo-finance-v1.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "f8544aa2bamshc436653380b874cp1efcc0jsn3741bd4318a2");
            IRestResponse response = client.Execute(request);
            string test = response.Content;

            var rootObject = JsonConvert.DeserializeObject<RootObject>(response.Content);

            foreach (var item in rootObject.quoteResponse.result)
            {
                StaticStockData stock = context.StaticStockData.SingleOrDefault(s => s.Symbol == item.symbol);

                HistDataStocks historicalDataForStock = new HistDataStocks { DateTime = DateTime.Now, CurrentPrice = item.regularMarketPrice, StockId = stock.Id  };

                context.HistDataStocks.Add(historicalDataForStock);
                context.SaveChanges();
            }


			var result2 = context.StaticStockData
				.Skip(99)
			   .Take(99)
			   .Select(g => g.Symbol)
			   .ToArray();

			var result3 = context.StaticStockData
				   .Skip(198)
				   .Take(99)
				   .Select(g => g.Symbol)
				   .ToArray();


			var result4 = context.StaticStockData
				  .Skip(297)
				  .Take(99)
				  .Select(g => g.Symbol)
				   .ToArray();


			var result5 = context.StaticStockData
				  .Skip(396)
				  .Take(99)
				  .Select(g => g.Symbol)
				   .ToArray();


			var result6 = context.StaticStockData
				.Skip(495)
				.Take(99)
				.Select(g => g.Symbol)
				   .ToArray();


			var result7 = context.StaticStockData
				.Skip(594)
				.Take(99)
				.Select(g => g.Symbol)
				   .ToArray();


			var result8 = context.StaticStockData
				.Skip(693)
				.Take(8)
				.Select(g => g.Symbol)
				   .ToArray();







			// Reverted back

		}

		public void AddStaticStockData()
		{
			string path = @"C:\Users\Daniel\source\repos\SustainAndGain\SustainAndGain\Models\yahoo.txt";

			string[] inputFileStocks = File.ReadAllLines(path);


			foreach (var item in inputFileStocks)
			{

				string[] lines = item.Split('\t');

				string symbol = lines[0];
				string companyName = lines[1];

				StaticStockData staticStockData = new StaticStockData { Symbol = symbol, CompanyName = companyName};


				//int firstPositionSymbol = 0;
				//int endPositionSymbol = item.IndexOf(".ST") + 3;
				//string symbol = item.Substring(firstPositionSymbol, endPositionSymbol);



				//Assigning values to DB model and saving to DB
				//staticStockData.Symbol = staticStockData.Symbol.ToUpper();
				//staticStockData.CompanyName = staticStockData.CompanyName;

				//staticStockData.Description = staticStockData.Description;
				//staticStockData.Sector = staticStockData.Sector;

				context.StaticStockData.Add(staticStockData);
				context.SaveChanges();

			}

		}


		public HistDataStocks GetResultAsync()
		{
			var client = new RestClient("https://morning-star.p.rapidapi.com/market/auto-complete?query=nasdaq");
			var request = new RestRequest(Method.GET);
			request.AddHeader("x-rapidapi-host", "morning-star.p.rapidapi.com");
			request.AddHeader("x-rapidapi-key", "f8544aa2bamshc436653380b874cp1efcc0jsn3741bd4318a2");
			IRestResponse response = client.Execute(request);
			string result = response.Content;
			var stocks = JsonConvert.DeserializeObject<HistDataStocks>(response.Content);
			return stocks;
		}

		public void GetCompanyDescription()
		{
			Encoding unicode = Encoding.UTF7;
			string descriptionFile = @"C:/Users/Daniel/Documents/SustainAndGain/testStockTickers.txt";

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





	}

}

