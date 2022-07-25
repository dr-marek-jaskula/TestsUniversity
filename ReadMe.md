## Solution structure

Two directories: src, test:
- src should contain all projects
- tests should contain all test projects

In test directory two logical (not physical) directories:
- Unit
- Integration

## NuGet Packages 

- xUnit
- xunit.runner.visualstudio
- coverlet.collector
- FluentAssertions
- NSubstitute

## Naming convention

Naming conventions: 

Unit tests projects:
```
<ProjectNameToTest>.Tests.Unit
```

Integration tests projects:
```
<ProjectNameToTest>.Tests.Integration
```

Test classes:
```
<ClassNameToTest>Tests
```

Systems under tests (private readonly fields):
```
_sut
```

Test methods:
```
<MethodNameToTest>_Should<ExpectedBehavior>_When<TestScenario>
```

## What should be unit tested

Unit tests should be written for methods and classes that contains business logic.

Moreover:
- Private methods should not be directly tested as they are being called by other public or internal methods. 
- Internal methods should be tested. In the configuration details section it is shown how to allow test project to reach them.
- We should not test repositories that just transfer the query to the database.
- We should avoid testing controllers. However, it depends on a case.

## Configuration details

Use comments to structure sections "Arrange", "Act", "Assert".

To expose internals to the test project add in a project file the following code:
```
<ItemGroup>
	<InternalsVisibleTo Include="<TestProjectName>"/>
</ItemGroup>
```

Use implicit (global) using for:
- xUnit
- ProjectToTest main namespace
- FLuentAssertion
- NSubstitute (or Moq if used)

## Good practisies

Try not to hard code values in test methods. It is better to hard code them at the test class level:
	private const string OVERWEIGHT_SUMMARY = "You are a bit overweight"; 

## Test Parallelism

In order to disable parallelization, so make that every test in the project will run one after another use:
```csharp
[assembly: CollectionBehavior(DisableTestParallelization = true)]
```