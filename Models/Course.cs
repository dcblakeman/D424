using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Collections.Generic;

namespace C_971.Models
{
    [Table("Courses")]
    public partial class Course : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("name"), MaxLength(100), NotNull]
        public string Name { get; set; } = string.Empty;

        [Column("course_status"), MaxLength(20), NotNull]
        public CourseStatus Status { get; set; } = CourseStatus.NotEnrolled;

        [Column("description"), MaxLength(250), NotNull]
        public string Description { get; set; } = string.Empty;

        [Column("credit_hours")]
        public int CreditHours { get; set; }

        [Column("instructor_id")]
        public int? InstructorId { get; set; }

        // Course belongs to ONE specific term
        [Column("term_id"), NotNull, Indexed]
        public int TermId { get; set; }

        // Navigation properties
        [Ignore]
        public AcademicTerm Term { get; set; }

        [Ignore]
        public CourseInstructor Instructor { get; set; }

        [Ignore]
        public List<UserCourse> UserCourses { get; set; } = new();
    }
}