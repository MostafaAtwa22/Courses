using Application.Features.Admin.Commands.Delete;
using FluentAssertions;

namespace Application.Tests.Admin.Commands.Delete;

public class DeleteAdminCommandHandlerTests
{
    [Fact]
    public void Command_ShouldHaveCorrectId()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var command = new DeleteAdminCommand(adminId);

        // Assert
        command.Id.Should().Be(adminId);
    }

    [Fact]
    public void Command_ShouldRequireAuthorization()
    {
        // Arrange
        var command = new DeleteAdminCommand(Guid.NewGuid());

        // Assert
        command.RequiredRoles.Should().Contain("Admin");
        command.RequiredRoles.Should().Contain("SuperAdmin");
        command.RequireOwnership.Should().BeFalse();
    }
}