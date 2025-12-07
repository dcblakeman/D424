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

            try
            {
                var databasePath = Path.Combine(FileSystem.AppDataDirectory, "CollegeCourseTracker.db");
                _database = new SQLiteAsyncConnection(databasePath);

                await _database.CreateTableAsync<AcademicTerm>();
                await _database.CreateTableAsync<Course>();
                await _database.CreateTableAsync<CourseNote>();
                await _database.CreateTableAsync<CourseAssessment>();
                await _database.CreateTableAsync<CourseInstructor>();
            }
            catch (Exception ex)
            {
                // Handle initialization exceptions
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to initialize database: {ex.Message}", "OK");
            }
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

        public async Task<List<CourseNote>> GetCourseNotesByCourseIdAsync(int id)
        {
            await InitializeAsync();
            return await _database.Table<CourseNote>()
                         .Where(n => n.CourseId == id)
                         .ToListAsync();
        }

        public async Task DeleteCourseNoteAsync(CourseNote note)
        {
            await InitializeAsync();
            await _database.DeleteAsync(note);
            return;
        }

        public async Task UpdateCourse(Course course)
        {
            await InitializeAsync();
            await _database.UpdateAsync(course);
            return;
        }

        public async Task GetAllCourses()
        {
            await InitializeAsync();
            await _database.Table<Course>().ToListAsync();
            return;
        }

        public async Task GetCoursesForTerm(int id)
        {
            await InitializeAsync();
            await _database.Table<Course>().Where(c => c.TermId == id).ToListAsync();
            return;
        }

        public async Task<IEnumerable<object>> GetNotesForCourseAsync(int id)
        {
            await InitializeAsync();
            return await _database.Table<CourseNote>()
                .Where(n => n.CourseId == id)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task LoadNotes()
        {
            await InitializeAsync();
            await _database.Table<CourseNote>().ToListAsync();
        }

        public async Task<int> DeleteTermAsync(AcademicTerm term)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(term);
        }

        Task IDatabaseService.DeleteTermAsync(AcademicTerm term)
        {
            return DeleteTermAsync(term);
        }

        public async Task AddCourse(Course newCourse)
        {
            await InitializeAsync();
            await _database.InsertAsync(newCourse);
            await _database.UpdateAsync(newCourse);
        }

        public async Task SaveInstructorAsync(CourseInstructor newInstructor)
        {
            await InitializeAsync();
            await _database.InsertAsync(newInstructor);
            await _database.UpdateAsync(newInstructor);
        }

        public async Task<CourseInstructor> GetInstructorByIdAsync(int? instructorId)
        {
            await InitializeAsync();
            return await _database.Table<CourseInstructor>().FirstOrDefaultAsync(i => i.Id == instructorId);
        }

        internal async Task GetCourseNotesAsync(int id)
        {
            throw new NotImplementedException();
        }

        internal async Task GetTermByIdAsync(int termId)
        {
            throw new NotImplementedException();
        }

        internal async Task<int> GetMaxAssessmentIdAsync()
        {
            await InitializeAsync();
            var assessments = await _database.Table<CourseAssessment>().OrderByDescending(a => a.Id).ToListAsync();
            return assessments.Count > 0 ? assessments[0].Id : 0;
        }

        internal async Task<int> GetNextCourseNoteIdAsync()
        {
            await InitializeAsync();
            var notes = await _database.Table<CourseNote>().OrderByDescending(n => n.Id).ToListAsync();
            return notes.Count > 0 ? notes[0].Id + 1 : 1;
        }

        internal async Task SaveCourseNoteAsync(CourseNote newNote)
        {
            await InitializeAsync();
            await _database.InsertAsync(newNote);
            await _database.UpdateAsync(newNote);
        }
    }
}
