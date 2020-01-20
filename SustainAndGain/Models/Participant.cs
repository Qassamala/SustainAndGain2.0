using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models
{
    public class Participant
    {
        public decimal StartMoney { get; set; }
        public List<Stock> Stocks { get; set; }
        public decimal AvailableMoney { get; set; }
        public decimal TotalInKr { get; set; }
        public StockCompetition Competition { get; set; }

    }
}
