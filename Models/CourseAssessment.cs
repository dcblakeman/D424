using C_971.Models;
using SQLite;

namespace C_971.Models
{
    [Table("CourseAssessment")]
    public partial class CourseAssessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100), NotNull]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;  

        [NotNull]
        public DateTime StartDate { get; set; }  
        [NotNull]
        public DateTime EndDate { get; set; }  
        public DateTime? CompletedDate { get; set; } = null;

        [NotNull]
        public AssessmentType Type { get; set; } 

        [NotNull]
        public AssessmentStatus Status { get; set; } = AssessmentStatus.Pending;

        public bool StartDateNotifications { get; set; } = true;
        public bool EndDateNotifications { get; set; } = true;

        [Indexed, NotNull]
        public int CourseId { get; set; }
    }
}