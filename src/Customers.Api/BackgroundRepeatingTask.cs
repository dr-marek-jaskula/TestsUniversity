namespace Customers.Api;

public class BackgroundRepeatingTask : BackgroundService
{
    //Just to show how to test background tasks
    //commonly we dont want to test them in our integration tests
    //Therefore, in our integrations test we should remove them
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}
