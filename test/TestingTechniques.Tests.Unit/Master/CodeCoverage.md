## Code Coverage 

Code coverage measures how many lines of code are covered by tests. 
Aiming at 100% is a very bad practice. 
Nevertheless, a high value of code coverage can represent the code performance.

## IDE tools

Code coverage is not free in VisualStudio by default.
However, the extension "Fine Code Coverage" is free and perform well.

## Code Coverage by Command Line and Command Line tools

In order to measure the code coverage by command line (for instance for continuous integration)

We need to have package coverlet.collector (it is by default with xUnit)
1. We need to install a dotnet specific tool:
dotnet tool install -g coverlet.console
2. make a dotnet build for our project and get the path to dll of our project
3. use tool we installed:
coverlet <path to dll> --target "dotnet" --targetargs "test --no-build"
This will return the data about code coverage

However, we should exclude some part of code from calculating the code coverage
There are two ways for that:
1. OldApproach: We can use attribute [ExcludeFromCodeCoverage] on a class level (its dot net attribute) -> its respected by IDE also (but not know does by extension)
2. BetterApproach: use command:
coverlet <path to dll> --target "dotnet" --targetargs "test --no-build" --exclude "[*]<namespaceToBeExcluded>*"

The non-human file "coverage.json" will be generated
To interpret it we can (by NuGet package we installed)
dotnet test --collect:"XPlat Code Coverage"
Now a xml file with standard form will be generated called "coverage.cobertura.xml"

To interpret it further we need to use another tool:
dotnet tool install -g dotnet-reportgenerator-globaltool

Then we can use it:
reportgenerator -reports:"<path to this xml file>" -targetdir:"codecoverage" -reporttypes:Html
This will create a HTML file "index.html" and much more (open it by browser) to get clean results

This process should be automatized (we can make it an artifact and show other in our CI/CD pipieline)
