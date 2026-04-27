using C_971.Models;
using C_971.Tests.Helpers;

namespace C_971.Tests.Services;

public class DatabaseServiceTests : IAsyncLifetime
{
    private TestDatabase _db = null!;

    public async Task InitializeAsync()
    {
        _db = await TestDatabase.CreateAsync();
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
    }

    // ═══════════════════════════════════════════════════════
    // Academic Term CRUD
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task SaveTermAsync_Insert_AssignsAutoIncrementId()
    {
        var term = new AcademicTerm { Name = "Fall 2025", StartDate = new DateTime(2025, 9, 1), EndDate = new DateTime(2025, 12, 31) };
        await _db.SaveTermAsync(term);
        List<AcademicTerm> terms = await _db.GetTermsAsync();
        Assert.Single(terms);
        Assert.True(terms[0].Id > 0);
    }

    [Fact]
    public async Task SaveTermAsync_Update_ModifiesExistingRecord()
    {
        var term = new AcademicTerm { Name = "Spring 2025", StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 5, 31) };
        await _db.SaveTermAsync(term);
        AcademicTerm? inserted = (await _db.GetTermsAsync()).First();

        inserted.Name = "Spring 2025 (Updated)";
        await _db.SaveTermAsync(inserted);

        AcademicTerm? updated = await _db.GetTermByIdAsync(inserted.Id);
        Assert.Equal("Spring 2025 (Updated)", updated!.Name);
    }

    [Fact]
    public async Task GetTermByIdAsync_ReturnsCorrectTerm()
    {
        await _db.SaveTermAsync(new AcademicTerm { Name = "Term A", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(6) });
        await _db.SaveTermAsync(new AcademicTerm { Name = "Term B", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(6) });

        List<AcademicTerm> all = await _db.GetTermsAsync();
        AcademicTerm first = all[0];

        AcademicTerm? found = await _db.GetTermByIdAsync(first.Id);
        Assert.NotNull(found);
        Assert.Equal(first.Name, found.Name);
    }

    [Fact]
    public async Task GetTermByIdAsync_ReturnsNull_WhenNotFound()
    {
        AcademicTerm? found = await _db.GetTermByIdAsync(9999);
        Assert.Null(found);
    }

    [Fact]
    public async Task DeleteTermAsync_RemovesTermFromDatabase()
    {
        var term = new AcademicTerm { Name = "To Delete", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(3) };
        await _db.SaveTermAsync(term);
        AcademicTerm? saved = (await _db.GetTermsAsync()).First();

        await _db.DeleteTermAsync(saved);
        List<AcademicTerm> remaining = await _db.GetTermsAsync();
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task SearchTermsByNameAsync_ReturnsMatchingTerms()
    {
        await _db.SaveTermAsync(new AcademicTerm { Name = "Fall 2024", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(4) });
        await _db.SaveTermAsync(new AcademicTerm { Name = "Spring 2025", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(4) });

        List<AcademicTerm> results = await _db.SearchTermsByNameAsync("Fall");
        Assert.Single(results);
        Assert.Equal("Fall 2024", results[0].Name);
    }

    [Fact]
    public async Task SearchTermsByNameAsync_ReturnsEmpty_WhenNoMatch()
    {
        await _db.SaveTermAsync(new AcademicTerm { Name = "Fall 2024", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(4) });

        List<AcademicTerm> results = await _db.SearchTermsByNameAsync("Summer");
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetTermsAsync_ReturnsAllTerms()
    {
        await _db.SaveTermAsync(new AcademicTerm { Name = "Term 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(4) });
        await _db.SaveTermAsync(new AcademicTerm { Name = "Term 2", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(4) });
        await _db.SaveTermAsync(new AcademicTerm { Name = "Term 3", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(4) });

        List<AcademicTerm> terms = await _db.GetTermsAsync();
        Assert.Equal(3, terms.Count);
    }

    // ═══════════════════════════════════════════════════════
    // Course CRUD
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task SaveCourseAsync_Insert_AssignsAutoIncrementId()
    {
        var course = new Course { Name = "CS101", TermId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(4) };
        await _db.SaveCourseAsync(course);

        List<Course> courses = await _db.GetAllCoursesAsync();
        Assert.Single(courses);
        Assert.True(courses[0].Id > 0);
    }

    [Fact]
    public async Task SaveCourseAsync_Update_ModifiesExistingRecord()
    {
        var course = new Course { Name = "CS101", TermId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(4) };
        await _db.SaveCourseAsync(course);
        Course? inserted = (await _db.GetAllCoursesAsync()).First();

        inserted.Name = "CS101 Advanced";
        await _db.SaveCourseAsync(inserted);

        Course? updated = await _db.GetCourseByIdAsync(inserted.Id);
        Assert.Equal("CS101 Advanced", updated!.Name);
    }

    [Fact]
    public async Task GetCoursesByTermIdAsync_ReturnsOnlyMatchingCourses()
    {
        await _db.SaveCourseAsync(new Course { Name = "Course A", TermId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(4) });
        await _db.SaveCourseAsync(new Course { Name = "Course B", TermId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(4) });
        await _db.SaveCourseAsync(new Course { Name = "Course C", TermId = 2, StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(4) });

        List<Course> term1Courses = await _db.GetCoursesByTermIdAsync(1);
        Assert.Equal(2, term1Courses.Count);
        Assert.All(term1Courses, c => Assert.Equal(1, c.TermId));
    }

    [Fact]
    public async Task GetCourseByIdAsync_ReturnsCorrectCourse()
    {
        await _db.SaveCourseAsync(new Course { Name = "CS201", TermId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(4) });
        Course? saved = (await _db.GetAllCoursesAsync()).First();

        Course? found = await _db.GetCourseByIdAsync(saved.Id);
        Assert.NotNull(found);
        Assert.Equal("CS201", found.Name);
    }

    [Fact]
    public async Task GetCourseByIdAsync_ReturnsNull_WhenNotFound()
    {
        Course? found = await _db.GetCourseByIdAsync(99999);
        Assert.Null(found);
    }

    [Fact]
    public async Task DeleteCourseAsync_RemovesCourseFromDatabase()
    {
        var course = new Course { Name = "To Delete", TermId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(4) };
        await _db.SaveCourseAsync(course);
        Course? saved = (await _db.GetAllCoursesAsync()).First();

        await _db.DeleteCourseAsync(saved);
        List<Course> remaining = await _db.GetAllCoursesAsync();
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task SaveCourseAsync_PreservesCourseStatus()
    {
        var course = new Course { Name = "CS303", TermId = 1, Status = CourseStatus.InProgress, StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(4) };
        await _db.SaveCourseAsync(course);

        Course? saved = (await _db.GetAllCoursesAsync()).First();
        Assert.Equal(CourseStatus.InProgress, saved.Status);
    }

    [Fact]
    public async Task SaveCourseAsync_PreservesGrade()
    {
        var course = new Course { Name = "CS404", TermId = 1, Grade = FinalGrade.AMinus, StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(4) };
        await _db.SaveCourseAsync(course);

        Course? saved = (await _db.GetAllCoursesAsync()).First();
        Assert.Equal(FinalGrade.AMinus, saved.Grade);
    }

    // ═══════════════════════════════════════════════════════
    // Course Notes
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task SaveCourseNoteAsync_Insert_AssignsAutoIncrementId()
    {
        var note = new CourseNote { CourseId = 1, NoteContent = "Review chapter 3", CreatedDate = DateTime.Now };
        await _db.SaveCourseNoteAsync(note);

        List<CourseNote> notes = await _db.GetCourseNotesAsync(1);
        Assert.Single(notes);
        Assert.True(notes[0].Id > 0);
    }

    [Fact]
    public async Task GetCourseNotesAsync_ReturnsOnlyNotesForCourse()
    {
        await _db.SaveCourseNoteAsync(new CourseNote { CourseId = 1, NoteContent = "Note A", CreatedDate = DateTime.Now });
        await _db.SaveCourseNoteAsync(new CourseNote { CourseId = 1, NoteContent = "Note B", CreatedDate = DateTime.Now });
        await _db.SaveCourseNoteAsync(new CourseNote { CourseId = 2, NoteContent = "Note C", CreatedDate = DateTime.Now });

        List<CourseNote> notes = await _db.GetCourseNotesAsync(1);
        Assert.Equal(2, notes.Count);
        Assert.All(notes, n => Assert.Equal(1, n.CourseId));
    }

    [Fact]
    public async Task GetCourseNotesAsync_ReturnsInDescendingDateOrder()
    {
        var older = new CourseNote { CourseId = 1, NoteContent = "Older note", CreatedDate = DateTime.Now.AddDays(-2) };
        var newer = new CourseNote { CourseId = 1, NoteContent = "Newer note", CreatedDate = DateTime.Now };

        await _db.SaveCourseNoteAsync(older);
        await _db.SaveCourseNoteAsync(newer);

        List<CourseNote> notes = await _db.GetCourseNotesAsync(1);
        Assert.Equal("Newer note", notes[0].NoteContent);
        Assert.Equal("Older note", notes[1].NoteContent);
    }

    [Fact]
    public async Task SaveCourseNoteAsync_Update_ModifiesContent()
    {
        var note = new CourseNote { CourseId = 1, NoteContent = "Original content", CreatedDate = DateTime.Now };
        await _db.SaveCourseNoteAsync(note);
        CourseNote saved = (await _db.GetCourseNotesAsync(1)).First();

        saved.NoteContent = "Updated content";
        await _db.SaveCourseNoteAsync(saved);

        List<CourseNote> updated = await _db.GetCourseNotesAsync(1);
        Assert.Equal("Updated content", updated[0].NoteContent);
    }

    [Fact]
    public async Task DeleteCourseNoteAsync_RemovesNote()
    {
        await _db.SaveCourseNoteAsync(new CourseNote { CourseId = 1, NoteContent = "To delete", CreatedDate = DateTime.Now });
        CourseNote saved = (await _db.GetCourseNotesAsync(1)).First();

        await _db.DeleteCourseNoteAsync(saved);
        List<CourseNote> remaining = await _db.GetCourseNotesAsync(1);
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task GetNextCourseNoteIdAsync_ReturnsOneWhenEmpty()
    {
        int nextId = await _db.GetNextCourseNoteIdAsync();
        Assert.Equal(1, nextId);
    }

    [Fact]
    public async Task GetNextCourseNoteIdAsync_ReturnsMaxIdPlusOne()
    {
        await _db.SaveCourseNoteAsync(new CourseNote { CourseId = 1, NoteContent = "First", CreatedDate = DateTime.Now });
        await _db.SaveCourseNoteAsync(new CourseNote { CourseId = 1, NoteContent = "Second", CreatedDate = DateTime.Now });

        int nextId = await _db.GetNextCourseNoteIdAsync();
        List<CourseNote> allNotes = await _db.GetCourseNotesAsync(1);
        int expectedMax = allNotes.Max(n => n.Id);
        Assert.Equal(expectedMax + 1, nextId);
    }

    // ═══════════════════════════════════════════════════════
    // Course Assessments
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task SaveCourseAssessmentAsync_Insert_AssignsAutoIncrementId()
    {
        var assessment = new CourseAssessment { CourseId = 1, Name = "Midterm", Type = AssessmentType.Objective };
        await _db.SaveCourseAssessmentAsync(assessment);

        List<CourseAssessment> saved = await _db.GetCourseAssessmentsAsync(1);
        Assert.Single(saved);
        Assert.True(saved[0].Id > 0);
    }

    [Fact]
    public async Task GetCourseAssessmentsAsync_ReturnsOnlyAssessmentsForCourse()
    {
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 1, Name = "Objective", Type = AssessmentType.Objective });
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 1, Name = "Performance", Type = AssessmentType.Performance });
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 2, Name = "Other", Type = AssessmentType.Objective });

        List<CourseAssessment> course1 = await _db.GetCourseAssessmentsAsync(1);
        Assert.Equal(2, course1.Count);
        Assert.All(course1, a => Assert.Equal(1, a.CourseId));
    }

    [Fact]
    public async Task DeleteCourseAssessmentAsync_RemovesAssessment()
    {
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 1, Name = "To Delete", Type = AssessmentType.Objective });
        CourseAssessment saved = (await _db.GetCourseAssessmentsAsync(1)).First();

        await _db.DeleteCourseAssessmentAsync(saved);
        List<CourseAssessment> remaining = await _db.GetCourseAssessmentsAsync(1);
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task GetNextAssessmentIdAsync_ReturnsOneWhenEmpty()
    {
        int nextId = await _db.GetNextAssessmentIdAsync();
        Assert.Equal(1, nextId);
    }

    [Fact]
    public async Task GetNextAssessmentIdAsync_ReturnsMaxIdPlusOne()
    {
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 1, Name = "A1", Type = AssessmentType.Objective });
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 1, Name = "A2", Type = AssessmentType.Performance });

        int nextId = await _db.GetNextAssessmentIdAsync();
        List<CourseAssessment> all = await _db.GetCourseAssessmentsAsync(1);
        int maxId = all.Max(a => a.Id);
        Assert.Equal(maxId + 1, nextId);
    }

    [Fact]
    public async Task GetAssessmentsByTypeAsync_ReturnsCorrectType()
    {
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 1, Name = "Obj1", Type = AssessmentType.Objective });
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 2, Name = "Perf1", Type = AssessmentType.Performance });
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 3, Name = "Obj2", Type = AssessmentType.Objective });

        List<CourseAssessment> objectives = await _db.GetAssessmentsByTypeAsync(AssessmentType.Objective);
        Assert.Equal(2, objectives.Count);
        Assert.All(objectives, a => Assert.Equal(AssessmentType.Objective, a.Type));
    }

    [Fact]
    public async Task GetAssessmentByCourseIdAndTypeAsync_ReturnsMatch()
    {
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 5, Name = "Final", Type = AssessmentType.Performance });

        CourseAssessment? found = await _db.GetAssessmentByCourseIdAndTypeAsync(5, AssessmentType.Performance);
        Assert.NotNull(found);
        Assert.Equal("Final", found.Name);
    }

    [Fact]
    public async Task GetAssessmentByCourseIdAndTypeAsync_ReturnsNull_WhenNotFound()
    {
        CourseAssessment? found = await _db.GetAssessmentByCourseIdAndTypeAsync(99, AssessmentType.Objective);
        Assert.Null(found);
    }

    // ═══════════════════════════════════════════════════════
    // ValidateCourseAssessments
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task ValidateCourseAssessments_ReturnsTrue_WhenBothTypesPresent()
    {
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 10, Name = "Obj", Type = AssessmentType.Objective });
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 10, Name = "Perf", Type = AssessmentType.Performance });

        bool valid = await _db.ValidateCourseAssessments(10);
        Assert.True(valid);
    }

    [Fact]
    public async Task ValidateCourseAssessments_ReturnsFalse_WhenNoAssessments()
    {
        bool valid = await _db.ValidateCourseAssessments(11);
        Assert.False(valid);
    }

    [Fact]
    public async Task ValidateCourseAssessments_ReturnsFalse_WhenOnlyObjective()
    {
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 12, Name = "Obj", Type = AssessmentType.Objective });

        bool valid = await _db.ValidateCourseAssessments(12);
        Assert.False(valid);
    }

    [Fact]
    public async Task ValidateCourseAssessments_ReturnsFalse_WhenOnlyPerformance()
    {
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 13, Name = "Perf", Type = AssessmentType.Performance });

        bool valid = await _db.ValidateCourseAssessments(13);
        Assert.False(valid);
    }

    [Fact]
    public async Task ValidateCourseAssessments_ReturnsFalse_WhenMoreThanTwoAssessments()
    {
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 14, Name = "Obj", Type = AssessmentType.Objective });
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 14, Name = "Perf", Type = AssessmentType.Performance });
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 14, Name = "Extra", Type = AssessmentType.Objective });

        bool valid = await _db.ValidateCourseAssessments(14);
        Assert.False(valid);
    }

    // ═══════════════════════════════════════════════════════
    // CanAddAssessment
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task CanAddAssessment_ReturnsTrue_WhenTypeNotPresent()
    {
        bool canAdd = await _db.CanAddAssessment(20, AssessmentType.Objective);
        Assert.True(canAdd);
    }

    [Fact]
    public async Task CanAddAssessment_ReturnsFalse_WhenTypeAlreadyExists()
    {
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 21, Name = "Existing", Type = AssessmentType.Performance });

        bool canAdd = await _db.CanAddAssessment(21, AssessmentType.Performance);
        Assert.False(canAdd);
    }

    [Fact]
    public async Task CanAddAssessment_ReturnsTrue_ForDifferentType()
    {
        await _db.SaveCourseAssessmentAsync(new CourseAssessment { CourseId = 22, Name = "Perf", Type = AssessmentType.Performance });

        bool canAddObjective = await _db.CanAddAssessment(22, AssessmentType.Objective);
        Assert.True(canAddObjective);
    }

    // ═══════════════════════════════════════════════════════
    // Course Instructors
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task SaveInstructorAsync_Insert_AssignsAutoIncrementId()
    {
        var instructor = new CourseInstructor { Name = "Dr. Smith", Email = "smith@uni.edu", Phone = "555-100-2000" };
        await _db.SaveInstructorAsync(instructor);

        List<CourseInstructor> list = await _db.GetInstructorsAsync();
        Assert.Single(list);
        Assert.True(list[0].Id > 0);
    }

    [Fact]
    public async Task SaveInstructorAsync_Update_ModifiesExistingRecord()
    {
        var instructor = new CourseInstructor { Name = "Prof. Lee", Email = "lee@uni.edu" };
        await _db.SaveInstructorAsync(instructor);
        CourseInstructor? saved = (await _db.GetInstructorsAsync()).First();

        saved.Name = "Prof. Lee (Updated)";
        await _db.SaveInstructorAsync(saved);

        CourseInstructor? updated = await _db.GetInstructorByIdAsync(saved.Id);
        Assert.Equal("Prof. Lee (Updated)", updated!.Name);
    }

    [Fact]
    public async Task GetInstructorByIdAsync_ReturnsCorrectInstructor()
    {
        await _db.SaveInstructorAsync(new CourseInstructor { Name = "Instructor A" });
        await _db.SaveInstructorAsync(new CourseInstructor { Name = "Instructor B" });

        List<CourseInstructor> all = await _db.GetInstructorsAsync();
        CourseInstructor first = all[0];

        CourseInstructor? found = await _db.GetInstructorByIdAsync(first.Id);
        Assert.NotNull(found);
        Assert.Equal(first.Name, found.Name);
    }

    [Fact]
    public async Task GetInstructorByIdAsync_ReturnsNull_WhenNotFound()
    {
        CourseInstructor? found = await _db.GetInstructorByIdAsync(99999);
        Assert.Null(found);
    }

    [Fact]
    public async Task DeleteInstructorAsync_RemovesInstructor()
    {
        await _db.SaveInstructorAsync(new CourseInstructor { Name = "To Remove" });
        CourseInstructor? saved = (await _db.GetInstructorsAsync()).First();

        await _db.DeleteInstructorAsync(saved);
        List<CourseInstructor> remaining = await _db.GetInstructorsAsync();
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task GetNextInstructorIdAsync_ReturnsOneWhenEmpty()
    {
        int nextId = await _db.GetNextInstructorIdAsync();
        Assert.Equal(1, nextId);
    }

    [Fact]
    public async Task GetNextInstructorIdAsync_ReturnsMaxIdPlusOne()
    {
        await _db.SaveInstructorAsync(new CourseInstructor { Name = "I1" });
        await _db.SaveInstructorAsync(new CourseInstructor { Name = "I2" });

        int nextId = await _db.GetNextInstructorIdAsync();
        List<CourseInstructor> all = await _db.GetInstructorsAsync();
        int maxId = all.Max(i => i.Id);
        Assert.Equal(maxId + 1, nextId);
    }

    // ═══════════════════════════════════════════════════════
    // User Authentication & Registration
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task CreateUserAsync_ReturnsTrue_WhenSuccessful()
    {
        bool result = await _db.CreateUserAsync("test@example.com", "SecurePass123");
        Assert.True(result);
    }

    [Fact]
    public async Task IsEmailRegisteredAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        bool registered = await _db.IsEmailRegisteredAsync("nobody@nowhere.com");
        Assert.False(registered);
    }

    [Fact]
    public async Task IsEmailRegisteredAsync_ReturnsTrue_AfterRegistration()
    {
        await _db.CreateUserAsync("alice@example.com", "password");
        bool registered = await _db.IsEmailRegisteredAsync("alice@example.com");
        Assert.True(registered);
    }

    [Fact]
    public async Task AuthenticateUserAsync_ReturnsTrue_WithCorrectCredentials()
    {
        await _db.CreateUserAsync("bob@example.com", "MyPassword99");
        bool auth = await _db.AuthenticateUserAsync("bob@example.com", "MyPassword99");
        Assert.True(auth);
    }

    [Fact]
    public async Task AuthenticateUserAsync_ReturnsFalse_WithWrongPassword()
    {
        await _db.CreateUserAsync("carol@example.com", "CorrectPassword");
        bool auth = await _db.AuthenticateUserAsync("carol@example.com", "WrongPassword");
        Assert.False(auth);
    }

    [Fact]
    public async Task AuthenticateUserAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        bool auth = await _db.AuthenticateUserAsync("ghost@example.com", "anypassword");
        Assert.False(auth);
    }

    [Fact]
    public async Task CreateUserAsync_HashesPasswordCorrectly()
    {
        await _db.CreateUserAsync("dave@example.com", "PlainTextPass");
        // Verify the stored password is not plain text
        User? user = await _db.Connection.Table<User>().FirstOrDefaultAsync(u => u.Email == "dave@example.com");
        Assert.NotNull(user);
        Assert.NotEqual("PlainTextPass", user.HashedPassword);
        // Verify BCrypt can verify it
        bool valid = BCrypt.Net.BCrypt.Verify("PlainTextPass", user.HashedPassword);
        Assert.True(valid);
    }

    // ═══════════════════════════════════════════════════════
    // UserCourse Enrollment
    // ═══════════════════════════════════════════════════════

    [Fact]
    public async Task SaveUserCourseAsync_Insert_StoresEnrollment()
    {
        var uc = new UserCourse { UserId = 1, CourseId = 5 };
        await _db.SaveUserCourseAsync(uc);

        List<UserCourse> enrollments = await _db.GetUserCoursesAsync(1);
        Assert.Single(enrollments);
        Assert.Equal(5, enrollments[0].CourseId);
    }

    [Fact]
    public async Task GetUserCoursesAsync_ReturnsOnlyForSpecifiedUser()
    {
        await _db.SaveUserCourseAsync(new UserCourse { UserId = 1, CourseId = 1 });
        await _db.SaveUserCourseAsync(new UserCourse { UserId = 1, CourseId = 2 });
        await _db.SaveUserCourseAsync(new UserCourse { UserId = 2, CourseId = 3 });

        List<UserCourse> user1 = await _db.GetUserCoursesAsync(1);
        Assert.Equal(2, user1.Count);
        Assert.All(user1, uc => Assert.Equal(1, uc.UserId));
    }

    [Fact]
    public async Task GetUserCoursesAsync_ReturnsEmpty_WhenNoEnrollments()
    {
        List<UserCourse> enrollments = await _db.GetUserCoursesAsync(999);
        Assert.Empty(enrollments);
    }
}
