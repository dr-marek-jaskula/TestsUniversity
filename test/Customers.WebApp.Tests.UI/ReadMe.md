## UI tests

!! We still need to add certificate

For UI tests we will be using:
- WireMock (to mock external apis like GitHub)
- Microsoft.Playwright (a bot that goes to the website and clicks. I enables opening and use the browser by c#)
- Creating app in a docker container 
- Ductus.FluentDocker (to fluently use docker for UI tests with playwright) - this allows to use docker-compose file from c# code

We will not use WebApplicationFactory for here but a playwright 
Nevertheless, we cannot called the in memory version running by playwright
Therefore, we will use docker approach

One docker-compose file for all tests will be created

## Playwright

1. Add NuGet Package
2. Build the project (Playwright will install the installation script when project builds)
3. Open terminal and write:
pwsh .\bin\Debug\net6.0\playwright.ps1
This will install some stuff. !! If the stuff is not installed use (this is better):
pwsh bin\Debug\net6.0\playwright.ps1 install

We can also do it by c# code:
Microsoft.Playwright.Program.Main(new[] { "install" });

## Docker helthcech:

    healthcheck:
      test: [ "CMD-SHELL", "pg_isready" ]
      interval: 2s
      timeout: 5s
      retries: 10

it is "dont start next container if this container is not running"
this "CMD-SHELL", "pg_isready" is postgres specific
this will be 10 times trying after 2 second each and 5seccond timeout

## Certificate of SSL
      - ASPNETCORE_URLS=https://+:443;http://+:80
      - ASPNETCORE_Kestrel__Certificates__Default__Password=Test1234!
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx