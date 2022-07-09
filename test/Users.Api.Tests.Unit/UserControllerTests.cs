using Microsoft.AspNetCore.Mvc;
using NSubstitute.ReturnsExtensions;
using Users.Api.Dtos;
using Users.Api.Controllers;
using Users.Api.Mappers;
using Users.Api.Models;
using Users.Api.Services;

namespace Users.Api.Tests.Unit;

//Controller should be uncommon to test.
//Nevertheless, in some cases it is good to test them.
//For this purpose this file was introduced

public class UserControllerTests
{
    private readonly UserController _sut;
    private readonly IUserService _userService = Substitute.For<IUserService>();

    public UserControllerTests()
    {
        _sut = new UserController(_userService);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkAndObject_WhenUserExists()
    {
        //Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Marek Jaskuła"
        };

        _userService.GetByIdAsync(user.Id).Returns(user);
        var userDto = user.ToUserDto();

        //Act
        //Here we need to cast to OkObjectResult (if there would not be any object, just OK, it would be OkResult)
        var actual = (OkObjectResult) await _sut.GetById(user.Id);

        //Assert
        actual.StatusCode.Should().Be(200);
        actual.Value.Should().BeEquivalentTo(userDto);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenUserDoesntExist()
    {
        //Arrange
        _userService.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        //Act
        var actual = (NotFoundResult) await _sut.GetById(Guid.NewGuid());

        //Assert
        actual.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        //Arrange
        _userService.GetAllAsync().Returns(Enumerable.Empty<User>());

        //Act
        var actual = (OkObjectResult) await _sut.GetAll();

        //Assert
        actual.StatusCode.Should().Be(200);
        //It is a good practice to case it to UserDto
        actual.Value.As<IEnumerable<UserDto>>().Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturnUsersDto_WhenUsersExist()
    {
        //Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Marek Jaskuła"
        };
        var users = new[] { user };
        var usersDtos = users.Select(x => x.ToUserDto());
        _userService.GetAllAsync().Returns(users);

        //Act
        var actual = (OkObjectResult) await _sut.GetAll();

        //Assert
        actual.StatusCode.Should().Be(200);
        actual.Value.As<IEnumerable<UserDto>>().Should().BeEquivalentTo(usersDtos);
    }

    [Fact]
    public async Task Create_ShouldCreateUser_WhenCreateUserDtoIsValid()
    {
        // Arrange
        var createUserDto = new CreateUserDto("Marek Jaskuła");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = createUserDto.FullName
        };

        //User above will be overwritten after the following call is done.
        //The purpose is to match the new user guid that would happen due to the Transient lifetime.
        _userService.CreateAsync(Arg.Do<User>(x => user = x)).Returns(true);
        //Nevertheless, we need to make a "expectedUserDto" initialization below the act, so in assert region
        //It is done because the "_sut.Create" will change the user guid

        //Act
        var actual = (CreatedAtActionResult) await _sut.Create(createUserDto);

        //Assert
        //This is moved to assertion section due to the "Arg.Do"
        var expectedUserDto = user.ToUserDto();
        actual.StatusCode.Should().Be(201);
        actual.Value.As<UserDto>().Should().BeEquivalentTo(expectedUserDto);
        //In order to examine the header of the response
        actual.RouteValues!["id"].Should().BeEquivalentTo(user.Id);

        //Other way, without this "Do" up there. To exclude what should not be compared (if guid would change)
        // result.Value.As<UserDto>().Should()
        //     .BeEquivalentTo(expectedUserDto, options => options.Excluding(x => x.Id));
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenCreateUserDtoIsInvalid()
    {
        //Arrange
        _userService.CreateAsync(Arg.Any<User>()).Returns(false);

        //Act
        var actual = (BadRequestResult) await _sut.Create(new CreateUserDto(string.Empty));

        //Assert
        actual.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task DeleteById_ReturnsOk_WhenUserWasDeleted()
    {
        //Arrange
        _userService.DeleteByIdAsync(Arg.Any<Guid>()).Returns(true);

        //Act
        //If no object is return we have "OkResult" and no "OkObjectResult"
        var actual = (OkResult) await _sut.DeleteById(Guid.NewGuid());

        //Assert
        actual.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeleteById_ReturnsNotFound_WhenUserWasNotDeleted()
    {
        // Arrange
        _userService.DeleteByIdAsync(Arg.Any<Guid>()).Returns(false);

        // Act
        var actual = (NotFoundResult)await _sut.DeleteById(Guid.NewGuid());

        // Assert
        actual.StatusCode.Should().Be(404);
    }
}