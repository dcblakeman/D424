using C_971.Models;

namespace C_971.Tests.Models;

public class CourseTests
{
    [Fact]
    public void DefaultConstructor_SetsIdToZero()
    {
        var course = new Course();
        Assert.Equal(0, course.Id);
    }

    [Fact]
    public void DefaultConstructor_SetsNameToEmptyString()
    {
        var course = new Course();
        Assert.Equal(string.Empty, course.Name);
    }

    [Fact]
    public void DefaultConstructor_SetsDescriptionToEmptyString()
    {
        var course = new Course();
        Assert.Equal(string.Empty, course.Description);
    }

    [Fact]
    public void DefaultConstructor_SetsCreditUnitsToThree()
    {
        var course = new Course();
        Assert.Equal(3, course.CreditUnits);
    }

    [Fact]
    public void DefaultConstructor_SetsStartDateNotificationsToFalse()
    {
        var course = new Course();
        Assert.False(course.StartDateNotifications);
    }

    [Fact]
    public void DefaultConstructor_SetsEndDateNotificationsToFalse()
    {
        var course = new Course();
        Assert.False(course.EndDateNotifications);
    }

    [Fact]
    public void DefaultConstructor_SetsGradeToA()
    {
        var course = new Course();
        // FinalGrade.A is the default (first value, 0) when no explicit initializer is set
        Assert.Equal(FinalGrade.A, course.Grade);
    }

    [Fact]
    public void DefaultConstructor_SetsStatusToNotEnrolled()
    {
        var course = new Course();
        Assert.Equal(CourseStatus.NotEnrolled, course.Status);
    }

    [Fact]
    public void DefaultConstructor_SetsStartDateToToday()
    {
        var course = new Course();
        Assert.Equal(DateTime.Today, course.StartDate.Date);
    }

    [Fact]
    public void DefaultConstructor_SetsEndDateToSixMonthsFromToday()
    {
        var course = new Course();
        Assert.Equal(DateTime.Today.AddMonths(6), course.EndDate);
    }

    [Fact]
    public void DefaultConstructor_InitializesAssessmentsAsEmptyList()
    {
        var course = new Course();
        Assert.NotNull(course.Assessments);
        Assert.Empty(course.Assessments);
    }

    [Fact]
    public void DefaultConstructor_InitializesNotesAsEmptyList()
    {
        var course = new Course();
        Assert.NotNull(course.Notes);
        Assert.Empty(course.Notes);
    }

    [Fact]
    public void DefaultConstructor_SetsTermIdToZero()
    {
        var course = new Course();
        Assert.Equal(0, course.TermId);
    }

    [Fact]
    public void DefaultConstructor_SetsInstructorIdToZero()
    {
        var course = new Course();
        Assert.Equal(0, course.InstructorId);
    }

    [Fact]
    public void DefaultConstructor_SetsInstructorToNull()
    {
        var course = new Course();
        Assert.Null(course.Instructor);
    }

    [Fact]
    public void DefaultConstructor_SetsTermToNull()
    {
        var course = new Course();
        Assert.Null(course.Term);
    }

    [Fact]
    public void CanSetAllCoreProperties()
    {
        var start = new DateTime(2025, 1, 15);
        var end = new DateTime(2025, 7, 15);
        var course = new Course
        {
            Id = 10,
            Name = "Advanced Algorithms",
            Description = "In-depth study of algorithms.",
            StartDate = start,
            EndDate = end,
            Status = CourseStatus.InProgress,
            Grade = FinalGrade.A,
            CreditUnits = 4,
            TermId = 2,
            InstructorId = 5,
            StartDateNotifications = true,
            EndDateNotifications = true
        };

        Assert.Equal(10, course.Id);
        Assert.Equal("Advanced Algorithms", course.Name);
        Assert.Equal("In-depth study of algorithms.", course.Description);
        Assert.Equal(start, course.StartDate);
        Assert.Equal(end, course.EndDate);
        Assert.Equal(CourseStatus.InProgress, course.Status);
        Assert.Equal(FinalGrade.A, course.Grade);
        Assert.Equal(4, course.CreditUnits);
        Assert.Equal(2, course.TermId);
        Assert.Equal(5, course.InstructorId);
        Assert.True(course.StartDateNotifications);
        Assert.True(course.EndDateNotifications);
    }

    [Fact]
    public void Assessments_CanBePopulated()
    {
        var course = new Course();
        course.Assessments.Add(new CourseAssessment { Name = "Midterm" });
        Assert.Single(course.Assessments);
        Assert.Equal("Midterm", course.Assessments[0].Name);
    }

    [Fact]
    public void Notes_CanBePopulated()
    {
        var course = new Course();
        course.Notes.Add(new CourseNote { NoteContent = "Review chapter 3" });
        Assert.Single(course.Notes);
        Assert.Equal("Review chapter 3", course.Notes[0].NoteContent);
    }

    [Theory]
    [InlineData(CourseStatus.NotEnrolled)]
    [InlineData(CourseStatus.InProgress)]
    [InlineData(CourseStatus.Completed)]
    [InlineData(CourseStatus.Dropped)]
    [InlineData(CourseStatus.Planned)]
    public void Status_CanBeSetToAnyValidValue(CourseStatus status)
    {
        var course = new Course { Status = status };
        Assert.Equal(status, course.Status);
    }

    [Theory]
    [InlineData(FinalGrade.A)]
    [InlineData(FinalGrade.B)]
    [InlineData(FinalGrade.F)]
    [InlineData(FinalGrade.NotGraded)]
    public void Grade_CanBeSetToAnyValidValue(FinalGrade grade)
    {
        var course = new Course { Grade = grade };
        Assert.Equal(grade, course.Grade);
    }
}
