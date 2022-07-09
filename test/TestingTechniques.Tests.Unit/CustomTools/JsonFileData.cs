using Newtonsoft.Json;
using System.Reflection;
using Xunit.Sdk;

namespace TestingTechniques.Tests.Unit.CustomTools;

//This class needs to inherit from DataAttribute to be used to get data from a json file

/// <summary>
/// Use the path to json file in order to get test data
/// </summary>
public class JsonFileData : DataAttribute
{
    private readonly string _jsonPath;

    public JsonFileData(string jsonPath)
    {
        _jsonPath = jsonPath;
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (testMethod is null)
            throw new ArgumentNullException(nameof(testMethod));

        var currentDirectory = Directory.GetCurrentDirectory();
        var jsonFullPath = Path.GetRelativePath(currentDirectory, _jsonPath);

        if (!File.Exists(jsonFullPath))
            throw new ArgumentException($"Could not find file: {jsonFullPath}");

        var jsonData = File.ReadAllText(jsonFullPath);
        var allTestCases = JsonConvert.DeserializeObject<IEnumerable<object[]>>(jsonData);
        return allTestCases;
    }
}