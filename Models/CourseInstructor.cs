using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Models
{
    [SQLite.Table("CourseInstructor")]
    public partial class CourseInstructor
    {
        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Instructor Name
        [SQLite.MaxLength(100), NotNull]
        public string Name { get; set; }

        // Instructor Email
        [SQLite.MaxLength(100)]
        [NotNull]
        public string Email { get; set; }

        // Instructor Phone
        [SQLite.MaxLength(20)]
        [NotNull]
        [RegularExpression(@"^(\+[1-9]\d{10,14}|[1-9]\d{2}-\d{3}-\d{4})$",
            ErrorMessage = "Phone must be in format 555-123-4567")]
        public string Phone { get; set; }
    }
}
