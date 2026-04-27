using C_971.Models;

namespace C_971.Tests.Models;

public class UserCourseTests
{
    [Fact]
    public void DefaultConstructor_SetsIdToZero()
    {
        var uc = new UserCourse();
        Assert.Equal(0, uc.Id);
    }

    [Fact]
    public void DefaultConstructor_SetsUserIdToZero()
    {
        var uc = new UserCourse();
        Assert.Equal(0, uc.UserId);
    }

    [Fact]
    public void DefaultConstructor_SetsCourseIdToZero()
    {
        var uc = new UserCourse();
        Assert.Equal(0, uc.CourseId);
    }

    [Fact]
    public void DefaultConstructor_SetsGradeToFirstEnumValue()
    {
        var uc = new UserCourse();
        Assert.Equal(default(FinalGrade), uc.Grade);
    }

    [Fact]
    public void DefaultConstructor_SetsStartDateToNow()
    {
        var before = DateTime.Now;
        var uc = new UserCourse();
        var after = DateTime.Now;
        Assert.InRange(uc.StartDate, before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void DefaultConstructor_SetsEndDateToSixMonthsFromNow()
    {
        var before = DateTime.Now.AddMonths(6);
        var uc = new UserCourse();
        var after = DateTime.Now.AddMonths(6);
        Assert.InRange(uc.EndDate, before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void DefaultConstructor_SetsUserToNull()
    {
        var uc = new UserCourse();
        Assert.Null(uc.User);
    }

    [Fact]
    public void DefaultConstructor_SetsCourseToNull()
    {
        var uc = new UserCourse();
        Assert.Null(uc.Course);
    }

    [Fact]
    public void CanSetUserId()
    {
        var uc = new UserCourse { UserId = 3 };
        Assert.Equal(3, uc.UserId);
    }

    [Fact]
    public void CanSetCourseId()
    {
        var uc = new UserCourse { CourseId = 7 };
        Assert.Equal(7, uc.CourseId);
    }

    [Fact]
    public void CanSetGrade()
    {
        var uc = new UserCourse { Grade = FinalGrade.A };
        Assert.Equal(FinalGrade.A, uc.Grade);
    }

    [Fact]
    public void CanSetNavigationProperties()
    {
        var user = new User { Id = 1, Email = "test@test.com" };
        var course = new Course { Id = 2, Name = "Math" };
        var uc = new UserCourse { User = user, Course = course };
        Assert.Equal(user, uc.User);
        Assert.Equal(course, uc.Course);
    }
}
