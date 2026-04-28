using C_971.Models;
using SQLite;

namespace C_971.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;

        public async Task InitializeAsync()
        {
            if (_database is not null)
            {
                return;
            }

            //For development -delete and recreate if schema changed
            //if (File.Exists(Constants.DatabasePath))
            //{
            //    File.Delete(Constants.DatabasePath);
            //}

            try
            {
                _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
                _ = await _database.CreateTableAsync<AcademicTerm>();
                _ = await _database.CreateTableAsync<Course>();
                _ = await _database.CreateTableAsync<CourseNote>();
                _ = await _database.CreateTableAsync<CourseAssessment>();
                _ = await _database.CreateTableAsync<CourseInstructor>();
                _ = await _database.CreateTableAsync<User>();
                _ = await _database.CreateTableAsync<UserCourse>();

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
            List<CourseNote> notes = await _database.Table<CourseNote>()
                .OrderByDescending(n => n.Id)
                .ToListAsync();
            return notes.Count > 0 ? notes[0].Id + 1 : 1;
        }
        #endregion



        #region Course Assessments
        public async Task<IEnumerable<CourseAssessment>> GetCourseAssessmentsAsync(int courseId)
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
            List<CourseAssessment> assessments = await _database.Table<CourseAssessment>()
                .OrderByDescending(a => a.Id)
                .ToListAsync();
            return assessments.Count > 0 ? assessments[0].Id + 1 : 1;
        }

        // In your DatabaseService
        public async Task<IEnumerable<CourseAssessment>> GetCourseAssessmentsByCourseIdAsync(int courseId)
        {
            await InitializeAsync();
            return await _database.Table<CourseAssessment>()
                .Where(a => a.CourseId == courseId)
                .ToListAsync();
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
            List<CourseInstructor> instructors = await _database.Table<CourseInstructor>()
                .OrderByDescending(i => i.Id)
                .ToListAsync();
            return instructors.Count > 0 ? instructors[0].Id + 1 : 1;
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

        public async Task<bool> CanAddAssessment(int courseId, AssessmentType assessmentType)
        {
            CourseAssessment existingAssessment = await _database.Table<CourseAssessment>()
                .FirstOrDefaultAsync(a => a.CourseId == courseId && a.Type == assessmentType);

            return existingAssessment == null;
        }

        public async Task<bool> ValidateCourseAssessments(int courseId)
        {
            List<CourseAssessment> assessments = await _database.Table<CourseAssessment>()
                .Where(a => a.CourseId == courseId)
                .ToListAsync();

            bool hasPerformance = assessments.Any(a => a.Type == AssessmentType.Performance);
            bool hasObjective = assessments.Any(a => a.Type == AssessmentType.Objective);

            return hasPerformance && hasObjective && assessments.Count == 2;
        }

        public async Task DeleteAssessmentAsync(int id)
        {
            await InitializeAsync();
            CourseAssessment assessment = await _database.FindAsync<CourseAssessment>(id);
            if (assessment != null)
            {
                _ = await _database.DeleteAsync(assessment);
            }
        }

        public async Task<List<CourseAssessment>> GetAssessmentsByTypeAsync(AssessmentType assessmentType)
        {
            await InitializeAsync();
            return await _database.Table<CourseAssessment>()
                .Where(a => a.Type == assessmentType)
                .ToListAsync();
        }

        //Get course assessment by course ID and type
        internal async Task<CourseAssessment?> GetCourseAssessmentByCourseIdAndTypeAsync(int courseId, AssessmentType type)
        {
            await InitializeAsync();
            return await _database.Table<CourseAssessment>()
                .FirstOrDefaultAsync(a => a.CourseId == courseId && a.Type == type);
        }

        internal async Task<IEnumerable<CourseAssessment>> GetCourseAssessmentsByType(AssessmentType assessmentType)
        {
            await InitializeAsync();
            return await _database.Table<CourseAssessment>()
                .Where(a => a.Type == assessmentType)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseAssessment>> GetAllAssessmentsAsync()
        {
            await InitializeAsync();
            return await _database.Table<CourseAssessment>().ToListAsync();
        }

        public async Task<CourseAssessment> GetAssessmentbyCourseId(int courseId)
        {
            await InitializeAsync();
            AsyncTableQuery<CourseAssessment> assessment = _database.Table<CourseAssessment>()
                .Where(a => a.CourseId == courseId);

            return await assessment.FirstOrDefaultAsync();
        }

        public async Task<CourseAssessment> GetAssessmentbyCourseIdAndType(int courseId, AssessmentType type)
        {
            await InitializeAsync();
            AsyncTableQuery<CourseAssessment> assessment = _database.Table<CourseAssessment>()
                .Where(a => a.CourseId == courseId && a.Type == type);
            return await assessment.FirstOrDefaultAsync();

        }

        internal async Task<CourseAssessment> GetAssessmentbyCourseIdAndTypeAndIsActive(int courseId, AssessmentType assessmentType, bool assessmentIsActive)
        {
            await InitializeAsync();
            AsyncTableQuery<CourseAssessment> assessment = _database.Table<CourseAssessment>()
                .Where(a => a.CourseId == courseId && a.Type == assessmentType && a.IsActive == assessmentIsActive);
            return await assessment.FirstOrDefaultAsync();
        }

        public async Task<bool> CreateUserAsync(string email, string password)
        {
            await InitializeAsync();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            User newUser = new()
            {
                Email = email,
                HashedPassword = hashedPassword
            };

            try
            {
                _ = await _database.InsertAsync(newUser);
                return true;
            }
            catch
            {
                return false;
            }

        }

        internal async Task<int> GetUserIdByEmailAsync(string email)
        {
            await InitializeAsync();
            User user = await _database.Table<User>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                int userId = user.Id;
                return userId;
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Error", "User not found.", "OK");
                return -1; // Indicate user not found
            }

        }

        internal async Task<bool> AuthenticateUserAsync(string email, string password)
        {
            await InitializeAsync();
            User user = await _database.Table<User>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                // Here you would normally hash the input password and compare it to the stored hash
                //BCrypt.Net.BCrypt.HashPassword(password);
                bool isValid = BCrypt.Net.BCrypt.Verify(password, user.HashedPassword);
                return isValid;
            }
            return false;
        }

        internal async Task<bool> IsEmailRegisteredAsync(string email)
        {
            await InitializeAsync();
            User user = await _database.Table<User>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
            return user != null;
        }

        internal async Task<AcademicTerm?> GetTermByNameAsync(string name)
        {

            await InitializeAsync();
            AcademicTerm term = await _database.Table<AcademicTerm>()
                .Where(t => t.Name == name)
                .FirstOrDefaultAsync();
            if (term != null)
            {
                return term;
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Error", "Term not found.", "OK");
                return null;
            }
        }

        internal async Task<Course?> GetCourseByNameAsync(string name)
        {
            await InitializeAsync();
            Course course = await _database.Table<Course>()
                .Where(c => c.Name == name)
                .FirstOrDefaultAsync();
            if (course != null)
            {
                return course;
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Error", "Course not found.", "OK");
                return null;
            }
        }

        public async Task<List<UserCourse>> GetUserCoursesAsync(int userId)
        {
            return await _database.Table<UserCourse>()
                .Where(uc => uc.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<UserCourse>> GetUserCoursesWithDetailsAsync(int userId)
        {
            List<UserCourse> userCourses = await _database.Table<UserCourse>()
                .Where(uc => uc.UserId == userId)
                .ToListAsync();

            // Load the actual course data for each enrollment
            foreach (UserCourse userCourse in userCourses)
            {
                userCourse.Course = await _database.Table<Course>()
                    .FirstOrDefaultAsync(c => c.Id == userCourse.CourseId);
            }

            return userCourses;
        }

        // Add this to your DatabaseService

        public async Task<List<Course>> GetCoursesWithDetailsAsync(int userId, int termId)
        {
            string query = @"
                SELECT DISTINCT 
                    c.id,
                    c.name,
                    c.description,
                    c.start_date,
                    c.end_date,
                    c.status,
                    c.start_date_notifications,
                    c.end_date_notifications,
                    c.credit_units,
                    c.grade,
                    c.term_id,
                    c.instructor_id
                FROM course c
                INNER JOIN user_course uc ON c.id = uc.course_id
                WHERE uc.user_id = ? AND c.term_id = ?
                ORDER BY c.start_date ASC";

            return await _database.QueryAsync<Course>(query, userId, termId);
        }

        public async Task<List<CourseAssessment>> GetAssessmentsForUserAndTermAsync(int userId, int termId)
        {
            string query = @"
                            SELECT DISTINCT a.*
                            FROM course_assessment a
                            INNER JOIN course c ON a.course_id = c.id
                            INNER JOIN user_course uc ON c.id = uc.course_id
                            WHERE uc.user_id = ? AND c.term_id = ?
                            ORDER BY a.end_date ASC";

            return await _database.QueryAsync<CourseAssessment>(query, userId, termId);
        }

        public async Task<List<CourseWithAssessments>> GetCoursesWithAssessmentsAsync(int userId, int termId)
        {
            // First get all courses for the user and term
            string coursesQuery = @"
                SELECT DISTINCT c.*
                FROM course c
                INNER JOIN user_course uc ON c.id = uc.course_id
                INNER JOIN academic_term t ON c.term_id = t.id
                WHERE uc.user_id = ? AND c.term_id = ?
                ORDER BY c.start_date ASC";

            List<Course> courses = await _database.QueryAsync<Course>(coursesQuery, userId, termId);

            List<CourseWithAssessments> result = [];

            foreach (Course course in courses)
            {
                // Get assessments for each course
                string assessmentsQuery = @"
                    SELECT * FROM course_assessment
                    WHERE course_id = ?
                    ORDER BY end_date ASC";

                List<CourseAssessment> assessments = await _database.QueryAsync<CourseAssessment>(assessmentsQuery, course.Id);

                result.Add(new CourseWithAssessments
                {
                    Course = course,
                    Assessments = assessments
                });
            }

            return result;
        }

        internal async Task<User?> GetUserByEmailAsync(string newLoginUserEmail)
        {
            await InitializeAsync();
            User user = await _database.Table<User>()
                .Where(u => u.Email == newLoginUserEmail)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                return user;
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Error", "User not found.", "OK");
                return null;
            }
        }

        internal async Task SaveUserCourseAsync(UserCourse userCourse)
        {
            await InitializeAsync();
            _ = userCourse.Id != 0 ? await _database.UpdateAsync(userCourse) : await _database.InsertAsync(userCourse);
        }
    }
}
