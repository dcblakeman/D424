using C_971.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace C_971.Services
{
    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection? _database;

        public async Task InitializeAsync()
        {
            if (_database is not null) return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "CollegeCourseTracker.db");
            _database = new SQLiteAsyncConnection(databasePath);

            await _database.CreateTableAsync<AcademicTerm>();
            await _database.CreateTableAsync<Course>();
            await _database.CreateTableAsync<CourseNote>();
            await _database.CreateTableAsync<CourseAssessment>();
        }

        // Terms
        public async Task<List<AcademicTerm>> GetTermsAsync()
        {
            await InitializeAsync();
            return await _database.Table<AcademicTerm>().ToListAsync();
        }

        public async Task<int> SaveTermAsync(AcademicTerm term)
        {
            await InitializeAsync();
            return term.Id != 0
                ? await _database.UpdateAsync(term)
                : await _database.InsertAsync(term);
        }

        public async Task<IEnumerable<AcademicTerm>> SearchTermsByNameAsync(string newValue)
        {
            await InitializeAsync();
            return await _database.Table<AcademicTerm>()
                .Where(t => t.Name.Contains(newValue))
                .ToListAsync();
        }

        // Courses
        public async Task<List<Course>> GetCoursesAsync()
        {
            await InitializeAsync();
            return await _database.Table<Course>().ToListAsync();
        }

        public async Task<List<Course>> GetCoursesByTermAsync(int termId)
        {
            await InitializeAsync();
            return await _database.Table<Course>().Where(c => c.TermId == termId).ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            await InitializeAsync();
            return await _database.Table<Course>().FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<int> SaveCourseAsync(Course course)
        {
            await InitializeAsync();
            return course.Id != 0
                ? await _database.UpdateAsync(course)
                : await _database.InsertAsync(course);
        }

        public async Task<int> DeleteCourseAsync(Course course)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(course);
        }

        // Notes
        public async Task<List<CourseNote>> GetNotesByCourseAsync(int courseId)
        {
            await InitializeAsync();
            return await _database.Table<CourseNote>()
                .Where(n => n.CourseId == courseId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        // Assessments
        public async Task<List<CourseAssessment>> GetAssessmentsByCourseAsync(int courseId)
        {
            await InitializeAsync();
            return await _database.Table<CourseAssessment>()
                .Where(a => a.CourseId == courseId)
                .ToListAsync();
        }

        internal async Task<IEnumerable<object>> GetCourseNotesByCourseIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        internal async Task DeleteCourseNoteAsync(CourseNote note)
        {
            throw new NotImplementedException();
        }

        internal void UpdateCourse(Course course)
        {
            throw new NotImplementedException();
        }

        internal List<Course> GetAllCourses()
        {
            throw new NotImplementedException();
        }

        internal List<Course> GetCoursesForTerm(int id)
        {
            throw new NotImplementedException();
        }

        internal async Task<IEnumerable<object>> GetNotesForCourseAsync(int id)
        {
            throw new NotImplementedException();
        }

        internal async Task LoadNotes()
        {
            throw new NotImplementedException();
        }
    }
}
