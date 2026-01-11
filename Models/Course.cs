using SQLite;

namespace C_971.Models
{
    [Table("course")]
    public partial class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("name"), MaxLength(100)]
        [NotNull]
        public string Name { get; set; } = string.Empty;

        [Column("description"), NotNull]
        public string Description { get; set; } = string.Empty;

        [Column("start_date"), NotNull]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Column("end_date"), NotNull]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(6);

        [Column("status"), NotNull]
        public CourseStatus Status { get; set; }

        [Column("start_date_notifications"), NotNull]
        public bool StartDateNotifications { get; set; } = true;

        [Column("end_date_notifications"), NotNull]
        public bool EndDateNotifications { get; set; } = true;

        [Column("credit_units"), NotNull]
        public int CreditUnits { get; set; } = 3;

        [Column("grade"), NotNull]
        public FinalGrade Grade { get; set; }

        // Foreign keys
        [Column("term_id"), NotNull]
        public int TermId { get; set; }

        [Column("instructor_id"), NotNull]
        public int InstructorId { get; set; }

        // Navigation properties - ignored by SQLite
        [Ignore]
        public AcademicTerm? Term { get; set; }

        [Ignore]
        public CourseInstructor? Instructor { get; set; }

        [Ignore]
        public List<CourseAssessment> Assessments { get; set; } = [];

        [Ignore]
        public List<CourseNote> Notes { get; set; } = [];
    }
}