## End-to-End tests

/We still need to add certificate

[They are integration UI tests?]

For End-to-End tests (UI tests) we will be using:
- WireMock
- Microsoft.Playwright (a bot that goes to the website and clicks, for opening the browser by c#)
- Creating app in a docker container 
- Ductus.FluentDocker (to fluently use docker for UI tests with playwright) - this allows to use docker-compose file from c# code

We want use WebApplicationFactory for this but a playwright (we cannot called the in memory version running by playwright)
Therefore, we will use docker
One docker-compose file for all tests

## Playwright

1. Add NuGet Package
2. Build the project (Playwright will install the installation script when project builds)
3. Open terminal and write:
pwsh .\bin\Debug\net6.0\playwright.ps1
this will install some stuff
We can also do it by c# code:
Microsoft.Playwright.Program.Main(new[] { "install" });

//Be sure it is installed (coz one time it does not). This is valid
pwsh bin\Debug\net6.0\playwright.ps1 install

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