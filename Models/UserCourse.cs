using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace C_971.Models
{
    [Table("user_course")]
    public partial class UserCourse
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("course_id")]
        public int CourseId { get; set; }
        [Column("start_date")]
        public DateTime StartDate { get; set; } = DateTime.Now;
        [Column("end_date")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(6);
        [Column("grade")]
        public FinalGrade Grade { get; set; }
        // Navigation properties
        [Ignore]
        public User? User { get; set; }

        [Ignore]
        public Course? Course { get; set; }
    }
}
