using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace C_971.ViewModels
{
    public partial class CourseInstructorViewModel : ObservableObject
    {
        [ObservableProperty]
        public string name = "Course Instructor";

        private DatabaseService _databaseService;

    }


}
