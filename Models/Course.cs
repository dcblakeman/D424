using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static C_971.Models.CourseStatus;

namespace C_971.Models
{
    [SQLite.Table("Courses")]
    public partial class Course
    {
        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }


        // Name
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;


        // Description
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;


        // Start and End Dates
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;


        // Course Status
        public CourseStatus Status { get; set; } = NotEnrolled;


        // Notifications
        public bool StartDateNotifications { get; set; } = true;
        public bool EndDateNotifications { get; set; }  = true;


        // Foreign Keys
        public int TermId { get; set; }
        public int? InstructorId { get; set; }  
    }
}
