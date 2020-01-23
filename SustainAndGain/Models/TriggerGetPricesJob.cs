using Microsoft.Extensions.Logging;
using Quartz;
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

        public TriggerGetStockPricesJob(StocksService service)
        {
            this.service = service;
        }
        private readonly ILogger<TriggerGetStockPricesJob> _logger;

        public TriggerGetStockPricesJob(ILogger<TriggerGetStockPricesJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            service.AddHistDataStocks();
            return Task.CompletedTask;
        }
    }
}
