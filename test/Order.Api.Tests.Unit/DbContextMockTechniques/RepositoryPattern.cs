using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Orders.Api.DbModels;
using Orders.Api.MapperProfiles;
using Orders.Api.Services;

namespace Orders.Api.Tests.Unit;

//This approach uses the repository pattern

public class OrderServiceTestsRepositoryPattern
{
    //private readonly DateTime _date;
    //private readonly OrderService _sut;
    //private readonly IOrderRepository _userRepository = Substitute.For<IOrderRepository>();

    //in-memory approach to mock the dbContext
    //1. Install "Microsoft.EntityFrameworkCore.InMemory" NuGet Package

    public OrderServiceTestsRepositoryPattern()
    {
    }

    [Fact]
    public void GetById_ShouldReturnOrder_WhenIdIsValid()
    {
        //Act
        //var actual = await _sut.GetById(1);

        //Assert
        //actual.Should().BeEquivalentTo(new Order
        //{
        //    Id = 1,
        //    Name = "balloon",
        //    Amount = 3,
        //    Deadline = _date.AddDays(3)
        //});
    }
}