using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Collections.Generic;

namespace C_971.Models
{
    [Table("Courses")]
    public partial class Course : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } = 0;

        [NotNull]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CreditUnits { get; set; } = 3;

        public CourseStatus Status { get; set; } = CourseStatus.NotEnrolled;

        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(6);

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Notification settings
        public bool StartDateNotifications { get; set; } = false;

        public bool EndDateNotifications { get; set; } = false;

        // Foreign keys
        [NotNull]
        public int TermId { get; set; } = 0;

        public int? InstructorId { get; set; } = null;

        // Navigation properties - ignored by SQLite
        [Ignore]
        public AcademicTerm Term { get; set; } = null!;

        [Ignore]
        public CourseInstructor Instructor { get; set; } = null!;

        [Ignore]
        public List<CourseAssessment> Assessments { get; set; } = new();

        [Ignore]
        public List<CourseNote> Notes { get; set; } = new();
    }
}