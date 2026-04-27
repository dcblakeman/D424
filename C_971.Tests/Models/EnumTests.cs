using C_971.Models;

namespace C_971.Tests.Models;

public class EnumTests
{
    [Fact]
    public void CourseStatus_HasExpectedValues()
    {
        Assert.Equal(0, (int)CourseStatus.NotEnrolled);
        Assert.Equal(1, (int)CourseStatus.InProgress);
        Assert.Equal(2, (int)CourseStatus.Completed);
        Assert.Equal(3, (int)CourseStatus.Dropped);
        Assert.Equal(4, (int)CourseStatus.Planned);
    }

    [Fact]
    public void CourseStatus_HasFiveMembers()
    {
        string[] names = Enum.GetNames(typeof(CourseStatus));
        Assert.Equal(5, names.Length);
    }

    [Fact]
    public void AssessmentType_HasExpectedValues()
    {
        Assert.Equal(0, (int)AssessmentType.Objective);
        Assert.Equal(1, (int)AssessmentType.Performance);
        Assert.Equal(2, (int)AssessmentType.Unknown);
    }

    [Fact]
    public void AssessmentType_HasThreeMembers()
    {
        string[] names = Enum.GetNames(typeof(AssessmentType));
        Assert.Equal(3, names.Length);
    }

    [Fact]
    public void AssessmentStatus_HasExpectedValues()
    {
        Assert.Equal(0, (int)AssessmentStatus.Pending);
        Assert.Equal(1, (int)AssessmentStatus.InProgress);
        Assert.Equal(2, (int)AssessmentStatus.Completed);
        Assert.Equal(3, (int)AssessmentStatus.Overdue);
    }

    [Fact]
    public void AssessmentStatus_HasFourMembers()
    {
        string[] names = Enum.GetNames(typeof(AssessmentStatus));
        Assert.Equal(4, names.Length);
    }

    [Fact]
    public void FinalGrade_ContainsAllExpectedMembers()
    {
        var expected = new[]
        {
            "A", "AMinus", "BPlus", "B", "BMinus", "CPlus", "C", "CMinus",
            "DPlus", "D", "DMinus", "F", "NotGraded"
        };
        string[] actual = Enum.GetNames(typeof(FinalGrade));
        Assert.Equal(expected.Length, actual.Length);
        foreach (string name in expected)
        {
            Assert.Contains(name, actual);
        }
    }

    [Fact]
    public void FinalGrade_NotGraded_IsLastEntry()
    {
        string[] names = Enum.GetNames(typeof(FinalGrade));
        Assert.Equal("NotGraded", names[^1]);
    }
}
