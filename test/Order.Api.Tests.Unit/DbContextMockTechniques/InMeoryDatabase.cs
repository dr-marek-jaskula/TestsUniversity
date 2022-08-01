using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orders.Api.DbModels;
using Orders.Api.Exceptions;
using Orders.Api.MapperProfiles;
using Orders.Api.Models.DataTransferObjects;
using Orders.Api.Services;

namespace Orders.Api.Tests.Unit;

//This approach uses in-memory database to mock the database context
//1. Install "Microsoft.EntityFrameworkCore.InMemory" NuGet Package

public class OrderServiceTestsInMemoryDatabase : IAsyncLifetime
{
    private readonly DateTime _date;
    private OrderService? _sut;

    public OrderServiceTestsInMemoryDatabase()
    {
        _date = DateTime.Now;
    }

    [Fact]
    public async Task GetById_ShouldReturnOrder_WhenIdIsValid()
    {
        //Act
        var actual = await _sut.GetById(1);

        //Assert
        actual.Should().BeEquivalentTo(new OrderDto(1, "balloon1", 1, _date.AddDays(1)));
    }

    [Fact]
    public async Task GetById_ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        //Act
        Func<Task> actual = async Task () => await _sut.GetById(-1);

        //Assert
        await actual.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Order not found");
    }

    //We could do GetDatabaseContext sync and then call it in the constructor
    public async Task InitializeAsync()
    {
        var context = await GetDatabaseContext();

        //Mock mapper
        //_mapper.Map<OrderDto>(new Order() { Id = 1, Name = "balloon", Amount = 3, Deadline = _date.AddDays(3)})
        //    .Returns(new OrderDto(1, "balloon", 3, _date.AddDays(3)));

        //Or use mapper
        var myProfile = new OrderMappingProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);

        _sut = new(context, mapper);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private async Task<MyDbContext> GetDatabaseContext()
    {
        //2. Create in-memory database
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        //3. Create database context
        var databaseContext = new MyDbContext(options);

        databaseContext.Database.EnsureCreated();

        if (await databaseContext.Orders.CountAsync() <= 0)
        {
            for (int i = 1; i <= 10; i++)
            {
                databaseContext.Orders.Add(new Order()
                {
                    Id = i,
                    Name = $"balloon{i}",
                    Amount = i,
                    Deadline = _date.AddDays(i)
                });

                await databaseContext.SaveChangesAsync();
            }
        }

        return databaseContext;
    }
}