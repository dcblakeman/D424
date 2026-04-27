using C_971.Models;
using SQLite;

namespace C_971.Tests.Helpers;

/// <summary>
/// Creates and initializes an in-memory SQLite database for testing
/// database business logic without requiring the MAUI runtime.
/// </summary>
public class TestDatabase : IAsyncDisposable
{
    public SQLiteAsyncConnection Connection { get; }

    private TestDatabase(SQLiteAsyncConnection connection)
    {
        Connection = connection;
    }

    public static async Task<TestDatabase> CreateAsync()
    {
        // Use a unique in-memory DB per test run to keep tests isolated
        var connection = new SQLiteAsyncConnection(":memory:");

        await connection.CreateTableAsync<AcademicTerm>();
        await connection.CreateTableAsync<Course>();
        await connection.CreateTableAsync<CourseNote>();
        await connection.CreateTableAsync<CourseAssessment>();
        await connection.CreateTableAsync<CourseInstructor>();
        await connection.CreateTableAsync<User>();
        await connection.CreateTableAsync<UserCourse>();

        return new TestDatabase(connection);
    }

    // ── Academic Terms ──────────────────────────────────────────────────────

    public Task<List<AcademicTerm>> GetTermsAsync() =>
        Connection.Table<AcademicTerm>().ToListAsync();

    public async Task<AcademicTerm?> GetTermByIdAsync(int id) =>
        await Connection.Table<AcademicTerm>().FirstOrDefaultAsync(t => t.Id == id);

    public Task<int> SaveTermAsync(AcademicTerm term) =>
        term.Id != 0
            ? Connection.UpdateAsync(term)
            : Connection.InsertAsync(term);

    public Task<int> DeleteTermAsync(AcademicTerm term) =>
        Connection.DeleteAsync(term);

    public Task<List<AcademicTerm>> SearchTermsByNameAsync(string text) =>
        Connection.Table<AcademicTerm>()
            .Where(t => t.Name.Contains(text))
            .ToListAsync();

    // ── Courses ─────────────────────────────────────────────────────────────

    public Task<List<Course>> GetAllCoursesAsync() =>
        Connection.Table<Course>().ToListAsync();

    public Task<List<Course>> GetCoursesByTermIdAsync(int termId) =>
        Connection.Table<Course>().Where(c => c.TermId == termId).ToListAsync();

    public async Task<Course?> GetCourseByIdAsync(int id) =>
        await Connection.Table<Course>().FirstOrDefaultAsync(c => c.Id == id);

    public Task<int> SaveCourseAsync(Course course) =>
        course.Id != 0
            ? Connection.UpdateAsync(course)
            : Connection.InsertAsync(course);

    public Task<int> DeleteCourseAsync(Course course) =>
        Connection.DeleteAsync(course);

    // ── Course Notes ────────────────────────────────────────────────────────

