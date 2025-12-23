using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static C_971.Models.CourseStatus;

namespace C_971.Models
{
    [Table("Course")]
    public partial class Course
    {
        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }


        // Name
        [MaxLength(100)]
        [NotNull]
        public string Name { get; set; } = string.Empty;


        // Description
        [MaxLength(250)]
        [NotNull]
        public string Description { get; set; } = string.Empty;


        // Start and End Dates
        [NotNull]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [NotNull]
        public DateTime EndDate { get; set; } = DateTime.Now;


        // Course Status
        [NotNull]
        public CourseStatus Status { get; set; } = NotEnrolled;


        // Notifications
        [NotNull]
        public bool StartDateNotifications { get; set; } = true;

        [NotNull]
        public bool EndDateNotifications { get; set; }  = true;

        // Foreign Keys
        [Indexed]
        [NotNull]
        public int TermId { get; set; }

        [NotNull]
        public int? InstructorId { get; set; }  
    }
}
