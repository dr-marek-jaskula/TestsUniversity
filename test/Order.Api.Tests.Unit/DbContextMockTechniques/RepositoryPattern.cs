using AutoMapper;
using Orders.Api.DbModels;
using Orders.Api.MapperProfiles;
using Orders.Api.Models.DataTransferObjects;
using Orders.Api.Repositories;
using Orders.Api.Services;

namespace Orders.Api.Tests.Unit;

//This approach uses the repository pattern

public class OrderServiceTestsRepositoryPattern
{
    private readonly DateTime _date;
    private readonly OrderServiceRepositoryPattern _sut;
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();

    public OrderServiceTestsRepositoryPattern()
    {
        _date = DateTime.Now;

        var myProfile = new OrderMappingProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);

        _sut = new(_orderRepository, mapper);
    }

    [Fact]
    public async Task GetById_ShouldReturnOrder_WhenIdIsValid()
    {
        //Arrange
        _orderRepository.GetById(1).Returns(new Order() 
        { 
            Id = 1, 
            Name = "balloon", 
            Amount = 3, Deadline = _date.AddDays(3) 
        });

        //Act
        var actual = await _sut.GetById(1);

        //Assert
        actual.Should().BeEquivalentTo(new OrderDto(1, "balloon", 3, _date.AddDays(3)));
    }
}