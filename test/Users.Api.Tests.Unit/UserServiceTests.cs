using Microsoft.Data.Sqlite;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Users.Api.Logging;
using Users.Api.Models;
using Users.Api.Repositories;
using Users.Api.Services;

namespace Users.Api.Tests.Unit;

//The main way to unit test the WebApi

public class UserServiceTests
{
    private readonly UserService _sut;
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ILoggerAdapter<UserService> _logger = Substitute.For<ILoggerAdapter<UserService>>();

    public UserServiceTests()
    {
        _sut = new(_userRepository, _logger);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        //Arrange
        _userRepository.GetAllAsync().Returns(Enumerable.Empty<User>());

        //Act
        var actual = await _sut.GetAllAsync();

        //Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsers_WhenUsersExist()
    {
        //Arrange
        User marekJaskula = new()
        {
            Id = Guid.NewGuid(),
            FullName = "Marek Jaskuła"
        };

        var expectedUsers = new[]
        {
            marekJaskula
        };

        _userRepository.GetAllAsync().Returns(expectedUsers);

        //Act
        var actual = await _sut.GetAllAsync();

        //Assert
        actual.Should().BeEquivalentTo(expectedUsers);
    }

    //How to test logger:
    //In order to test the logger we would need to get the deepest level we can but then there is a internal class we cannot access
    //Therefore, we need to find another solution which is Adapter design pattern

    [Fact]
    public async Task GetAllAsync_ShouldLogMessages_WhenInvoked()
    {
        //Arrange
        _userRepository.GetAllAsync().Returns(Enumerable.Empty<User>());

        //Act
        await _sut.GetAllAsync();

        //Assert
        //Received means how many time the following method is called. Arg.Is specifies the certain argument
        _logger.Received(1).LogInformation(Arg.Is("Retrieving all users"));
        //_logger.Received(1).LogInformation(Arg.Is<string?>(x => x!.StartsWith("Retrieving")));
        _logger.Received(1).LogInformation(Arg.Is("All users retrieved in {0}ms"), Arg.Any<long>());
    }

    [Fact]
    public async Task GetAllAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
    {
        //Arrange
        var sqliteException = new SqliteException("Something went wrong", 500);
        _userRepository.GetAllAsync()
            .Throws(sqliteException);

        //Act
        //Because it throws an exception we define it in a lambda
        var requestAction = async () => await _sut.GetAllAsync();

        //Assert
        await requestAction.Should()
            .ThrowAsync<SqliteException>()
            .WithMessage("Something went wrong");

        _logger.Received(1)
            .LogError(Arg.Is(sqliteException), Arg.Is("Something went wrong while retrieving all users"));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        //Arrange
        _userRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        //Act
        var actual = await _sut.GetByIdAsync(Guid.NewGuid());

        //Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        //Arrange
        var existingUser = new User()
        {
            Id = Guid.NewGuid(),
            FullName = "Marek Jaskuła"
        };

        _userRepository.GetByIdAsync(Arg.Is(existingUser.Id)).Returns(existingUser);

        //Act
        var actualUser = await _sut.GetByIdAsync(existingUser.Id);

        //Assert
        actualUser.Should().BeEquivalentTo(existingUser);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldLogMessages_WhenInvoked()
    {
        //Arrange
        var userId = Guid.NewGuid();
        _userRepository.GetByIdAsync(userId).ReturnsNull();

        //Act
        await _sut.GetByIdAsync(userId);

        //Assert
        _logger.Received(1).LogInformation(Arg.Is("Retrieving user with id: {0}"), Arg.Is(userId));
        _logger.Received(1).LogInformation(Arg.Is("User with id {0} retrieved in {1}ms"), Arg.Is(userId), Arg.Any<long>());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var sqliteException = new SqliteException("Something went wrong", 500);
        _userRepository.GetByIdAsync(userId)
            .Throws(sqliteException);

        //Act
        var requestAction = async () => await _sut.GetByIdAsync(userId);

        //Assert
        await requestAction.Should()
            .ThrowAsync<SqliteException>()
            .WithMessage("Something went wrong");

        _logger.Received(1)
            .LogError(Arg.Is(sqliteException), Arg.Is("Something went wrong while retrieving user with id {0}"), Arg.Is(userId));
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser_WhenDetailsAreValid()
    {
        //Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Marek Jaskuła"
        };
        _userRepository.CreateAsync(user).Returns(true);

        //Act
        var actual = await _sut.CreateAsync(user);

        //Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_ShouldLogMessages_WhenInvoked()
    {
        //Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Marek Jaskuła"
        };
        _userRepository.CreateAsync(user).Returns(true);

        //Act
        await _sut.CreateAsync(user);

        //Assert
        _logger.Received(1).LogInformation(Arg.Is("Creating user with id {0} and name: {1}"),
            Arg.Is(user.Id), Arg.Is(user.FullName));
        _logger.Received(1).LogInformation(Arg.Is("User with id {0} created in {1}ms"),
            Arg.Is(user.Id), Arg.Any<long>());
    }

    [Fact]
    public async Task CreateAsync_ShouldLogMessages_WhenExceptionIsThrown()
    {
        //Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Marek Jaskuła"
        };
        var sqliteException = new SqliteException("Something went wrong", 500);
        _userRepository.CreateAsync(user)
            .Throws(sqliteException);

        //Act
        var requestAction = async () => await _sut.CreateAsync(user);

        //Assert
        await requestAction.Should()
            .ThrowAsync<SqliteException>()
            .WithMessage("Something went wrong");

        _logger.Received(1).LogError(Arg.Is(sqliteException),
            Arg.Is("Something went wrong while creating a user"));
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldDeleteUser_WhenUserExists()
    {
        //Arrange
        var userId = Guid.NewGuid();
        _userRepository.DeleteByIdAsync(userId).Returns(true);

        //Act
        var actual = await _sut.DeleteByIdAsync(userId);

        //Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldNotDeleteUser_WhenUserDoesntExist()
    {
        //Arrange
        var userId = Guid.NewGuid();
        _userRepository.DeleteByIdAsync(userId).Returns(false);

        //Act
        var actual = await _sut.DeleteByIdAsync(userId);

        //Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldLogMessages_WhenInvoked()
    {
        //Arrange
        var userId = Guid.NewGuid();
        _userRepository.DeleteByIdAsync(userId).Returns(true);

        //Act
        await _sut.DeleteByIdAsync(userId);

        //Assert
        _logger.Received(1).LogInformation(Arg.Is("Deleting user with id: {0}"), Arg.Is(userId));
        _logger.Received(1).LogInformation(Arg.Is("User with id {0} deleted in {1}ms"), Arg.Is(userId), Arg.Any<long>());
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var sqliteException = new SqliteException("Something went wrong", 500);
        _userRepository.DeleteByIdAsync(userId)
            .Throws(sqliteException);

        //Act
        var requestAction = async () => await _sut.DeleteByIdAsync(userId);

        //Assert
        await requestAction.Should()
            .ThrowAsync<SqliteException>()
            .WithMessage("Something went wrong");

        _logger.Received(1)
            .LogError(Arg.Is(sqliteException), Arg.Is("Something went wrong while deleting user with id {0}"), Arg.Is(userId));
    }
}