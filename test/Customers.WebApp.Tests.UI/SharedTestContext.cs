using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Builders;
using Microsoft.Playwright;

namespace Customers.WebApp.Tests.UI;

//This is an equivalent of CustomerApiFactory for UI tests
public class SharedTestContext : IAsyncLifetime
{
    public const string ValidGitHubUsername = "validuser";
    public const string AppUrl = "https://localhost:7780";
    public GitHubApiServer GitHubApiServer { get; } = new();

    //Here we make the docker compose to be configured to be run from the code
    private static readonly string DockerComposeFile = 
        Path.Combine(Directory.GetCurrentDirectory(), (TemplateString)"../../../docker-compose.UI.yml");

    private readonly ICompositeService _dockerService = new Builder()
        .UseContainer()
        .UseCompose()
        .FromFile(DockerComposeFile)
        .RemoveOrphans()
        .WaitForHttp("test-app", AppUrl)
        .Build();

    //Here we configure the playwright
    private IPlaywright _playwright;
    public IBrowser Browser { get; private set; }

    public async Task InitializeAsync()
    {
        GitHubApiServer.Start();
        GitHubApiServer.SetupUser(ValidGitHubUsername);
        _dockerService.Start();

        _playwright = await Playwright.CreateAsync();
        //Use Chromium, because Firefox has problems with ssl certificate for docker
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            //Headless = false, //show me the browser and steps that are being done. For tests it is true, to examine tests we can false
            //SlowMo = 1000 //slow motion
            SlowMo = 100 //preferred speed is 100 or 150
        });
    }

    public async Task DisposeAsync()
    {
        if (Browser is not null)
            await Browser.DisposeAsync();
        if (_playwright is not null)
            _playwright.Dispose();

        _dockerService.Dispose();
        GitHubApiServer.Dispose();
    }
}