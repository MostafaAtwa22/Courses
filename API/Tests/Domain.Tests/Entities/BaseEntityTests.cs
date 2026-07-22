using Domain.Entities;
using FluentAssertions;

namespace Domain.Tests.Entities;

public class BaseEntityTests
{
    [Fact]
    public void BaseEntity_ShouldHaveGuidId()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void BaseEntity_ShouldHaveCreatedAtSetToUtcNow()
    {
        // Arrange & Act
        var beforeCreation = DateTime.UtcNow;
        var entity = new TestEntity();
        var afterCreation = DateTime.UtcNow;

        // Assert
        entity.CreatedAt.Should().BeOnOrBefore(afterCreation);
        entity.CreatedAt.Should().BeOnOrAfter(beforeCreation);
    }

    [Fact]
    public void BaseEntity_ShouldHaveUpdatedAtSetToUtcNow()
    {
        // Arrange & Act
        var beforeUpdate = DateTime.UtcNow;
        var entity = new TestEntity();
        var afterUpdate = DateTime.UtcNow;

        // Assert
        entity.UpdatedAt.Should().BeOnOrBefore(afterUpdate);
        entity.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
    }

    [Fact]
    public void BaseEntity_ShouldAllowIdToBeSet()
    {
        // Arrange
        var expectedId = Guid.NewGuid();

        // Act
        var entity = new TestEntity { Id = expectedId };

        // Assert
        entity.Id.Should().Be(expectedId);
    }

    [Fact]
    public void BaseEntity_ShouldAllowDatesToBeSet()
    {
        // Arrange
        var expectedCreatedAt = DateTime.UtcNow.AddDays(-1);
        var expectedUpdatedAt = DateTime.UtcNow;

        // Act
        var entity = new TestEntity
        {
            CreatedAt = expectedCreatedAt,
            UpdatedAt = expectedUpdatedAt
        };

        // Assert
        entity.CreatedAt.Should().Be(expectedCreatedAt);
        entity.UpdatedAt.Should().Be(expectedUpdatedAt);
    }

    private class TestEntity : BaseEntity;
}
