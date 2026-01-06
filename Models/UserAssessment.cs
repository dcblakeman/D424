using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace C_971.Models
{
    [Table("user_assessment")]
    public class UserAssessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("user_course_id"), NotNull]
        public int UserCourseId { get; set; }

        [Column("assessment_id"), NotNull]
        public int AssessmentId { get; set; }

        [Column("grade")]
        public FinalGrade? Grade { get; set; }

        // Optional: Add these for more functionality
        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation properties
        [Ignore]
        public UserCourse UserCourse { get; set; }

        [Ignore]
        public CourseAssessment Assessment { get; set; }

        // Computed properties
        [Ignore]
        public string GradeDisplay => Grade?.ToString() ?? "Not Graded";
    }
}
