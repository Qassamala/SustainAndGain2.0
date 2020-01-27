﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
    public class OrderVM
    {
        public int StockId { get; set; }
        public int CompetitionId { get; set; }
        public string Symbol { get; set; }
        [Display(Name = "Order Value")]
        public decimal OrderValue { get; set; }
        [Display(Name = "Time Of Insertion")]
        public DateTime TimeOfInsertion { get; set; }
        [Display(Name = "Buy Or Sell")]
        public string BuyOrSell { get; set; }

        [Display(Name = "Available to Invest")]
        public decimal AvailableToInvest { get; set; }
    }
}
