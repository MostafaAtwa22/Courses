using Domain.Enums.Identity;
using FluentAssertions;

namespace Domain.Tests.Enums.Identity;

public class RoleTests
{
    [Fact]
    public void Role_ShouldHaveSuperAdminValue()
    {
        // Arrange & Act
        var role = Role.SuperAdmin;

        // Assert
        role.Should().Be(Role.SuperAdmin);
    }

    [Fact]
    public void Role_ShouldHaveAdminValue()
    {
        // Arrange & Act
        var role = Role.Admin;

        // Assert
        role.Should().Be(Role.Admin);
    }

    [Fact]
    public void Role_ShouldHaveInstructorValue()
    {
        // Arrange & Act
        var role = Role.Instructor;

        // Assert
        role.Should().Be(Role.Instructor);
    }

    [Fact]
    public void Role_ShouldHaveStudentValue()
    {
        // Arrange & Act
        var role = Role.Student;

        // Assert
        role.Should().Be(Role.Student);
    }

    [Fact]
    public void Role_ShouldHaveFourValues()
    {
        // Arrange & Act
        var values = Enum.GetValues<Role>();

        // Assert
        values.Should().HaveCount(4);
    }

    [Fact]
    public void Role_ShouldBeComparable()
    {
        // Arrange & Act
        var superAdmin = Role.SuperAdmin;
        var admin = Role.Admin;
        var instructor = Role.Instructor;
        var student = Role.Student;

        // Assert
        superAdmin.Should().NotBe(admin);
        admin.Should().NotBe(instructor);
        instructor.Should().NotBe(student);
    }

    [Fact]
    public void Role_ShouldBeAssignableToInt()
    {
        // Arrange & Act
        var superAdmin = (int)Role.SuperAdmin;
        var admin = (int)Role.Admin;
        var instructor = (int)Role.Instructor;
        var student = (int)Role.Student;

        // Assert
        superAdmin.Should().BeOfType(typeof(int));
        admin.Should().BeOfType(typeof(int));
        instructor.Should().BeOfType(typeof(int));
        student.Should().BeOfType(typeof(int));
    }

    [Fact]
    public void Role_ShouldHaveDistinctIntegerValues()
    {
        // Arrange & Act
        var values = new[] { (int)Role.SuperAdmin, (int)Role.Admin, (int)Role.Instructor, (int)Role.Student };

        // Assert
        values.Should().OnlyHaveUniqueItems();
    }
}
