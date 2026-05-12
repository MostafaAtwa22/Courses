using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Account.Queries.GetById;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using MockQueryable.Moq;
using Moq;

namespace Application.Tests.Account.Queries;

public class GetUserByIdHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IUrlProvider> _urlProviderMock;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdHandlerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _urlProviderMock = new Mock<IUrlProvider>();
        _handler = new GetUserByIdQueryHandler(_userManagerMock.Object, _urlProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var users = new List<ApplicationUser>
        {
            new() { Id = userId.ToString(), FirstName = "John", LastName = "Doe" }
        }.BuildMock();

        _userManagerMock.Setup(x => x.Users).Returns(users);
        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(["Instructor"]);

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId.ToString());
        result.Roles.Should().Contain("Instructor");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var users = new List<ApplicationUser>().BuildMock();
        _userManagerMock.Setup(x => x.Users).Returns(users);

        var query = new GetUserByIdQuery(Guid.NewGuid());

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
