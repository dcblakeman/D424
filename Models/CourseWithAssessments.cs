using System;
using System.Collections.Generic;
using System.Text;

namespace C_971.Models
{
    public class CourseWithAssessments
    {
        public Course Course { get; set; }
        public List<CourseAssessment> Assessments { get; set; }
    }
}
