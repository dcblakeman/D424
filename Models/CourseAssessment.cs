using C_971.Models;
using SQLite;

namespace C_971.Models
{
    [Table("CourseAssessment")]
    public partial class CourseAssessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } = 0;

        [MaxLength(100), NotNull]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        [NotNull]
        public string Description { get; set; } = string.Empty;  

        [NotNull]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [NotNull]
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(6);

        [NotNull]
        public DateTime? CompletedDate { get; set; } = null;

        [NotNull]
        public AssessmentType Type { get; set; } = AssessmentType.Performance;

        [NotNull]
        public AssessmentStatus Status { get; set; } = AssessmentStatus.Pending;

        [NotNull]
        public bool StartDateNotifications { get; set; } = true;

        [NotNull]
        public bool EndDateNotifications { get; set; } = true;

        [NotNull]
        public bool IsActive { get; set; } = false;

        [Indexed, NotNull]
        public int CourseId { get; set; } = 0;
    }
}