using System;
using System.Collections.Generic;
using System.Text;

// Define enums for CourseStatus, AssessmentType, and AssessmentStatus
namespace C_971.Models
{
    public enum CourseStatus { NotEnrolled, InProgress, Completed, Dropped, Planned }
    public enum AssessmentType { Objective, Performance }
    public enum AssessmentStatus { Pending, InProgress, Completed, Overdue }
}
