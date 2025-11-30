using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Models
{
    [SQLite.Table("AcademicTerms")]
    public partial class AcademicTerm
    {
        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Name
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Start and End Dates
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
    }
}
