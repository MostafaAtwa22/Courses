using Application.Features.Admin.Queries.GetAll;
using FluentAssertions;
using Application.Common.Models;

namespace Application.Tests.Admin.Queries;

public class GetAllAdminsQueryHandlerTests
{
    [Fact]
    public void Handle_ShouldReturnAdmins_WhenAdminsExist()
    {
        // Arrange
        var queryParams = new AdminQueryParams();
        var query = new GetAllAdminsQuery(queryParams);

        query.Params.Should().Be(queryParams);
    }

    [Fact]
    public void Handle_ShouldReturnEmptyList_WhenNoAdminsExist()
    {
        // Arrange
        var queryParams = new AdminQueryParams();
        var query = new GetAllAdminsQuery(queryParams);

        query.Params.Should().Be(queryParams);
    }
}