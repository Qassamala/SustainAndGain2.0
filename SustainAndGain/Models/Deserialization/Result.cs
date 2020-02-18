using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models
{
    public class Result
    {
        public string language { get; set; }
        public string region { get; set; }
        public string quoteType { get; set; }
        public double preMarketChange { get; set; }
        public double preMarketChangePercent { get; set; }
        public int preMarketTime { get; set; }
        public double preMarketPrice { get; set; }
        public double regularMarketChangePercent { get; set; }
        public double regularMarketPreviousClose { get; set; }
        public string fullExchangeName { get; set; }
        public string longName { get; set; }
        public string marketState { get; set; }
        public string exchange { get; set; }
        public string shortName { get; set; }
        public decimal regularMarketPrice { get; set; }
        public int regularMarketTime { get; set; }
        public double regularMarketChange { get; set; }
        public int regularMarketVolume { get; set; }
        public string symbol { get; set; }
    }
}
