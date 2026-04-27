using C_971.Models;

namespace C_971.Tests.Models;

public class CourseWithAssessmentsTests
{
    [Fact]
    public void CanCreateWithRequiredProperties()
    {
        var course = new Course { Id = 1, Name = "CS101" };
        var assessments = new List<CourseAssessment>
        {
            new() { Id = 1, Name = "Midterm", Type = AssessmentType.Objective },
            new() { Id = 2, Name = "Final", Type = AssessmentType.Performance }
        };

        var cwa = new CourseWithAssessments
        {
            Course = course,
            Assessments = assessments
        };

        Assert.Equal(course, cwa.Course);
        Assert.Equal(assessments, cwa.Assessments);
    }

    [Fact]
    public void Assessments_CanBeEmpty()
    {
        var cwa = new CourseWithAssessments
        {
            Course = new Course { Id = 1, Name = "CS101" },
            Assessments = []
        };

        Assert.NotNull(cwa.Assessments);
        Assert.Empty(cwa.Assessments);
    }

    [Fact]
    public void Assessments_ReflectsCorrectCount()
    {
        var assessments = new List<CourseAssessment>
        {
            new() { Name = "Quiz 1", Type = AssessmentType.Objective },
            new() { Name = "Project", Type = AssessmentType.Performance }
        };

        var cwa = new CourseWithAssessments
        {
            Course = new Course(),
            Assessments = assessments
        };

        Assert.Equal(2, cwa.Assessments.Count);
    }

    [Fact]
    public void Course_ReflectsCorrectData()
    {
        var start = new DateTime(2025, 1, 10);
        var end = new DateTime(2025, 6, 10);
        var course = new Course
        {
            Id = 42,
            Name = "Web Dev",
            StartDate = start,
            EndDate = end,
            Status = CourseStatus.Completed,
            Grade = FinalGrade.BPlus
        };

        var cwa = new CourseWithAssessments
        {
            Course = course,
            Assessments = []
        };

        Assert.Equal(42, cwa.Course.Id);
        Assert.Equal("Web Dev", cwa.Course.Name);
        Assert.Equal(start, cwa.Course.StartDate);
        Assert.Equal(end, cwa.Course.EndDate);
        Assert.Equal(CourseStatus.Completed, cwa.Course.Status);
        Assert.Equal(FinalGrade.BPlus, cwa.Course.Grade);
    }
}
