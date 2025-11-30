using C_971.Views;

namespace C_971
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register your routes
            Routing.RegisterRoute("AcademicTermListView", typeof(AcademicTermListView));
            Routing.RegisterRoute("CourseListView", typeof(CourseListView));
            Routing.RegisterRoute("CourseDetailsView", typeof(CourseDetailsView));
            Routing.RegisterRoute("AddNoteView", typeof(AddNoteView));
            Routing.RegisterRoute("ViewNotesView", typeof(ViewNotesView));
        }
    }
}











