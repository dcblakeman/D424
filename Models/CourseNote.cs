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
        public static int? Count { get; internal set; }

        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Note Title
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        // Note Content
        [MaxLength(2000)]
        public string NoteContent { get; set; } = string.Empty;

        // Created Date
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Modified Date
        public DateTime? ModifiedDate { get; set; }


        // Foreign key to Course
        public int CourseId { get; set; }
    }
}
