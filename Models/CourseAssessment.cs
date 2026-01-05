using C_971.Models;
using SQLite;

namespace C_971.Models
{
    [Table("course_assessment")]
    public partial class CourseAssessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } = 0;

        [MaxLength(100), NotNull]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        [NotNull]
        [Column("description")]
        public string Description { get; set; } = string.Empty;  

        [NotNull]
        [Column("start_date")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [NotNull]
        [Column("end_date")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(6);

        [Column("completed_date")]
        public DateTime CompletedDate { get; set; } = DateTime.Today.AddMonths(6);

        [Column("type")]
        [NotNull]
        public AssessmentType Type { get; set; } = AssessmentType.Performance;

        [NotNull]
        [Column("status")]
        public AssessmentStatus Status { get; set; } = AssessmentStatus.Pending;

        [NotNull]
        [Column("start_date_notifications")]
        public bool StartDateNotifications { get; set; } = true;

        [NotNull]
        [Column("end_date_notifications")]
        public bool EndDateNotifications { get; set; } = true;

        [NotNull]
        [Column("is_active")]
        public bool IsActive { get; set; } = false;

        [Indexed, NotNull]
        [Column("course_id")]
        public int CourseId { get; set; } = 0;
    }
}