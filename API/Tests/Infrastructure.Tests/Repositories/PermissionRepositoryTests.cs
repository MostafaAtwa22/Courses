using Application.DTOs.Authorization.Permissions;
using Application.Common.Interfaces;
using Domain.Constants;
using FluentAssertions;
using Infrastructure.Persistence.Data;
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

            _repository = new PermissionRepository(_roleManagerMock.Object, null!);
        }

        [Fact]
        public async Task GetRolePermissionsAsync_ShouldReturnRolePermissions()
        {
            // Arrange
            var role = new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin" };
            var claims = new List<Claim>
            {
                new(CustomClaims.Permissions, "Permission:Role:Read"),
                new(CustomClaims.Permissions, "Permission:User:Create")
            };

            _roleManagerMock.Setup(x => x.GetClaimsAsync(role))
                .ReturnsAsync(claims);

            // Act
            var result = await _repository.GetRolePermissionsAsync(role);

            // Assert
            result.Should().NotBeNull();
            result.RoleId.Should().Be(Guid.Parse(role.Id));
            result.RoleName.Should().Be("Admin");
            result.Permissions.Should().NotBeEmpty();

            var readPermission = result.Permissions.FirstOrDefault(p => p.Name == "Permission:Role:Read");
            readPermission.Should().NotBeNull();
            readPermission.IsSelected.Should().BeTrue();
            readPermission.Module.Should().Be("Role");
            readPermission.Action.Should().Be("Read");

            var updatePermission = result.Permissions.FirstOrDefault(p => p.Name == "Permission:Role:Update");
            updatePermission.Should().NotBeNull();
            updatePermission.IsSelected.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateRolePermissionsAsync_ShouldThrow_WhenDbContextIsNull()
        {
            // Arrange
            var role = new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin" };
            var selectedPermissions = new List<string> { "Permission:Role:Read" };

            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _repository.UpdateRolePermissionsAsync(role, selectedPermissions));
        }
    }
}
