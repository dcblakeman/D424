using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Models
{
    [Table("CourseAssessment")]
    public partial class CourseAssessment
    {
        // Primary Key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }


        // Name and Description
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;


        // Start, Due, and Completed Dates
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public DateTime? CompletedDate { get; set; } = null;


        // Assessment Type and Status
        public AssessmentType Type { get; set; } = AssessmentType.Objective;
        public AssessmentStatus Status { get; set; } = AssessmentStatus.Pending;


        // Notifications
        public bool StartDateNotifications { get; set; } = true;
        public bool EndDateNotifications { get; set; } = true;


        // Foreign key to Course
        public int CourseId { get; set; } 
    }
}
