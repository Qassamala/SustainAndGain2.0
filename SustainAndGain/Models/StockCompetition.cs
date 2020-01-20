using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models
{
    public class StockCompetition
    {
        public decimal Saldo { get; set; }
        public int Procentuel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Stock> Stocks { get; set; }

    }
}
