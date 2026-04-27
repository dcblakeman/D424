using C_971.Models;

namespace C_971.Tests.Models;

public class CourseInstructorTests
{
    [Fact]
    public void DefaultConstructor_SetsIdToZero()
    {
        var instructor = new CourseInstructor();
        Assert.Equal(0, instructor.Id);
    }

    [Fact]
    public void DefaultConstructor_SetsNameToNull()
    {
        var instructor = new CourseInstructor();
        Assert.Null(instructor.Name);
    }

    [Fact]
    public void DefaultConstructor_SetsEmailToNull()
    {
        var instructor = new CourseInstructor();
        Assert.Null(instructor.Email);
    }

    [Fact]
    public void DefaultConstructor_SetsPhoneToNull()
    {
        var instructor = new CourseInstructor();
        Assert.Null(instructor.Phone);
    }

    [Fact]
    public void CanSetName()
    {
        var instructor = new CourseInstructor { Name = "Anika Patel" };
        Assert.Equal("Anika Patel", instructor.Name);
    }

    [Fact]
    public void CanSetEmail()
    {
        var instructor = new CourseInstructor { Email = "anika.patel@university.edu" };
        Assert.Equal("anika.patel@university.edu", instructor.Email);
    }

    [Fact]
    public void CanSetPhone()
    {
        var instructor = new CourseInstructor { Phone = "555-123-4567" };
        Assert.Equal("555-123-4567", instructor.Phone);
    }

    [Fact]
    public void CanSetId()
    {
        var instructor = new CourseInstructor { Id = 7 };
        Assert.Equal(7, instructor.Id);
    }

    [Fact]
    public void CanSetAllProperties()
    {
        var instructor = new CourseInstructor
        {
            Id = 1,
            Name = "Jane Smith",
            Email = "jsmith@university.edu",
            Phone = "987-654-3210"
        };

        Assert.Equal(1, instructor.Id);
        Assert.Equal("Jane Smith", instructor.Name);
        Assert.Equal("jsmith@university.edu", instructor.Email);
        Assert.Equal("987-654-3210", instructor.Phone);
    }

    [Fact]
    public void Name_AllowsNull()
    {
        var instructor = new CourseInstructor { Name = null };
        Assert.Null(instructor.Name);
    }

    [Fact]
    public void Email_AllowsNull()
    {
        var instructor = new CourseInstructor { Email = null };
        Assert.Null(instructor.Email);
    }

    [Fact]
    public void Phone_AllowsNull()
    {
        var instructor = new CourseInstructor { Phone = null };
        Assert.Null(instructor.Phone);
    }
}
