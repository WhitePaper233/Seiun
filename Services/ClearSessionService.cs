namespace Seiun.Services;

public class ClearSessionTimedService(ICurrentStudySessionService currentStudySession, ILogger<ClearSessionTimedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 20小时执行一次
        using var timer = new PeriodicTimer(TimeSpan.FromHours(20)); 
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Clearing session start......");
                await currentStudySession.ClearSessionAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error clearing session");
            }
            await timer.WaitForNextTickAsync(stoppingToken); // 等待下次执行
        }
    }
}
