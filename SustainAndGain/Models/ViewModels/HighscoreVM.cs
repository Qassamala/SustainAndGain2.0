using SustainAndGain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
    public class HighscoreVM
    {
        public AspNetUsers User { get; set; }
        public int Nr { get; set; }
        public decimal? CurrentValue { get; set; }
    }
}
