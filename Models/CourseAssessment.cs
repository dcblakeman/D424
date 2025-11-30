using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Models
{
    public partial class CourseAssessment : BaseEntity
    {
        public enum AssessmentType { Objective, Performance }

        [ObservableProperty]
        private AssessmentType type = AssessmentType.Objective;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private DateTime startDate = DateTime.Now;

        [ObservableProperty]
        private DateTime endDate = DateTime.Now;

        [ObservableProperty]
        private bool startDateNotifications = true;

        [ObservableProperty]
        private bool endDateNotifications = true;
    }
}
