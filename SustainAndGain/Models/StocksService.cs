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

        public void AddStaticStockData()
        {
            string path = @"C:\Users\Abdi G\Desktop\LargeMidSmallSwedenYahooorg.txt";

            string[] inputFileStocks = File.ReadAllLines(path);


            foreach (var item in inputFileStocks)
            {
                
                string[] lines = item.Split('\t');

                string symbol = lines[0];
                string companyName = lines[1];

                StaticStockData staticStockData = new StaticStockData { Symbol = symbol, CompanyName = companyName };


                //int firstPositionSymbol = 0;
                //int endPositionSymbol = item.IndexOf(".ST") + 3;
                //string symbol = item.Substring(firstPositionSymbol, endPositionSymbol);

            

            //Assigning values to DB model and saving to DB
            //staticStockData.Symbol = staticStockData.Symbol.ToUpper();
            //staticStockData.CompanyName = staticStockData.CompanyName;

            //staticStockData.Description = staticStockData.Description;
            //staticStockData.Sector = staticStockData.Sector;

            context.StaticStockData.Add(staticStockData);
            }
            context.SaveChanges();
        }


        public StocksIndexVM GetResultAsync()
        {
            var client = new RestClient("https://morning-star.p.rapidapi.com/market/auto-complete?query=nasdaq");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "morning-star.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "f8544aa2bamshc436653380b874cp1efcc0jsn3741bd4318a2");
            IRestResponse response = client.Execute(request);

            var photos = JsonConvert.DeserializeObject<StocksIndexVM>(response.Content);
            return photos;
        }

    }

}