    public Task<List<CourseNote>> GetCourseNotesAsync(int courseId) =>
        Connection.Table<CourseNote>()
            .Where(n => n.CourseId == courseId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();

    public Task<int> SaveCourseNoteAsync(CourseNote note) =>
        note.Id != 0
            ? Connection.UpdateAsync(note)
            : Connection.InsertAsync(note);

    public Task<int> DeleteCourseNoteAsync(CourseNote note) =>
        Connection.DeleteAsync(note);

    public async Task<int> GetNextCourseNoteIdAsync()
    {
        List<CourseNote> notes = await Connection.Table<CourseNote>()
            .OrderByDescending(n => n.Id)
            .ToListAsync();
        return notes.Count > 0 ? notes[0].Id + 1 : 1;
    }

    // ── Assessments ─────────────────────────────────────────────────────────

    public Task<List<CourseAssessment>> GetCourseAssessmentsAsync(int courseId) =>
        Connection.Table<CourseAssessment>()
            .Where(a => a.CourseId == courseId)
            .ToListAsync();

    public Task<int> SaveCourseAssessmentAsync(CourseAssessment assessment) =>
        assessment.Id != 0
            ? Connection.UpdateAsync(assessment)
            : Connection.InsertAsync(assessment);

    public Task<int> DeleteCourseAssessmentAsync(CourseAssessment assessment) =>
        Connection.DeleteAsync(assessment);

    public async Task<int> GetNextAssessmentIdAsync()
    {
        List<CourseAssessment> list = await Connection.Table<CourseAssessment>()
            .OrderByDescending(a => a.Id)
            .ToListAsync();
        return list.Count > 0 ? list[0].Id + 1 : 1;
    }

    public async Task<bool> CanAddAssessment(int courseId, AssessmentType assessmentType)
    {
        CourseAssessment? existing = await Connection.Table<CourseAssessment>()
            .FirstOrDefaultAsync(a => a.CourseId == courseId && a.Type == assessmentType);
        return existing == null;
    }

    public async Task<bool> ValidateCourseAssessments(int courseId)
    {
        List<CourseAssessment> assessments = await Connection.Table<CourseAssessment>()
            .Where(a => a.CourseId == courseId)
            .ToListAsync();
        bool hasPerformance = assessments.Any(a => a.Type == AssessmentType.Performance);
        bool hasObjective = assessments.Any(a => a.Type == AssessmentType.Objective);
        return hasPerformance && hasObjective && assessments.Count == 2;
    }

    public Task<List<CourseAssessment>> GetAssessmentsByTypeAsync(AssessmentType type) =>
        Connection.Table<CourseAssessment>().Where(a => a.Type == type).ToListAsync();

    public async Task<CourseAssessment?> GetAssessmentByCourseIdAndTypeAsync(int courseId, AssessmentType type) =>
        await Connection.Table<CourseAssessment>()
            .FirstOrDefaultAsync(a => a.CourseId == courseId && a.Type == type);

    // ── Instructors ─────────────────────────────────────────────────────────

    public Task<List<CourseInstructor>> GetInstructorsAsync() =>
        Connection.Table<CourseInstructor>().ToListAsync();

    public async Task<CourseInstructor?> GetInstructorByIdAsync(int id) =>
        await Connection.Table<CourseInstructor>().FirstOrDefaultAsync(i => i.Id == id);

    public Task<int> SaveInstructorAsync(CourseInstructor instructor) =>
        instructor.Id != 0
            ? Connection.UpdateAsync(instructor)
            : Connection.InsertAsync(instructor);

    public Task<int> DeleteInstructorAsync(CourseInstructor instructor) =>
        Connection.DeleteAsync(instructor);

    public async Task<int> GetNextInstructorIdAsync()
    {
        List<CourseInstructor> list = await Connection.Table<CourseInstructor>()
            .OrderByDescending(i => i.Id)
            .ToListAsync();
        return list.Count > 0 ? list[0].Id + 1 : 1;
    }

    // ── Users ────────────────────────────────────────────────────────────────

    public async Task<bool> CreateUserAsync(string email, string password)
    {
        string hashed = BCrypt.Net.BCrypt.HashPassword(password);
        try
        {
            await Connection.InsertAsync(new User { Email = email, HashedPassword = hashed });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AuthenticateUserAsync(string email, string password)
    {
        User? user = await Connection.Table<User>().FirstOrDefaultAsync(u => u.Email == email);
        return user != null && BCrypt.Net.BCrypt.Verify(password, user.HashedPassword);
    }

    public async Task<bool> IsEmailRegisteredAsync(string email)
    {
        User? user = await Connection.Table<User>().FirstOrDefaultAsync(u => u.Email == email);
        return user != null;
    }

    // ── User Courses ─────────────────────────────────────────────────────────

    public Task<List<UserCourse>> GetUserCoursesAsync(int userId) =>
        Connection.Table<UserCourse>().Where(uc => uc.UserId == userId).ToListAsync();

    public Task SaveUserCourseAsync(UserCourse uc) =>
        uc.Id != 0 ? Connection.UpdateAsync(uc) : Connection.InsertAsync(uc);

    public async ValueTask DisposeAsync() => await Connection.CloseAsync();
}
