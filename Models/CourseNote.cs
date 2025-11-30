using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Models
{
    public class CourseNote
    {
        public int Id { get; set; }
        public string NoteContent { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int CourseId { get; set; }

        // Navigation property
        public Course? Course { get; set; }
    }
}
