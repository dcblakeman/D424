using C_971.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace C_971.Services
{
    public class DatabaseService
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
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to initialize database: {ex.Message}", "OK");
            }
        }

        #region Academic Terms
        public async Task<List<AcademicTerm>> GetTermsAsync()
        {
            await InitializeAsync();
            return await _database.Table<AcademicTerm>().ToListAsync();
        }

        public async Task<AcademicTerm?> GetTermByIdAsync(int termId)
        {
            await InitializeAsync();
            return await _database.Table<AcademicTerm>()
                .FirstOrDefaultAsync(t => t.Id == termId);
        }

        public async Task<int> SaveTermAsync(AcademicTerm term)
        {
            await InitializeAsync();
            return term.Id != 0
                ? await _database.UpdateAsync(term)
                : await _database.InsertAsync(term);
        }

        public async Task<int> DeleteTermAsync(AcademicTerm term)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(term);
        }

        public async Task<List<AcademicTerm>> SearchTermsByNameAsync(string searchText)
        {
            await InitializeAsync();
            return await _database.Table<AcademicTerm>()
                .Where(t => t.Name.Contains(searchText))
                .ToListAsync();
        }
        #endregion

        #region Courses
        public async Task<List<Course>> GetAllCoursesAsync()
        {
            await InitializeAsync();
            return await _database.Table<Course>().ToListAsync();
        }

        public async Task<List<Course>> GetCoursesByTermIdAsync(int termId)
        {
            await InitializeAsync();
            return await _database.Table<Course>()
                .Where(c => c.TermId == termId)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            await InitializeAsync();
            return await _database.Table<Course>()
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<int> SaveCourseAsync(Course course)
        {
            await InitializeAsync();

            try
            {
                System.Diagnostics.Debug.WriteLine($"Database saving course: {course.Id}, Status: {course.Status}");

                int result = course.Id != 0
                    ? await _database.UpdateAsync(course)
                    : await _database.InsertAsync(course);

                System.Diagnostics.Debug.WriteLine($"Database save complete: {result} rows affected");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database save error: {ex}");
                throw;
            }
        }

        public async Task<int> DeleteCourseAsync(Course course)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(course);
        }
        #endregion

        #region Course Notes
        public async Task<List<CourseNote>> GetCourseNotesAsync(int courseId)
        {
            await InitializeAsync();
            return await _database.Table<CourseNote>()
                .Where(n => n.CourseId == courseId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<int> SaveCourseNoteAsync(CourseNote note)
        {
            await InitializeAsync();
            return note.Id != 0
                ? await _database.UpdateAsync(note)
                : await _database.InsertAsync(note);
        }

        public async Task<int> DeleteCourseNoteAsync(CourseNote note)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(note);
        }

        public async Task<int> GetNextCourseNoteIdAsync()
        {
            await InitializeAsync();
            var notes = await _database.Table<CourseNote>()
                .OrderByDescending(n => n.Id)
                .ToListAsync();
            return notes.Count > 0 ? notes[0].Id + 1 : 1;
        }
        #endregion

        #region Course Assessments
        public async Task<List<CourseAssessment>> GetCourseAssessmentsAsync(int courseId)
        {
            await InitializeAsync();
            return await _database.Table<CourseAssessment>()
                .Where(a => a.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<int> SaveCourseAssessmentAsync(CourseAssessment assessment)
        {
            await InitializeAsync();
            return assessment.Id != 0
                ? await _database.UpdateAsync(assessment)
                : await _database.InsertAsync(assessment);
        }

        public async Task<int> DeleteCourseAssessmentAsync(CourseAssessment assessment)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(assessment);
        }

        public async Task<int> GetNextAssessmentIdAsync()
        {
            await InitializeAsync();
            var assessments = await _database.Table<CourseAssessment>()
                .OrderByDescending(a => a.Id)
                .ToListAsync();
            return assessments.Count > 0 ? assessments[0].Id + 1 : 1;
        }
        #endregion

        #region Course Instructors
        public async Task<List<CourseInstructor>> GetCourseInstructorsAsync()
        {
            await InitializeAsync();
            return await _database.Table<CourseInstructor>().ToListAsync();
        }

        public async Task<CourseInstructor?> GetCourseInstructorByIdAsync(int instructorId)
        {
            await InitializeAsync();
            return await _database.Table<CourseInstructor>()
                .FirstOrDefaultAsync(i => i.Id == instructorId);
        }

        public async Task<int> SaveCourseInstructorAsync(CourseInstructor instructor)
        {
            await InitializeAsync();
            return instructor.Id != 0
                ? await _database.UpdateAsync(instructor)
                : await _database.InsertAsync(instructor);
        }

        public async Task<int> DeleteCourseInstructorAsync(CourseInstructor instructor)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(instructor);
        }

        public async Task<int> GetNextCourseInstructorIdAsync()
        {
            await InitializeAsync();
            var instructors = await _database.Table<CourseInstructor>()
                .OrderByDescending(i => i.Id)
                .ToListAsync();
            return instructors.Count > 0 ? instructors[0].Id + 1 : 1;
        }

        internal async Task<int> GetMaxAssessmentIdAsync()
        {
            throw new NotImplementedException();
        }

        internal async Task AddCourseInstructorAsync(CourseInstructor newInstructor)
        {
            throw new NotImplementedException();
        }

        internal async Task<IEnumerable<object>> GetCourseNotesByCourseIdAsync(int id)
        {
            //Retrieve course notes by course ID
            await InitializeAsync();
            return await _database.Table<CourseNote>()
                .Where(n => n.CourseId == id)
                .ToListAsync();
        }

        internal async Task<CourseInstructor> GetInstructorByIdAsync(int? instructorId)
        {
            //Retrieve instructor by ID
            await InitializeAsync();
            return await _database.Table<CourseInstructor>()
                .FirstOrDefaultAsync(i => i.Id == instructorId);
        }
        #endregion
    }
}