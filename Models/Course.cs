using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_971.Models
{
    public partial class Course : BaseEntity
    {
        public enum Status { InProgress, Completed, Dropped, Planned }

        [ObservableProperty]
        private int termId;

        [ObservableProperty]
        private DateTime startDate = DateTime.Now;

        [ObservableProperty]
        private DateTime endDate = DateTime.Now;

        [ObservableProperty]
        private Status courseStatus = Status.InProgress;

        [ObservableProperty]
        private bool startDateNotifications = true;

        [ObservableProperty]
        private bool endDateNotifications = true;

        [ObservableProperty]
        private CourseInstructor? instructor;

        [ObservableProperty]
        private ObservableCollection<CourseNote> notes = new ObservableCollection<CourseNote>();

        [ObservableProperty]
        private List<CourseAssessment> assessments = new List<CourseAssessment>();
    }
}
