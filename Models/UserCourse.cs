using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace C_971.Models
{
    [Table("user_course")]
    public partial class UserCourse : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Foreign keys
        [Column("user_id"), NotNull, Indexed]
        public int UserId { get; set; }

        [Column("course_id"), NotNull, Indexed]
        public int CourseId { get; set; }

        // User-specific enrollment data
        [Column("start_date"), NotNull]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Column("end_date"), NotNull]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [Column("status"), NotNull]
        public CourseStatus Status { get; set; } = CourseStatus.NotEnrolled;

        [Column("start_date_notifications"), NotNull]
        public bool StartDateNotifications { get; set; } = true;

        [Column("end_date_notifications"), NotNull]
        public bool EndDateNotifications { get; set; } = true;

        [Column("enrollment_date")]
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        [Column("grade")]
        public FinalGrade? Grade { get; set; }

        // Navigation properties
        [Ignore]
        public User User { get; set; }

        [Ignore]
        public Course Course { get; set; }
    }
}
