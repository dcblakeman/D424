using C_971.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace C_971.Services
{
    public interface IDatabaseService
    {
        Task InitializeAsync();

        // Terms
        Task<List<AcademicTerm>> GetTermsAsync();
        Task<int> SaveTermAsync(AcademicTerm term);
        Task<IEnumerable<AcademicTerm>> SearchTermsByNameAsync(string newValue);

        // Courses
        Task<List<Course>> GetCoursesAsync();
        Task<List<Course>> GetCoursesByTermIdAsync(int termId);
        Task<Course?> GetCourseByIdAsync(int courseId);
        Task<int> SaveCourseAsync(Course course);
        Task<int> DeleteCourseAsync(Course course);

        // Notes
        Task<List<CourseNote>> GetNotesByCourseAsync(int courseId);

        // Assessments
        Task<List<CourseAssessment>> GetAssessmentsByCourseAsync(int courseId);
        Task DeleteTermAsync(AcademicTerm term);
        Task AddCourse(Course newCourse);
    }
}
