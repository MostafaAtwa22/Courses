using Application.DTOs.Authorization.Permissions;
using Application.Common.Interfaces;
using Domain.Constants;
using FluentAssertions;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;

namespace Infrastructure.Tests.Repositories
{
    public class PermissionRepositoryTests
    {
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly PermissionRepository _repository;

        public PermissionRepositoryTests()
        {
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(),
                null!,
                null!,
                null!,
                null!
            );
            _repository = new PermissionRepository(_roleManagerMock.Object);
        }

        [Fact]
        public async Task UpdateRolePermissionsAsync_ShouldOnlyAddNewPermissions()
        {
            // Arrange
            var role = new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin" };
            var currentClaims = new List<Claim>
            {
                new(CustomClaims.Permissions, "Permission:Role:Read")
            };
            var selectedPermissions = new List<string>
            {
                "Permission:Role:Read",
                "Permission:Role:Update"
            };

            _roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(currentClaims);
            _roleManagerMock.Setup(x => x.AddClaimAsync(role, It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _repository.UpdateRolePermissionsAsync(role, selectedPermissions);

            // Assert
            _roleManagerMock.Verify(x => x.AddClaimAsync(
                role,
                It.Is<Claim>(c => c.Type == CustomClaims.Permissions && c.Value == "Permission:Role:Update")),
                Times.Once);
            _roleManagerMock.Verify(x => x.RemoveClaimAsync(role, It.IsAny<Claim>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRolePermissionsAsync_ShouldOnlyRemoveDeselectedPermissions()
        {
            // Arrange
            var role = new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin" };
            var currentClaims = new List<Claim>
            {
                new(CustomClaims.Permissions, "Permission:Role:Read"),
                new(CustomClaims.Permissions, "Permission:Role:Update")
            };
            var selectedPermissions = new List<string>
            {
                "Permission:Role:Read"
            };

            _roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(currentClaims);
            _roleManagerMock.Setup(x => x.RemoveClaimAsync(role, It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _repository.UpdateRolePermissionsAsync(role, selectedPermissions);

            // Assert
            _roleManagerMock.Verify(x => x.RemoveClaimAsync(
                role,
                It.Is<Claim>(c => c.Type == CustomClaims.Permissions && c.Value == "Permission:Role:Update")),
                Times.Once);
            _roleManagerMock.Verify(x => x.AddClaimAsync(role, It.IsAny<Claim>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRolePermissionsAsync_ShouldDoNothing_WhenPermissionsUnchanged()
        {
            // Arrange
            var role = new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin" };
            var currentClaims = new List<Claim>
            {
                new(CustomClaims.Permissions, "Permission:Role:Read"),
                new(CustomClaims.Permissions, "Permission:Role:Update")
            };
            var selectedPermissions = new List<string>
            {
                "Permission:Role:Read",
                "Permission:Role:Update"
            };

            _roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(currentClaims);

            // Act
            await _repository.UpdateRolePermissionsAsync(role, selectedPermissions);

            // Assert
            _roleManagerMock.Verify(x => x.AddClaimAsync(role, It.IsAny<Claim>()), Times.Never);
            _roleManagerMock.Verify(x => x.RemoveClaimAsync(role, It.IsAny<Claim>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRolePermissionsAsync_ShouldAddAndRemove_WhenPermissionsChanged()
        {
            // Arrange
            var role = new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin" };
            var currentClaims = new List<Claim>
            {
                new(CustomClaims.Permissions, "Permission:Role:Read"),
                new(CustomClaims.Permissions, "Permission:Role:Delete")
            };
            var selectedPermissions = new List<string>
            {
                "Permission:Role:Read",
                "Permission:Role:Update"
            };

            _roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(currentClaims);
            _roleManagerMock.Setup(x => x.AddClaimAsync(role, It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(x => x.RemoveClaimAsync(role, It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _repository.UpdateRolePermissionsAsync(role, selectedPermissions);

            // Assert
            _roleManagerMock.Verify(x => x.AddClaimAsync(
                role,
                It.Is<Claim>(c => c.Type == CustomClaims.Permissions && c.Value == "Permission:Role:Update")),
                Times.Once);
            _roleManagerMock.Verify(x => x.RemoveClaimAsync(
                role,
                It.Is<Claim>(c => c.Type == CustomClaims.Permissions && c.Value == "Permission:Role:Delete")),
                Times.Once);
        }

        [Fact]
        public async Task UpdateRolePermissionsAsync_ShouldRemoveAll_WhenNoneSelected()
        {
            // Arrange
            var role = new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin" };
            var currentClaims = new List<Claim>
            {
                new(CustomClaims.Permissions, "Permission:Role:Read"),
                new(CustomClaims.Permissions, "Permission:Role:Update")
            };
            var selectedPermissions = new List<string>();

            _roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(currentClaims);
            _roleManagerMock.Setup(x => x.RemoveClaimAsync(role, It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _repository.UpdateRolePermissionsAsync(role, selectedPermissions);

            // Assert
            _roleManagerMock.Verify(x => x.RemoveClaimAsync(role, It.IsAny<Claim>()), Times.Exactly(2));
            _roleManagerMock.Verify(x => x.AddClaimAsync(role, It.IsAny<Claim>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRolePermissionsAsync_ShouldAddAll_WhenNoneExist()
        {
            // Arrange
            var role = new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin" };
            var currentClaims = new List<Claim>();
            var selectedPermissions = new List<string>
            {
                "Permission:Role:Read",
                "Permission:Role:Update"
            };

            _roleManagerMock.Setup(x => x.GetClaimsAsync(role)).ReturnsAsync(currentClaims);
            _roleManagerMock.Setup(x => x.AddClaimAsync(role, It.IsAny<Claim>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _repository.UpdateRolePermissionsAsync(role, selectedPermissions);

            // Assert
            _roleManagerMock.Verify(x => x.AddClaimAsync(role, It.IsAny<Claim>()), Times.Exactly(2));
            _roleManagerMock.Verify(x => x.RemoveClaimAsync(role, It.IsAny<Claim>()), Times.Never);
        }
    }
}
