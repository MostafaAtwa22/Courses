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
            var expectedRoles = new List<RoleResponseDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Admin", UserCount = 5 },
                new() { Id = Guid.NewGuid(), Name = "Student", UserCount = 100 }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRoles);

            // Act
            var result = await AuthorizationEndpoints.GetRoles(_mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<IReadOnlyCollection<RoleResponseDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedRoles);
        }

        [Fact]
        public async Task GetRoles_ShouldReturnOk_WithEmptyList_WhenNoRolesExist()
        {
            // Arrange
            var expectedRoles = new List<RoleResponseDto>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRoles);

            // Act
            var result = await AuthorizationEndpoints.GetRoles(_mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<IReadOnlyCollection<RoleResponseDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEmpty();
        }
    }
}
