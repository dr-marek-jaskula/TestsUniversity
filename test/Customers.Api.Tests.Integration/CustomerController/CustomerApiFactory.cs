using Customers.Api.Database;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Customers.Api.Tests.Integration.CustomerController;

//We derive from WebApplicationFactory<IApiMarker> in order to override all that we want about the app logic
//This give much control: over the database and github integration point

public class CustomerApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    //this is a constant that we introduce to test GitHub
    //So this will be a valid user to be called
    public const string ValidGitHubUser = "validuser";

    //This is for a user that has exceeded the available number of calls to the api
    public const string ThrottledUser = "throttle";

    //We do not need to worry about security, because it is just a test database, that will be deleted after tests

    //This is a general way for custom containers
    //private readonly TestcontainersContainer _dbContainer = new TestcontainersBuilder<TestcontainersContainer>()
    //    .WithImage("postgres")
    //    .WithEnvironment("POSTGRES_USER", "postgres")
    //    .WithEnvironment("POSTGRES_PASSWORD", "DataBase")
    //    .WithEnvironment("POSTGRES_DB", "TestDatabase")
    //    .WithPortBinding(5555, 5432)
    //    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
    //    .Build();

    //For a databases we do it simpler (for all popular db it is supported: MSSQL, MySQL, PostgreSQL, MongoDB, Oracle):
    private readonly TestcontainerDatabase _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "TestDatabase",
            Username = "postgres",
            Password = "DataBase"
        })
        .Build();
    //this will run on a random port, but it will match the ports (random port is important, due to many tests)

    //Here we will add a fake GitHubServer for fake responses
    private readonly GitHubApiServer _gitHubApiServer = new(); 

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            //This means "run this application without any logging providers" (to avoid logs on the server) 
            logging.ClearProviders(); //To avoid logs being made for tests
        });

        //This is the configuration that will let us use the container for our tests
        //The image needs to be created previously
        //This is DI container services that we can manipulate anyway we want
        //This is same as ConfigureServicer but explicit for tests (it is just better for tests)
        builder.ConfigureTestServices(services =>
        {
            //We remove background services. We do not want to test them in integrations tests. They can spoil much
            services.RemoveAll(typeof(IHostedService)); //this will remove all background services

            //We want to remove IDbConnectionFactory that is used by repositories and replace with one 
            //that is using the connecting string that we want (just to override the connection string to connect
            //to the container)
            //The other way would be to override the configuration, but this is a better approach 

            //1. We remove the old connection factory
            services.RemoveAll(typeof(IDbConnectionFactory));
            //2. We add new connection factory with a valid test connection string
            //The port is the Host Port is the external one (so just change the port)
            
            services.AddSingleton<IDbConnectionFactory>(_ =>
                new NpgsqlConnectionFactory(_dbContainer.ConnectionString));

            services.AddHttpClient("GitHub", httpClient =>
            {
                httpClient.BaseAddress = new Uri(_gitHubApiServer.Url);
                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.Accept, "application/vnd.github.v3+json");
                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.UserAgent, $"Course-{Environment.MachineName}");
            });

            //For the Entity Framework Core way:
            //1. remove all dbContexts or remove all specific dbContext: AppContext
            //services.RemoveAll(typeof(DbContext));
            //2. Add db context with connection to the database in docker (the proper, preferred way)
            //services.AddDbContext<AppDbContext>(options =>
            //    options.UseNpgsql(_dbContainer.ConnectionString));

            //!! this is very bad for integration tests!!!
            //services.AddDbContext<AppDbContext>(options =>
            //    options.UseInMemoryDatabase("TestDatabase")); //soo bad
        });
    }

    public async Task InitializeAsync()
    {
        //We setup the fake github user server
        _gitHubApiServer.Start();
        _gitHubApiServer.SetupUser(ValidGitHubUser);
        _gitHubApiServer.SetupThrottledUser(ThrottledUser);

        await _dbContainer.StartAsync();
    }

    //This method from the interface IAsyncLifetime will hide the one in the "WebApplicationFactory<IApiMarker>"
    //This in intentional hiding
    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        _gitHubApiServer.Dispose();
    }
}

//Just to show how to test with Entity Framework Core (and remember not to use InMemory database with EFCore)
//public class AppDbContext : DbContext
//{

//}