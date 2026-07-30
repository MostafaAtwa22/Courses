using API.Endpoints;
using Application.Common.Models;
using Application.DTOs.Account;
using Application.Features.Account.Commands.ForgetPassword;
using Application.Features.Account.Commands.Lock;
using Application.Features.Account.Commands.ResetPassword;
using Application.Features.Account.Commands.UnLock;
using Application.Features.Account.Queries.GetAll;
using Application.Features.Account.Queries.GetById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace API.Tests.Endpoints
{
    public class AccountEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public AccountEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task ForgetPassword_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var request = new ForgetPasswordDto { Email = "test@example.com" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ForgetPasswordCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await AccountEndpoints.ForgetPassword(request, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var request = new ResetPasswordDto { Email = "test@example.com", Token = "token", NewPassword = "NewPassword123!" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ResetPasswordCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await AccountEndpoints.ResetPassword(request, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOk_WithPaginatedResult()
        {
            // Arrange
            var queryParams = new UserQueryParams();
            var expectedResult = new PaginatedResult<UserResponseDto>(new List<UserResponseDto>(), 0, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await AccountEndpoints.GetUsers(queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<UserResponseDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedUser = new UserResponseDto { Id = id };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await AccountEndpoints.GetUserById(id, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<UserResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(expectedUser);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserResponseDto)null!);

            // Act
            var result = await AccountEndpoints.GetUserById(id, _mediatorMock.Object);

            // Assert
            var notFoundResult = result.Result as NotFound;
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task LockUser_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new LockUserDto { LockoutUntil = DateTime.UtcNow.AddDays(7) };
            _mediatorMock.Setup(m => m.Send(It.IsAny<LockUserCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await AccountEndpoints.LockUser(id, request, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task UnlockUser_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<UnLockUserCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await AccountEndpoints.UnlockUser(id, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }
    }
}
