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

namespace SustainAndGain.Models
{
    public class StocksService
    {

        private readonly IHttpClientFactory httpClientFactory;

        public StocksService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
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

