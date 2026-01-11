namespace C_971.Models
{
    public class CourseWithAssessments
    {
        public required Course Course { get; set; }
        public required List<CourseAssessment> Assessments { get; set; }
    }
}
