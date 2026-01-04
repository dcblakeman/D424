using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace C_971.Models
{
    [Table("Courses")]
    public partial class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.Today;

        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(6);

        public CourseStatus Status { get; set; } = CourseStatus.Planned;

        public bool StartDateNotifications { get; set; } = true;

        public bool EndDateNotifications { get; set; } = true;

        public int CreditUnits { get; set; } = 3;

        public FinalGrade Grade { get; set; } = FinalGrade.NotGraded;

        // Foreign keys
        [NotNull]
        public int TermId { get; set; }

        public int InstructorId { get; set; }

        // Navigation properties - ignored by SQLite
        [Ignore]
        public AcademicTerm? Term { get; set; }

        [Ignore]
        public CourseInstructor? Instructor { get; set; }

        [Ignore]
        public List<CourseAssessment> Assessments { get; set; } = new();

        [Ignore]
        public List<CourseNote> Notes { get; set; } = new();
    }
}