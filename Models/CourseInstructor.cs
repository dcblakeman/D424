using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Models
{
    [SQLite.Table("CourseInstructor")]
    public partial class CourseInstructor
    {
        // Primary Key
        [PrimaryKey, AutoIncrement, Unique, NotNull]
        public int Id { get; set; }

        // Instructor Name
        [SQLite.MaxLength(100), NotNull]
        public string Name { get; set; }

        // Instructor Email
        [SQLite.MaxLength(100)]
        public string Email { get; set; }

        // Instructor Phone
        [RegularExpression(@"^[\d\s\-\(\)\+\.]+$", ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }
    }
}
