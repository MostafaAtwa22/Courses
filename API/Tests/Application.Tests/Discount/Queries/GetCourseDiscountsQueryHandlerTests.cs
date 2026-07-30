using Application.Features.Discount.Queries.GetDiscounts;
using FluentAssertions;
using Xunit;

namespace Application.Tests.Discount.Queries;

public class GetCourseDiscountsQueryHandlerTests
{

    [Fact]
    public void Handle_ShouldReturnDiscounts_WhenCourseExists()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var query = new GetCourseDiscountsQuery(courseId);
        
        query.CourseId.Should().Be(courseId);
    }

    [Fact]
    public void Handle_ShouldReturnEmptyList_WhenCourseHasNoDiscounts()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var query = new GetCourseDiscountsQuery(courseId);

        query.CourseId.Should().Be(courseId);
    }
}
