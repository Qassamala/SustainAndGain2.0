﻿using System;
using System.Collections.Generic;

namespace SustainAndGain.Models.Entities
{
    public partial class StocksInCompetition
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CompId { get; set; }
        public int StockId { get; set; }
        public int Quantity { get; set; }

        public virtual Competition Comp { get; set; }
        public virtual StaticStockData Stock { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
