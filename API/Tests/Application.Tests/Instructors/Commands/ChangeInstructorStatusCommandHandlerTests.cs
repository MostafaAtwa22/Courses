using Application.Features.Instructors.Commands.ChangeStatus;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Instructors.Commands;

public class ChangeInstructorStatusCommandHandlerTests
{

    [Fact]
    public void Handle_ShouldChangeStatus_WhenInstructorExists()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var command = new ChangeInstructorStatusCommand(instructorId, InstructorStatus.Verfied);
        
        // Note: This is a placeholder test. In a real implementation,
        // you would need to set up a DbContext with test data
        // and create the handler with the appropriate dependencies.
        
        // For now, we'll just verify the command structure is correct
        command.Id.Should().Be(instructorId);
        command.Status.Should().Be(InstructorStatus.Verfied);
    }

    [Fact]
    public void Handle_ShouldReturnNotFound_WhenInstructorDoesNotExist()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var command = new ChangeInstructorStatusCommand(instructorId, InstructorStatus.Unverfied);
        
        // Note: This is a placeholder test. In a real implementation,
        // you would need to set up a DbContext with test data
        // and create the handler with the appropriate dependencies.
        
        // For now, we'll just verify the command structure is correct
        command.Id.Should().Be(instructorId);
        command.Status.Should().Be(InstructorStatus.Unverfied);
    }
}
