using C_971.Models;

namespace C_971.Tests.Models;

public class AcademicTermTests
{
    [Fact]
    public void DefaultConstructor_SetsNameToEmptyString()
    {
        var term = new AcademicTerm();
        Assert.Equal(string.Empty, term.Name);
    }

    [Fact]
    public void DefaultConstructor_SetsIdToZero()
    {
        var term = new AcademicTerm();
        Assert.Equal(0, term.Id);
    }

    [Fact]
    public void DefaultConstructor_SetsIsActiveToFalse()
    {
        var term = new AcademicTerm();
        Assert.False(term.IsActive);
    }

    [Fact]
    public void DefaultConstructor_InitializesCoursesAsEmptyList()
    {
        var term = new AcademicTerm();
        Assert.NotNull(term.Courses);
        Assert.Empty(term.Courses);
    }

    [Fact]
    public void DefaultConstructor_SetsStartDateToNow()
    {
        var before = DateTime.Now;
        var term = new AcademicTerm();
        var after = DateTime.Now;
        Assert.InRange(term.StartDate, before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void DefaultConstructor_SetsEndDateToNow()
    {
        var before = DateTime.Now;
        var term = new AcademicTerm();
        var after = DateTime.Now;
        Assert.InRange(term.EndDate, before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void DefaultConstructor_SetsCreatedDateToNow()
    {
        var before = DateTime.Now;
        var term = new AcademicTerm();
        var after = DateTime.Now;
        Assert.InRange(term.CreatedDate, before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void CanSetName()
    {
        var term = new AcademicTerm { Name = "Fall 2025" };
        Assert.Equal("Fall 2025", term.Name);
    }

    [Fact]
    public void CanSetStartAndEndDate()
    {
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 6, 30);
        var term = new AcademicTerm { StartDate = start, EndDate = end };
        Assert.Equal(start, term.StartDate);
        Assert.Equal(end, term.EndDate);
    }

    [Fact]
    public void CanSetIsActive()
    {
        var term = new AcademicTerm { IsActive = true };
        Assert.True(term.IsActive);
    }

    [Fact]
    public void Courses_CanBePopulated()
    {
        var term = new AcademicTerm();
        term.Courses.Add(new Course { Name = "Math 101" });
        Assert.Single(term.Courses);
        Assert.Equal("Math 101", term.Courses[0].Name);
    }

    [Fact]
    public void CanSetId()
    {
        var term = new AcademicTerm { Id = 42 };
        Assert.Equal(42, term.Id);
    }
}
