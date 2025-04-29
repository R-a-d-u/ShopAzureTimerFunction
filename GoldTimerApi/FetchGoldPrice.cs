using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GoldTimerApi
{
    public class FetchGoldPrice
    {
        private readonly ILogger _logger;
        private readonly IGoldHistoryRepository _goldHistoryRepository;

        public FetchGoldPrice(ILoggerFactory loggerFactory, IGoldHistoryRepository goldHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<FetchGoldPrice>();
            _goldHistoryRepository = goldHistoryRepository;
        }

        [Function("Function1")]
        public async Task  Run([TimerTrigger("1-4 3 * * *")] TimerInfo myTimer)
        {
            var result = await _goldHistoryRepository.AddGoldHistoryAsync();
            _logger.LogInformation($"Gold price updated successfully. Price per gram: {result}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
