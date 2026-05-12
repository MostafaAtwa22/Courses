using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Account.Queries.GetAll;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using MockQueryable.Moq;
using Moq;

namespace Application.Tests.Account.Queries;

public class GetUsersHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IUrlProvider> _urlProviderMock;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersHandlerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _urlProviderMock = new Mock<IUrlProvider>();
        _handler = new GetUsersQueryHandler(_userManagerMock.Object, _urlProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedUsers_WhenUsersExist()
    {
        // Arrange
        var users = new List<ApplicationUser>
        {
            new() { Id = "1", FirstName = "John", LastName = "Doe", Email = "john@example.com", UserName = "johndoe" },
            new() { Id = "2", FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", UserName = "janedoe" }
        }.BuildMock();

        _userManagerMock.Setup(x => x.Users).Returns(users);
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(["Student"]);

        var query = new GetUsersQuery(new UserQueryParams { PageNumber = 1, PageSize = 10 });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Items[0].FirstName.Should().Be("John");
        result.Items[0].Roles.Should().Contain("Student");
    }

    [Fact]
    public async Task Handle_ShouldApplySearchFilter_WhenSearchTermProvided()
    {
        // Arrange
        var users = new List<ApplicationUser>
        {
            new() { Id = "1", FirstName = "John", LastName = "Doe", Email = "john@example.com", UserName = "johndoe" },
            new() { Id = "2", FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", UserName = "janedoe" }
        }.BuildMock();

        _userManagerMock.Setup(x => x.Users).Returns(users);
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync([]);

        var query = new GetUsersQuery(new UserQueryParams { SearchTerm = "Jane" });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items[0].FirstName.Should().Be("Jane");
    }
}
