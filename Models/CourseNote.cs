using SQLite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Models
{
    [Table("course_note")]
    public class CourseNote
    {
        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Note Content
        [MaxLength(1000)]
        [Column("note_content")]
        [NotNull]
        public string NoteContent { get; set; } = string.Empty;

        // Created Date
        [NotNull]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign key to Course
        [Column("course_id")]
        [NotNull]
        [Indexed]
        public int CourseId { get; set; }
    }
}
