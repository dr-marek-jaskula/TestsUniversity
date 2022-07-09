using Users.Api.Dtos;
using Users.Api.Models;

namespace Users.Api.Mappers;

public static class UserMapper
{
    public static UserDto ToUserDto(this User user) =>
        new(user.Id, user.FullName);
}
