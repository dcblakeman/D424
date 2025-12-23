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
    [Table("AcademicTerm")]
    public partial class AcademicTerm
    {
        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Name
        [MaxLength(100)]
        [NotNull]
        public string Name { get; set; } = string.Empty;

        // Start and End Dates
        [NotNull]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [NotNull]
        public DateTime EndDate { get; set; } = DateTime.Now;
    }
}
