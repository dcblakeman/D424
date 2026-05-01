using C_971.Views;
using Microsoft.Extensions.DependencyInjection;

namespace C_971
{
    public partial class AppShell : Shell
    {
        public AppShell(IServiceProvider services)
        {
            InitializeComponent();
            LoginShellContent.ContentTemplate = new DataTemplate(() => services.GetRequiredService<LoginView>());

            // Register navigation routes for all non-root pages.
            Routing.RegisterRoute("AcademicTermListView", typeof(AcademicTermListView));
            Routing.RegisterRoute("AiAssistantView", typeof(AiAssistantView));
            Routing.RegisterRoute("CourseListView", typeof(CourseListView));
            Routing.RegisterRoute("CourseDetailsView", typeof(CourseDetailsView));
            Routing.RegisterRoute("AddNoteView", typeof(AddNoteView));
            Routing.RegisterRoute("ViewNotesView", typeof(ViewNotesView));
            Routing.RegisterRoute("CourseInstructorView", typeof(CourseInstructorView));
            Routing.RegisterRoute("AssessmentSelectionView", typeof(AssessmentSelectionView));
            Routing.RegisterRoute("PerformanceAssessmentView", typeof(PerformanceAssessmentView));
            Routing.RegisterRoute("ObjectiveAssessmentView", typeof(ObjectiveAssessmentView));
            Routing.RegisterRoute("ReportView", typeof(ReportView));
        }
    }
}
