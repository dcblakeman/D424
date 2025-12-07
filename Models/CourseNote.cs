using SQLite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Models
{
    [SQLite.Table("CourseNotes")]
    public class CourseNote
    {
        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Note Content
        public string NoteContent { get; set; } = string.Empty;

        // Created Date
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Modified Date
        public DateTime? ModifiedDate { get; set; }

        // Foreign key to Course
        [Indexed]
        public int CourseId { get; set; }
    }
}
