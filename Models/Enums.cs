using System;
using System.Collections.Generic;
using System.Text;

// Define enums for CourseStatus, AssessmentType, and AssessmentStatus
namespace C_971.Models
{
    public enum CourseStatus
    {
        NotEnrolled = 0,
        InProgress = 1,
        Completed = 2,
        Dropped = 3,
        Planned = 4
    }
    public enum AssessmentType 
    { 
        Objective = 0, 
        Performance = 1,
        Unknown = 2
    }
    public enum AssessmentStatus 
    { 
        Pending = 0, 
        InProgress = 1, 
        Completed = 2, 
        Overdue = 3 
    }

    public enum FinalGrade
    {
        A,
        AMinus,
        BPlus,
        B,
        BMinus,
        CPlus,
        C,
        CMinus,
        DPlus,
        D,
        DMinus,
        F,
        NotGraded
    }
}
