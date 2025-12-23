using SQLite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Models
{
    [Table("CourseNote")]
    public class CourseNote
    {
        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Note Content
        [NotNull]
        public string NoteContent { get; set; } = string.Empty;

        // Created Date
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign key to Course
        [Indexed]
        public int CourseId { get; set; }
    }
}
