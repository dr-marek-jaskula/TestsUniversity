namespace TestingTechniques.Tests.Unit.Master;

public class MyClassFixture : IDisposable
{
    public Guid Id { get; } = Guid.NewGuid();

    //The first what is called is the constructor of the fixture
    public MyClassFixture()
    {

    }

    public void Dispose()
    {
    }
}