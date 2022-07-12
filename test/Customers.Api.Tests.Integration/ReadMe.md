## Integration tests

- Old way:
1. Run the application (we can do this also by "dotnet run" in console)
2. Write tests

- Better way:
1. Use WebApplicationFactory with a running database

- The best modern way:
1. Use WebApplicationFactory
2. Use docker to run the database (test database)
    - For this project we run command "docker compose -f .\docker-compose-full.yml up"
    - We create the database
    - We do the integration tests
    - We delete the database (so all cleanups will be done automatically because database will go down)
To run integration tests with a docker containers we will use the NuGet Package:
"Testcontainers"

!!! IMPORATANT 
There is currently a bug in the latest version of Docker.DotNet which is maintained by Microsoft and the latest version of Docker Desktop (4.10) which will prevent you from running your tests.
A fix has been submitted so it is a matter of time until Microsoft adds it in.
In the meantime I suggest downgrading to Docker Desktop version  4.9.1. 
The Docker Docker Desktop 4.10.1 is still not working
Therfore, I have downgraded to Docker Desktop 4.9.1

## Naming convention

Get_ReturnsNotFound_WhenCustomerDoesNotExist

## Structure the test classes

Do not use one class, for instance "CustomerControllerTests" for all tests in this controller.

It is good practice to differentiate the Get, Create, Update, Delete actions into different test classes.
Nevertheless, the state should be shared between all these classes (so single in memory app should run for all of them)

In order to do that we create a "TestCollection.cs" file with a collection definition that implements 
ICollectionFixture<TypeWeWantToBeFixInACollection>
The collection definition name is important. 
The test classes needs to decorated with an attribute "Collection" with a specific name. Then, the type is shared
to all test classes

## Generate a certificate

in a project folder
dotnet dev-certs https -ep cert.pfx -p Test1234!

This is from docker-compose:

  api:
    build: .
    ports:
      - "5001:443"
      - "5000:80"
    environment:
      - ASPNETCORE_URLS=https://+:443;http://+:80
      - ASPNETCORE_Kestrel__Certificates__Default__Password=Test1234!
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx
      - ASPNETCORE_Environment=Production
      - CustomersApi_Database__ConnectionString=Server=db;Port=5432;Database=mydb;User ID=course;Password=changeme;
    depends_on:
      db:
        condition: service_started

## External Api tests

If we are testing the connection with external api, we should not test if it is working or not.
We should mock it (for instance github)

#### Medium approach:
1. Create a console application called "FakeApi" (we can specify the name after)
2. Download the NuGet Package "WireMock.Net"
3. Write

var wireMockServer = WireMockServer.Start();

Console.WriteLine($"Wiremock is now runnign on: {wireMockServer.Url}");

wireMockServer
    .Given(Request
        .Create()
        .WithPath("/example")
        .UsingGet())
    .RespondWith(Response
        .Create()
        .WithBody("This is coming from WireMock")
        .WithStatusCode(200));

Console.ReadKey();
wireMockServer.Dispose();

And use it to simulate responses for given requsts (for instance it was "http://localhost:65012/example" because 
wireMockServer.Url was equal to "http://localhost:65012")

#### Good approach:

Add class to integration test that will simulate the response from the api (here we will add GitHubApiServer.cs)
1. Install WireMock.Net;
2. Add <Name>ApiServer.cs class (fill the class)
3. Add GitHubApiServer in a CustomApiFactory and then start and setup it in a InitializeAsync and stop and dispose in a DisposeAsync
4. Get the client configuration from API, and override it in a "ConfigureTestServices":

## Entity Framework Core

If we use Entity Framework Core then NEVER replace build in Entity Framework dbContext with a in memory one.
NEVER, NEVER do this. This killing the integration point of view. It does not test the right database.
Then, it behaves in a completely different way (different queries and other).
NEVER to this.

Look at the bottom of the CustomerApiFactory for a proper way