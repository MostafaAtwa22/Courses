using API.Endpoints;
using Application.DTOs.Authorization;
using Application.Features.Authorization.Commands.UpdateUserRoles;
using Application.Features.Authorization.Queries.GetAll;
using Application.Features.Authorization.Queries.GetRoleByUserId;
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
            var expectedUserRoles = new UserRolesResponseDto
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
            var okResult = result.Result as Ok<UserRolesResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedUserRoles);
        }

        [Fact]
        public async Task GetRoleByUserId_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "non-existent-user";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRoleByUserIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserRolesResponseDto?)null);

            // Act
            var result = await AuthorizationEndpoints.GetRoleByUserId(userId, _mediatorMock.Object);

            // Assert
            var notFoundResult = result.Result as NotFound;
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateUserRoles_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var userRolesDto = new UserRolesManageDto
            {
                UserId = "user-123",
                Roles = new List<CheckBoxRoleManageDto>
                {
                    new() { RoleId = Guid.NewGuid().ToString(), RoleName = "Admin", IsSelected = true },
                    new() { RoleId = Guid.NewGuid().ToString(), RoleName = "Student", IsSelected = false }
                }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserRolesCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await AuthorizationEndpoints.UpdateUserRoles(userRolesDto, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserRolesCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserRoles_ShouldCallMediator_WithCorrectCommand()
        {
            // Arrange
            var userRolesDto = new UserRolesManageDto
            {
                UserId = "user-456",
                Roles = new List<CheckBoxRoleManageDto>
                {
                    new() { RoleId = Guid.NewGuid().ToString(), RoleName = "Instructor", IsSelected = true }
                }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserRolesCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await AuthorizationEndpoints.UpdateUserRoles(userRolesDto, _mediatorMock.Object);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateUserRolesCommand>(c => c.Dto == userRolesDto), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
