using API.Endpoints;
using Application.Common.Models;
using Application.DTOs.Admin;
using Application.Features.Admin.Commands.Create;
using Application.Features.Admin.Commands.Delete;
using Application.Features.Admin.Queries.GetAll;
using Application.Features.Admin.Queries.GetById;
using Domain.Enums.Identity;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace API.Tests.Endpoints;

public class AdminEndpointsTests
{
    private readonly Mock<IMediator> _mediatorMock;

    public AdminEndpointsTests()
    {
        _mediatorMock = new Mock<IMediator>();
    }

    [Fact]
    public async Task GetAllAdmins_ShouldReturnOk_WhenAdminsExist()
    {
        // Arrange
        var admins = new List<AdminResponseDto>
        {
            new AdminResponseDto { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Role = Role.Admin },
            new AdminResponseDto { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Role = Role.SuperAdmin }
        };

        var paginatedResult = new PaginatedResult<AdminResponseDto>(admins, 2, 1, 10);
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllAdminsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await AdminEndpoints.GetAllAdmins(It.IsAny<AdminQueryParams>(), _mediatorMock.Object);

        // Assert
        var okResult = result.Should().BeAssignableTo<Results<Ok<PaginatedResult<AdminResponseDto>>, BadRequest>>().Subject;
        okResult.Result.Should().BeOfType<Ok<PaginatedResult<AdminResponseDto>>>();
    }

    [Fact]
    public async Task GetAdminById_ShouldReturnOk_WhenAdminExists()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var admin = new AdminResponseDto { Id = adminId, FirstName = "John", LastName = "Doe", Role = Role.Admin };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAdminByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(admin);

        // Act
        var result = await AdminEndpoints.GetAdminById(adminId, _mediatorMock.Object);

        // Assert
        var okResult = result.Should().BeAssignableTo<Results<Ok<AdminResponseDto>, NotFound>>().Subject;
        okResult.Result.Should().BeOfType<Ok<AdminResponseDto>>();
    }

    [Fact]
    public async Task GetAdminById_ShouldReturnNotFound_WhenAdminNotExists()
    {
        // Arrange
        var adminId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAdminByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdminResponseDto?)null);

        // Act
        var result = await AdminEndpoints.GetAdminById(adminId, _mediatorMock.Object);

        // Assert
        var notFoundResult = result.Should().BeAssignableTo<Results<Ok<AdminResponseDto>, NotFound>>().Subject;
        notFoundResult.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task DeleteAdmin_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        var adminId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteAdminCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await AdminEndpoints.DeleteAdmin(adminId, _mediatorMock.Object);

        // Assert
        var noContentResult = result.Should().BeAssignableTo<Results<NoContent, NotFound>>().Subject;
        noContentResult.Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task CreateAdmin_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        var adminDto = new AdminCreateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            UserName = "johndoe",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Role = Role.Admin
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateAdminCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await AdminEndpoints.CreateAdmin(adminDto, _mediatorMock.Object);

        // Assert
        var noContentResult = result.Should().BeAssignableTo<Results<NoContent, BadRequest>>().Subject;
        noContentResult.Result.Should().BeOfType<NoContent>();
    }
}