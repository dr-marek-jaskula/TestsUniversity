namespace TestingTechniques.Tests.Unit.Master;

//This is just a marker class, to make fixture possible to be shared to many test classes

//1. The class needs to implement the ICollectionFixture<NameOfAClassFixture> interface
//2. Class needs to have an attribute "CollectionDefinition" with some name

[CollectionDefinition("My custom collection fixture")]
//if we want to make it one by one within a collection
//[CollectionDefinition("My custom collection fixture", DisableParallelization = true)]
public class TestCollectionFixture : ICollectionFixture<MyClassFixture>
{

}