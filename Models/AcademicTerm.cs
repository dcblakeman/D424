using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Models
{
    [Table("academic_term")]
    public partial class AcademicTerm
    {
        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Name
        [MaxLength(100)]
        [NotNull]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        // Start and End Dates
        [NotNull]
        [Column("start_date")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [NotNull]
        [Column("end_date")]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [NotNull]
        [Column("is_active")]
        public bool IsActive { get; set; }

        [NotNull]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties - ignored by SQLite
        [Ignore]
        public List<Course> Courses { get; set; } = new();
    }
}
