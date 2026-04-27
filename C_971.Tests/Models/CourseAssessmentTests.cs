using C_971.Models;

namespace C_971.Tests.Models;

public class CourseAssessmentTests
{
    [Fact]
    public void DefaultConstructor_SetsIdToZero()
    {
        var assessment = new CourseAssessment();
        Assert.Equal(0, assessment.Id);
    }

    [Fact]
    public void DefaultConstructor_SetsNameToEmptyString()
    {
        var assessment = new CourseAssessment();
        Assert.Equal(string.Empty, assessment.Name);
    }

    [Fact]
    public void DefaultConstructor_SetsDescriptionToEmptyString()
    {
        var assessment = new CourseAssessment();
        Assert.Equal(string.Empty, assessment.Description);
    }

    [Fact]
    public void DefaultConstructor_SetsTypeToPerformance()
    {
        var assessment = new CourseAssessment();
        Assert.Equal(AssessmentType.Performance, assessment.Type);
    }

    [Fact]
    public void DefaultConstructor_SetsStatusToPending()
    {
        var assessment = new CourseAssessment();
        Assert.Equal(AssessmentStatus.Pending, assessment.Status);
    }

    [Fact]
    public void DefaultConstructor_SetsGradeToNotGraded()
    {
        var assessment = new CourseAssessment();
        Assert.Equal(FinalGrade.NotGraded, assessment.Grade);
    }

    [Fact]
    public void DefaultConstructor_SetsIsActiveToFalse()
    {
        var assessment = new CourseAssessment();
        Assert.False(assessment.IsActive);
    }

    [Fact]
    public void DefaultConstructor_SetsStartDateNotificationsToFalse()
    {
        var assessment = new CourseAssessment();
        Assert.False(assessment.StartDateNotifications);
    }

    [Fact]
    public void DefaultConstructor_SetsEndDateNotificationsToFalse()
    {
        var assessment = new CourseAssessment();
        Assert.False(assessment.EndDateNotifications);
    }

    [Fact]
    public void DefaultConstructor_SetsCourseIdToZero()
    {
        var assessment = new CourseAssessment();
        Assert.Equal(0, assessment.CourseId);
    }

    [Fact]
    public void DefaultConstructor_SetsStartDateToNow()
    {
        var before = DateTime.Now;
        var assessment = new CourseAssessment();
        var after = DateTime.Now;
        Assert.InRange(assessment.StartDate, before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void DefaultConstructor_SetsEndDateToSixMonthsFromNow()
    {
        var before = DateTime.Now.AddMonths(6);
        var assessment = new CourseAssessment();
        var after = DateTime.Now.AddMonths(6);
        Assert.InRange(assessment.EndDate, before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void DefaultConstructor_SetsCompletedDateToSixMonthsFromToday()
    {
        var expected = DateTime.Today.AddMonths(6);
        var assessment = new CourseAssessment();
        Assert.Equal(expected, assessment.CompletedDate);
    }

    [Fact]
    public void CanSetAllCoreProperties()
    {
        var start = new DateTime(2025, 3, 1);
        var end = new DateTime(2025, 3, 15);
        var assessment = new CourseAssessment
        {
            Id = 5,
            Name = "Midterm Project",
            Description = "Build a mobile app",
            StartDate = start,
            EndDate = end,
            Type = AssessmentType.Objective,
            Status = AssessmentStatus.InProgress,
            Grade = FinalGrade.B,
            IsActive = true,
            CourseId = 3,
            StartDateNotifications = true,
            EndDateNotifications = true
        };

        Assert.Equal(5, assessment.Id);
        Assert.Equal("Midterm Project", assessment.Name);
        Assert.Equal("Build a mobile app", assessment.Description);
        Assert.Equal(start, assessment.StartDate);
        Assert.Equal(end, assessment.EndDate);
        Assert.Equal(AssessmentType.Objective, assessment.Type);
        Assert.Equal(AssessmentStatus.InProgress, assessment.Status);
        Assert.Equal(FinalGrade.B, assessment.Grade);
        Assert.True(assessment.IsActive);
        Assert.Equal(3, assessment.CourseId);
        Assert.True(assessment.StartDateNotifications);
        Assert.True(assessment.EndDateNotifications);
    }

    [Theory]
    [InlineData(AssessmentType.Objective)]
    [InlineData(AssessmentType.Performance)]
    [InlineData(AssessmentType.Unknown)]
    public void Type_CanBeSetToAnyValidValue(AssessmentType type)
    {
        var assessment = new CourseAssessment { Type = type };
        Assert.Equal(type, assessment.Type);
    }

    [Theory]
    [InlineData(AssessmentStatus.Pending)]
    [InlineData(AssessmentStatus.InProgress)]
    [InlineData(AssessmentStatus.Completed)]
    [InlineData(AssessmentStatus.Overdue)]
    public void Status_CanBeSetToAnyValidValue(AssessmentStatus status)
    {
        var assessment = new CourseAssessment { Status = status };
        Assert.Equal(status, assessment.Status);
    }
}
