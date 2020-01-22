using SustainAndGain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models
{
    public class CompetitionsService
    {
        private readonly SustainGainContext context;

        public CompetitionsService(SustainGainContext context)
        {
            this.context = context;
        }

        public void AddCompetition()
        {
            int month = 1;
            
            for (int i = 0; i < 5; i++)
                
            {
                Competition competition = new Competition
                {
                    StartTime = new DateTime(2020, month, 23),
                    EndTime = new DateTime(2020, month + 1, 23),
              
                };
                month++;
                context.Competition.Add(competition);
                context.SaveChanges();
            }
            
        }

        public void DisplayCompetitions()
        {

        }

    }
}
