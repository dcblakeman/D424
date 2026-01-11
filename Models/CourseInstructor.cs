using SQLite;
using System.ComponentModel.DataAnnotations;

namespace C_971.Models
{
    [SQLite.Table("course_instructor")]
    public partial class CourseInstructor
    {
        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Instructor Name
        [Column("name")]
        [SQLite.MaxLength(100)]
        public string? Name { get; set; }

        // Instructor Email
        [Column("email")]
        [SQLite.MaxLength(100)]
        public string? Email { get; set; }

        // Instructor Phone
        [Column("phone")]
        [SQLite.MaxLength(20)]
        [RegularExpression(@"^(\+[1-9]\d{10,14}|[1-9]\d{2}-\d{3}-\d{4})$",
            ErrorMessage = "Phone must be in format 555-123-4567")]
        public string? Phone { get; set; }
    }
}
