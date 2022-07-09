namespace TestingTechniques.Tests.Unit.Basics;

public class LongRunningTests
{
    //If we do not want to wait for a slow test to finish, we can set a timeout
    [Fact(Timeout = 2000)] //this will fail after 2s
    public async Task SlowTest()
    {
        await Task.Delay(10000);
    }
}