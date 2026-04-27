using C_971.Models;

namespace C_971.Tests.Models;

public class CourseNoteTests
{
    [Fact]
    public void DefaultConstructor_SetsIdToZero()
    {
        var note = new CourseNote();
        Assert.Equal(0, note.Id);
    }

    [Fact]
    public void DefaultConstructor_SetsNoteContentToEmptyString()
    {
        var note = new CourseNote();
        Assert.Equal(string.Empty, note.NoteContent);
    }

    [Fact]
    public void DefaultConstructor_SetsCourseIdToZero()
    {
        var note = new CourseNote();
        Assert.Equal(0, note.CourseId);
    }

    [Fact]
    public void DefaultConstructor_SetsCreatedDateToNow()
    {
        var before = DateTime.Now;
        var note = new CourseNote();
        var after = DateTime.Now;
        Assert.InRange(note.CreatedDate, before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void CanSetNoteContent()
    {
        var note = new CourseNote { NoteContent = "Study chapter 5 for the exam." };
        Assert.Equal("Study chapter 5 for the exam.", note.NoteContent);
    }

    [Fact]
    public void CanSetCourseId()
    {
        var note = new CourseNote { CourseId = 7 };
        Assert.Equal(7, note.CourseId);
    }

    [Fact]
    public void CanSetId()
    {
        var note = new CourseNote { Id = 100 };
        Assert.Equal(100, note.Id);
    }

    [Fact]
    public void CanSetCreatedDate()
    {
        var date = new DateTime(2025, 6, 15, 10, 30, 0);
        var note = new CourseNote { CreatedDate = date };
        Assert.Equal(date, note.CreatedDate);
    }

    [Fact]
    public void CanSetAllProperties()
    {
        var date = new DateTime(2025, 4, 1);
        var note = new CourseNote
        {
            Id = 42,
            NoteContent = "Review normalization concepts.",
            CourseId = 3,
            CreatedDate = date
        };

        Assert.Equal(42, note.Id);
        Assert.Equal("Review normalization concepts.", note.NoteContent);
        Assert.Equal(3, note.CourseId);
        Assert.Equal(date, note.CreatedDate);
    }
}
