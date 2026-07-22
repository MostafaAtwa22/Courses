using Domain.Enums;
using FluentAssertions;

namespace Domain.Tests.Enums;

public class InstructorStatusTests
{
    [Fact]
    public void InstructorStatus_ShouldHavePendingValue()
    {
        // Arrange & Act
        var status = InstructorStatus.Pending;

        // Assert
        status.Should().Be(InstructorStatus.Pending);
    }

    [Fact]
    public void InstructorStatus_ShouldHaveVerfiedValue()
    {
        // Arrange & Act
        var status = InstructorStatus.Verfied;

        // Assert
        status.Should().Be(InstructorStatus.Verfied);
    }

    [Fact]
    public void InstructorStatus_ShouldHaveUnverfiedValue()
    {
        // Arrange & Act
        var status = InstructorStatus.Unverfied;

        // Assert
        status.Should().Be(InstructorStatus.Unverfied);
    }

    [Fact]
    public void InstructorStatus_ShouldHaveThreeValues()
    {
        // Arrange & Act
        var values = Enum.GetValues<InstructorStatus>();

        // Assert
        values.Should().HaveCount(3);
    }

    [Fact]
    public void InstructorStatus_ShouldBeComparable()
    {
        // Arrange & Act
        var pending = InstructorStatus.Pending;
       (var verfied, var unverfied) = (InstructorStatus.Verfied, InstructorStatus.Unverfied);

        // Assert
        pending.Should().NotBe(verfied);
        pending.Should().NotBe(unverfied);
        verfied.Should().NotBe(unverfied);
    }

    [Fact]
    public void InstructorStatus_ShouldBeAssignableToInt()
    {
        // Arrange & Act
        var pending = (int)InstructorStatus.Pending;
        var verfied = (int)InstructorStatus.Verfied;
        var unverfied = (int)InstructorStatus.Unverfied;

        // Assert
        pending.Should().BeOfType(typeof(int));
        verfied.Should().BeOfType(typeof(int));
        unverfied.Should().BeOfType(typeof(int));
        pending.Should().NotBe(verfied);
        verfied.Should().NotBe(unverfied);
    }
}
