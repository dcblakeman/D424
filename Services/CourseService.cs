using C_971.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Services
{
    public class CourseService
    {
        private ObservableCollection<AcademicTerm> _academicTerms;
        private List<Course> _courses;

        public CourseService()
        {
            InitializeData();
        }

        private void InitializeData()
        {
            // Initialize Academic Terms
            _academicTerms = new ObservableCollection<AcademicTerm>
            {
                new AcademicTerm { Id = 1, Name = "Spring 2023", StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 4, 30) },
                //new AcademicTerm { Id = 2, Name = "Summer 2023", StartDate = new DateTime(2023, 5, 1), EndDate = new DateTime(2023, 8, 31) },
                //new AcademicTerm { Id = 3, Name = "Fall 2023", StartDate = new DateTime(2023, 9, 1), EndDate = new DateTime(2023, 12, 31) }
            };

            // Initialize Courses (your existing code)
            _courses = new List<Course>()
            {
                new Course
                {
                    TermId = 1,
                    Id = 1,
                    Name = "Mobile App Development",
                    StartDate = new DateTime(2023, 1, 1),
                    EndDate = new DateTime(2023, 6, 1),
                    CourseStatus = Course.Status.Planned,
                    Notes = new ObservableCollection<CourseNote>
                    {
                        new CourseNote { Id = 1, CourseId = 1, NoteContent = "Remember to review MVVM pattern." },
                        new CourseNote { Id = 2, CourseId = 1, NoteContent = "Check out the latest Xamarin updates." }
                    },
                    Instructor = new CourseInstructor
                    {
                        Id = 1,
                        Name = "Anika Patel",
                        Email = "anika.patel@strimeuniversity.edu",
                        Phone = "555-123-4567"
                    },
                    Assessments = new List<CourseAssessment>
                    {
                        new CourseAssessment
                        {
                            Id = 1,
                            Name = "Midterm Project",
                            Type = CourseAssessment.AssessmentType.Objective,
                            Description = "Build a simple mobile app.",
                            StartDate = new DateTime(2023, 3, 1),
                            EndDate = new DateTime(2023, 3, 15),
                        },
                        new CourseAssessment
                        {
                            Id = 2,
                            Name = "Final Project",
                            Type = CourseAssessment.AssessmentType.Performance,
                            Description = "Complete a full-featured mobile app.",
                            StartDate = new DateTime(2023, 5, 1),
                            EndDate = new DateTime(2023, 5, 15),
                        }
                    }
                },

                new Course
                {
                    TermId = 1,
                    Id = 2,
                    Name = "Database Management",
                    StartDate = new DateTime(2023, 2, 1),
                    EndDate = new DateTime(2023, 7, 1),
                    CourseStatus = Course.Status.Planned,
                    Notes = new ObservableCollection<CourseNote>
                    {
                        new CourseNote { Id = 3, CourseId = 2, NoteContent = "Focus on SQL queries." },
                        new CourseNote { Id = 4, CourseId = 2, NoteContent = "Review normalization concepts." }
                    },
                    Instructor = new CourseInstructor
                    {
                        Id = 2,
                        Name = "Jane Smith",
                        Email = "",
                        Phone = "987-654-3210"
                    },
                    Assessments = new List<CourseAssessment>
                    {
                        new CourseAssessment
                        {
                            Id = 3,
                            Name = "SQL Exam",
                            Type = CourseAssessment.AssessmentType.Objective,
                            Description = "Test on SQL basics and advanced queries.",
                            StartDate = new DateTime(2023, 4, 1),
                            EndDate = new DateTime(2023, 4, 15),
                        },
                        new CourseAssessment
                        {
                            Id = 4,
                            Name = "Database Design Project",
                            Type = CourseAssessment.AssessmentType.Performance,
                            Description = "Design and implement a database schema.",
                            StartDate = new DateTime(2023, 6, 1),
                            EndDate = new DateTime(2023, 6, 15),
                        }
                    }
                },

                new Course
                {
                    Id = 3,
                    Name = "Web Development",
                    StartDate = new DateTime(2023, 3, 1),
                    EndDate = new DateTime(2023, 8, 1),
                    CourseStatus = Course.Status.Planned,
                    TermId = 1,
                    Notes = new ObservableCollection<CourseNote>
                    {
                        new CourseNote { Id = 5, CourseId = 3, NoteContent = "Learn about responsive design." },
                        new CourseNote { Id = 6, CourseId = 3, NoteContent = "Explore modern JavaScript frameworks." }
                    },
                    Instructor = new CourseInstructor
                    {
                        Id = 3,
                        Name = "Alice Johnson",
                        Email = "",
                        Phone = "555-123-4567"
                    },
                    Assessments = new List<CourseAssessment>
                    {
                        new CourseAssessment
                        {
                            Id = 5,
                            Name = "Frontend Development Exam",
                            Type = CourseAssessment.AssessmentType.Objective,
                            Description = "Covers HTML, CSS, and JavaScript.",
                            StartDate = new DateTime(2023, 5, 1),
                            EndDate = new DateTime(2023, 5, 15),
                        },
                        new CourseAssessment
                        {
                            Id = 6,
                            Name = "Fullstack Project",
                            Type = CourseAssessment.AssessmentType.Performance,
                            Description = "Build a complete web application.",
                            StartDate = new DateTime(2023, 7, 1),
                            EndDate = new DateTime(2023, 7, 15),
                        }
                    }
                },

                new Course
                {
                    Id = 4,
                    Name = "Cloud Computing",
                    StartDate = new DateTime(2023, 4, 1),
                    EndDate = new DateTime(2023, 9, 1),
                    CourseStatus = Course.Status.Dropped,
                    TermId = 1,
                    Notes = new ObservableCollection<CourseNote>
                    {
                        new CourseNote { Id = 7, CourseId = 4, NoteContent = "Understand cloud service models." },
                        new CourseNote { Id = 8, CourseId = 4, NoteContent = "Explore AWS and Azure platforms." }
                    },
                    Instructor = new CourseInstructor
                    {
                        Id = 4,
                        Name = "Bob Brown",
                        Email = "",
                        Phone = "444-555-6666"
                    },
                    Assessments = new List<CourseAssessment>
                    {
                        new CourseAssessment
                        {
                            Id = 7,
                            Name = "Cloud Fundamentals Exam",
                            Type = CourseAssessment.AssessmentType.Objective,
                            Description = "Basics of cloud computing.",
                            StartDate = new DateTime(2023, 6, 1),
                            EndDate = new DateTime(2023, 6, 15),
                        },
                        new CourseAssessment
                        {
                            Id = 8,
                            Name = "Cloud Deployment Project",
                            Type = CourseAssessment.AssessmentType.Performance,
                            Description = "Deploy an application to the cloud.",
                            StartDate = new DateTime(2023, 8, 1),
                            EndDate = new DateTime(2023, 8, 15),
                        }
                    }
                },

                new Course
                {
                    Id = 5,
                    Name = "Cybersecurity Basics",
                    StartDate = new DateTime(2023, 5, 1),
                    EndDate = new DateTime(2023, 10, 1),
                    CourseStatus = Course.Status.Planned,
                    TermId = 1,
                    Notes = new ObservableCollection<CourseNote>
                    {
                        new CourseNote { Id = 9, CourseId = 5, NoteContent = "Study encryption methods." },
                        new CourseNote { Id = 10, CourseId = 5, NoteContent = "Review network security protocols." }
                    },
                    Instructor = new CourseInstructor
                    {
                        Id = 5,
                        Name = "Charlie Davis",
                        Email = "",
                        Phone = "777-888-9999"
                    },
                    Assessments = new List<CourseAssessment>
                    {
                        new CourseAssessment
                        {
                            Id = 9,
                            Name = "Security Fundamentals Exam",
                            Type = CourseAssessment.AssessmentType.Objective,
                            Description = "Covers basic cybersecurity concepts.",
                            StartDate = new DateTime(2023, 7, 1),
                            EndDate = new DateTime(2023, 7, 15),
                        },
                        new CourseAssessment
                        {
                            Id = 10,
                            Name = "Security Implementation Project",
                            Type = CourseAssessment.AssessmentType.Performance,
                            Description = "Implement security measures for a system.",
                            StartDate = new DateTime(2023, 9, 1),
                            EndDate = new DateTime(2023, 9, 15),
                        }
                    }
                },

                new Course
                {
                    Id = 6,
                    Name = "Data Science Introduction",
                    StartDate = new DateTime(2023, 6, 1),
                    EndDate = new DateTime(2023, 11, 1),
                    CourseStatus = Course.Status.Planned,
                    TermId = 1,
                    Notes = new ObservableCollection<CourseNote>
                    {
                        new CourseNote { Id = 11, CourseId = 6, NoteContent = "Focus on Python for data analysis." },
                        new CourseNote { Id = 12, CourseId = 6, NoteContent = "Explore machine learning basics." }
                    },
                    Instructor = new CourseInstructor
                    {
                        Id = 6,
                        Name = "Diana Evans",
                        Email = "",
                        Phone = "222-333-4444"
                    },
                    Assessments = new List<CourseAssessment>
                    {
                        new CourseAssessment
                        {
                            Id = 11,
                            Name = "Data Analysis Exam",
                            Type = CourseAssessment.AssessmentType.Objective,
                            Description = "Test on data manipulation and analysis techniques.",
                            StartDate = new DateTime(2023, 8, 1),
                            EndDate = new DateTime(2023, 8, 15),
                        },
                        new CourseAssessment
                        {
                            Id = 12,
                            Name = "Machine Learning Project",
                            Type = CourseAssessment.AssessmentType.Performance,
                            Description = "Build a simple machine learning model.",
                            StartDate = new DateTime(2023, 10, 1),
                            EndDate = new DateTime(2023, 10, 15),
                        }
                    }
                }
            };
        }

        public List<Course> GetCourses()
        {
            return _courses;
        }

        public Course GetCourseById(int id)
        {
            return _courses.FirstOrDefault(c => c.Id == id);
        }

        public ObservableCollection<CourseNote> GetCourseNotes(int courseId)
        {
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            return course?.Notes ?? new ObservableCollection<CourseNote>();
        }

        // Get all academic terms
        public ObservableCollection<AcademicTerm> GetAcademicTerms()
        {
            return _academicTerms;
        }

        // Get a single academic term by ID
        public AcademicTerm GetAcademicTerm(int termId)
        {
            return _academicTerms.FirstOrDefault(t => t.Id == termId);
        }

        // Get courses for a specific term
        public List<Course> GetCoursesForTerm(int termId)
        {
            return _courses.Where(c => c.TermId == termId).ToList();
        }

        public Task SaveNote(CourseNote note)
        {
            if (note == null || string.IsNullOrWhiteSpace(note.NoteContent) || note.CourseId <= 0)
            {
                throw new ArgumentException("Invalid note or course ID.");
            }
            return Task.CompletedTask;
        }

        internal void UpdateCourse(Course course)
        {
            // Update the course in the list
            var existingCourse = _courses.FirstOrDefault(c => c.Id == course.Id);
            if (existingCourse != null)
            {
                existingCourse.Name = course.Name;
                existingCourse.StartDate = course.StartDate;
                existingCourse.EndDate = course.EndDate;
                existingCourse.CourseStatus = course.CourseStatus;
                existingCourse.Instructor = course.Instructor;
                existingCourse.Assessments = course.Assessments;
                existingCourse.Notes = course.Notes;
                existingCourse.TermId = course.TermId;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Course with ID {course.Id} not found for update.");
            }
        }
    }
}