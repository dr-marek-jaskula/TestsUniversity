using AutoMapper;
using EntityFrameworkCoreMock.NSubstitute;
using Microsoft.EntityFrameworkCore;
using Orders.Api.DbModels;
using Orders.Api.MapperProfiles;
using Orders.Api.Models.DataTransferObjects;
using Orders.Api.Services;

namespace Orders.Api.Tests.Unit;

//This approach uses the EntityFrameworkCoreMock.NSubstitute NuGet Package (there is also EntityFrameworkCoreMock.Moq version)

public class OrderServiceTestsNuGet
{
    private readonly DateTime _date;

    //1. Create DbContextOptions
    public DbContextOptions<MyDbContext> Options { get; } = new DbContextOptionsBuilder<MyDbContext>().Options;

    private OrderService? _sut;

    public OrderServiceTestsNuGet()
    {
        _date = DateTime.Now;

        //2. Create test data
        var initialEntities = new[]
{
            new Order()
            {
                Id = 1,
                Name = $"balloon1",
                Amount = 1,
                Deadline = _date.AddDays(1)
            },
            new Order()
            {
                Id = 2,
                Name = $"balloon2",
                Amount = 2,
                Deadline = _date.AddDays(2)
            }
        };

        //3. Mock the dbContext
        var dbContextMock = new DbContextMock<MyDbContext>(Options);

        //4. Mock dbContext sets
        var usersDbSetMock = dbContextMock.CreateDbSetMock(x => x.Orders, initialEntities);

        //Mock mapper
        //_mapper.Map<OrderDto>(new Order() { Id = 1, Name = "balloon1", Amount = 1, Deadline = _date.AddDays(1)})
        //    .Returns(new OrderDto(1, "balloon1", 1, _date.AddDays(1)));

        //Or use mapper
        var myProfile = new OrderMappingProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);

        _sut = new(dbContextMock.Object, mapper);
    }

    [Fact]
    public async Task GetById_ShouldReturnOrder_WhenIdIsValid()
    {
        //Act
        var actual = await _sut.GetById(1);

        //Assert
        actual.Should().BeEquivalentTo(new OrderDto(1, "balloon1", 1, _date.AddDays(1)));
    }
}