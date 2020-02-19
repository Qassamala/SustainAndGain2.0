using Quartz;
using SustainAndGain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models.Scheduling
{
    [DisallowConcurrentExecution]
    public class TriggerUpdateCurrentPortfolioValues : IJob
    {
        private readonly PortfoliosService service;
        private readonly SustainGainContext dbContext;

        public TriggerUpdateCurrentPortfolioValues(PortfoliosService service, SustainGainContext dbContext)
        {
            this.service = service;
            this.dbContext = dbContext;
        }

        public Task Execute(IJobExecutionContext context)
        {
            service.UpdateCurrentValues();
            return Task.CompletedTask;
        }
    }
}
