using Domain.Entities;
using Domain.Entities.Identity;
using FluentAssertions;

namespace Domain.Tests.Entities;

public class EnrollmentTests
{
    [Fact]
    public void Enrollment_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var enrollment = new Enrollment();

        // Assert
        enrollment.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Enrollment_ShouldAllowSettingStudentId()
    {
        // Arrange
        var expectedStudentId = Guid.NewGuid();

        // Act
        var enrollment = new Enrollment { StudentId = expectedStudentId };

        // Assert
        enrollment.StudentId.Should().Be(expectedStudentId);
    }

    [Fact]
    public void Enrollment_ShouldAllowSettingCourseId()
    {
        // Arrange
        var expectedCourseId = Guid.NewGuid();

        // Act
        var enrollment = new Enrollment { CourseId = expectedCourseId };

        // Assert
        enrollment.CourseId.Should().Be(expectedCourseId);
    }

    [Fact]
    public void Enrollment_ShouldAllowSettingStudent()
    {
        // Arrange
        var expectedStudent = new Student();

        // Act
        var enrollment = new Enrollment { Student = expectedStudent };

        // Assert
        enrollment.Student.Should().Be(expectedStudent);
    }

    [Fact]
    public void Enrollment_ShouldAllowSettingCourse()
    {
        // Arrange
        var expectedCourse = new Course();

        // Act
        var enrollment = new Enrollment { Course = expectedCourse };

        // Assert
        enrollment.Course.Should().Be(expectedCourse);
    }

    [Fact]
    public void Enrollment_ShouldHaveUniqueCombination()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        // Act
        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId
        };

        // Assert
        enrollment.StudentId.Should().Be(studentId);
        enrollment.CourseId.Should().Be(courseId);
    }

    [Fact]
    public void Enrollment_ShouldAllowMultipleEnrollmentsForSameStudent()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId1 = Guid.NewGuid();
        var courseId2 = Guid.NewGuid();

        // Act
        var enrollment1 = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId1
        };
        var enrollment2 = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId2
        };

        // Assert
        enrollment1.StudentId.Should().Be(studentId);
        enrollment2.StudentId.Should().Be(studentId);
        enrollment1.CourseId.Should().NotBe(enrollment2.CourseId);
    }

    [Fact]
    public void Enrollment_ShouldAllowMultipleEnrollmentsForSameCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId1 = Guid.NewGuid();
        var studentId2 = Guid.NewGuid();

        // Act
        var enrollment1 = new Enrollment
        {
            StudentId = studentId1,
            CourseId = courseId
        };
        var enrollment2 = new Enrollment
        {
            StudentId = studentId2,
            CourseId = courseId
        };

        // Assert
        enrollment1.CourseId.Should().Be(courseId);
        enrollment2.CourseId.Should().Be(courseId);
        enrollment1.StudentId.Should().NotBe(enrollment2.StudentId);
    }
}
