using Microsoft.AspNetCore.Mvc.Testing;

namespace Customers.Api.Tests.Integration.CustomerControllerBasic;

//Here we share the context
//So the collection will be now a set of test classes that are decorated by attribute 
//[Collection("CustomerControllerCollection")
//So one instance of a WebApplicationFactory will be shared for all of them, and they will run one by one

[CollectionDefinition("CustomerControllerBasicCollection")]
public class TestCollection : ICollectionFixture<WebApplicationFactory<IApiMarker>>
{

}