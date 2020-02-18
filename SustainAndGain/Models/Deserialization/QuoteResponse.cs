using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models
{
    public class QuoteResponse
    {
            public List<Result> result { get; set; }
            public object error { get; set; }       
    }
}
