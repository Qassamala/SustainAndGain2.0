using Microsoft.Extensions.Logging;
using Quartz;
using SustainAndGain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustainAndGain.Models
{
    [DisallowConcurrentExecution]
    public class TriggerGetStockPricesJob : IJob
    {
        private readonly StocksService service;
        private readonly SustainGainContext dbContext;

        public TriggerGetStockPricesJob(StocksService service, SustainGainContext dbContext)
        {
            this.service = service;
            this.dbContext = dbContext;
        }

        public Task Execute(IJobExecutionContext context)
        {
            service.AddHistDataStocks();
            return Task.CompletedTask;
        }
    }
}
