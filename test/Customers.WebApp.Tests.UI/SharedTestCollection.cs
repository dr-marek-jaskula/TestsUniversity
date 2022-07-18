namespace Customers.WebApp.Tests.UI;

//The proper way to share context between the different test classes


//Use the class with constants to be sure that the spelling will not happen
public static class CollectionNames
{
    public const string UI_TestsCollection = "UI test collection";
}

//[CollectionDefinition("UI test collection")]
[CollectionDefinition(CollectionNames.UI_TestsCollection)]
public class SharedTestCollection : ICollectionFixture<SharedTestContext>
{

}