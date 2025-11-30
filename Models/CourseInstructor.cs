using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Models
{
    [SQLite.Table("CourseInstructors")]
    public class CourseInstructor
    {
        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }


        // Instructor Name
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Instructor Email
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        // Instructor Phone
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
    }
}
