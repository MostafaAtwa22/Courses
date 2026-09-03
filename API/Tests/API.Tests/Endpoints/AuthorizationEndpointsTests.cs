using API.Endpoints;
using Application.DTOs.Authorization;
using Application.Features.Authorization.Queries.GetAll;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace API.Tests.Endpoints
{
    public class AuthorizationEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public AuthorizationEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetRoles_ShouldReturnOk_WithRoles_WhenSuccessful()
        {
            // Arrange
            var expectedRoles = new List<RolesResponseDto>
            {
                new() { Id = Guid.NewGuid().ToString(), Name = "Admin", UserCount = 5 },
                new() { Id = Guid.NewGuid().ToString(), Name = "Student", UserCount = 100 }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRoles);

            // Act
            var result = await AuthorizationEndpoints.GetRoles(_mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<IReadOnlyCollection<RolesResponseDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedRoles);
        }

        [Fact]
        public async Task GetRoles_ShouldReturnOk_WithEmptyList_WhenNoRolesExist()
        {
            // Arrange
            var expectedRoles = new List<RolesResponseDto>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRoles);

            // Act
            var result = await AuthorizationEndpoints.GetRoles(_mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<IReadOnlyCollection<RolesResponseDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRoleByUserId_ShouldReturnOk_WithUserRoles_WhenUserExists()
        {
            // Arrange
            var userId = "user-123";
            var expectedUserRoles = new UserRolesManageDto
            {
                UserId = userId,
                UserName = "testuser",
                Email = "test@example.com",
                Roles = new List<CheckBoxRoleManageDto>
                {
                    new() { RoleId = Guid.NewGuid().ToString(), RoleName = "Admin", IsSelected = true },
                    new() { RoleId = Guid.NewGuid().ToString(), RoleName = "Student", IsSelected = false }
                }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRoleByUserIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUserRoles);

            // Act
            var result = await AuthorizationEndpoints.GetRoleByUserId(userId, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<UserRolesManageDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedUserRoles);
        }

        [Fact]
        public async Task GetRoleByUserId_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "non-existent-user";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRoleByUserIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserRolesManageDto?)null);

            // Act
            var result = await AuthorizationEndpoints.GetRoleByUserId(userId, _mediatorMock.Object);

            // Assert
            var notFoundResult = result.Result as NotFound;
            notFoundResult.Should().NotBeNull();
        }
    }
}
