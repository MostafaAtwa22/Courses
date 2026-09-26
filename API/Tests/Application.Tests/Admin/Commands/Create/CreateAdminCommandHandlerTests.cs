using Application.DTOs.Admin;
using Application.Features.Admin.Commands.Create;
using Domain.Enums.Identity;
using FluentAssertions;

namespace Application.Tests.Admin.Commands.Create;

public class CreateAdminCommandHandlerTests
{
    [Fact]
    public void Command_ShouldHaveCorrectDto()
    {
        // Arrange
        var adminDto = new AdminCreateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            UserName = "johndoe",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Role = Role.Admin
        };
        var command = new CreateAdminCommand(adminDto);

        // Assert
        command.Dto.Should().Be(adminDto);
    }

    [Fact]
    public void Command_ShouldContainValidAdminData()
    {
        // Arrange
        var adminDto = new AdminCreateDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            UserName = "janesmith",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Role = Role.SuperAdmin
        };
        var command = new CreateAdminCommand(adminDto);

        // Assert
        command.Dto.FirstName.Should().Be("Jane");
        command.Dto.LastName.Should().Be("Smith");
        command.Dto.Email.Should().Be("jane.smith@example.com");
        command.Dto.UserName.Should().Be("janesmith");
        command.Dto.Role.Should().Be(Role.SuperAdmin);
    }
}