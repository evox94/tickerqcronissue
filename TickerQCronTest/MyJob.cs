using Microsoft.EntityFrameworkCore.Query.Internal;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.Interfaces.Managers;
using TickerQ.Utilities.Models;
using TickerQ.Utilities.Models.Ticker;

namespace TickerQCronTest;

public class MyJob
{
    private const string Name = "MyJob";
    private readonly ICronTickerManager<CronTicker> cronTickerManager;
    private readonly ILogger<MyJob> logger;

    public MyJob(ICronTickerManager<CronTicker> cronTickerManager, ILogger<MyJob> logger)
    {
        this.cronTickerManager = cronTickerManager;
        this.logger = logger;
    }

    [TickerFunction(Name)]
    public async Task ExecuteAsync()
    {
        await Task.Delay(1000); // Simulate work
    }

    public async Task<TickerResult<CronTicker>> QueueCron(string cronExpression)
    {
        var cronTicker = new CronTicker
        {
            Function = Name,
            Expression = cronExpression,
            Retries = 0
        };

        // Queue the cron job
        var result = await cronTickerManager.AddAsync(cronTicker);

        if (result.IsSucceded)
        {
            logger.LogInformation("Successfully queued cron job: {CronJob}", cronTicker);
        }
        else
        {
            logger.LogError(result.Exception, "Failed to queue cron job: {CronJob}", cronTicker);
        }

        return result;
    }
}