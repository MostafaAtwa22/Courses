using Domain.Enums;
using FluentAssertions;

namespace Domain.Tests.Enums;

public class GenderTests
{
    [Fact]
    public void Gender_ShouldHaveMaleValue()
    {
        // Arrange & Act
        var gender = Gender.Male;

        // Assert
        gender.Should().Be(Gender.Male);
    }

    [Fact]
    public void Gender_ShouldHaveFemaleValue()
    {
        // Arrange & Act
        var gender = Gender.Female;

        // Assert
        gender.Should().Be(Gender.Female);
    }

    [Fact]
    public void Gender_ShouldHaveTwoValues()
    {
        // Arrange & Act
        var values = Enum.GetValues<Gender>();

        // Assert
        values.Should().HaveCount(2);
    }

    [Fact]
    public void Gender_ShouldBeComparable()
    {
        // Arrange & Act
        var male = Gender.Male;
        var female = Gender.Female;

        // Assert
        male.Should().NotBe(female);
    }

    [Fact]
    public void Gender_ShouldBeAssignableToInt()
    {
        // Arrange & Act
        var male = (int)Gender.Male;
        var female = (int)Gender.Female;

        // Assert
        male.Should().BeOfType(typeof(int));
        female.Should().BeOfType(typeof(int));
        male.Should().NotBe(female);
    }
}
