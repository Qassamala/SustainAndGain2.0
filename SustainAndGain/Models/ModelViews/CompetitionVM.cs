using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.ModelViews
{
    public class CompetitionVM
    {
        public CompetitionVM()
        {
            

        }
        private string daysLeft;

        public string DaysLeft
        {
            get { return daysLeft; }
            set
            {
                if (double.Parse(value) > 0)
                {
                    daysLeft = value;
                }
                else
                    daysLeft = "Ongoing";
                }
        }

        
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsOngoing { get; set; }
        public bool IsRegistered { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public bool HasJoined { get; set; }
        public string UserId { get; set; }
        public string CompId { get; set; }
    }
}
