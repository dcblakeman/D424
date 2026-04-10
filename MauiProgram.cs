using C_971.Services;
using C_971.ViewModels;
using C_971.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using System.Runtime.Versioning;

namespace C_971
{
    public static class MauiProgram
    {
        [SupportedOSPlatform("android21.0")]
        [SupportedOSPlatform("windows10.0.17763.0")]
        [SupportedOSPlatform("ios13.0")]
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
#pragma warning disable CA1416 // Validate platform compatibility
            MauiAppBuilder mauiAppBuilder = builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#pragma warning restore CA1416 // Validate platform compatibility

            // Register database service and interface
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<NotificationService>();
            builder.Services.AddSingleton<PermissionService>();

            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<LoginViewModel>();

            builder.Services.AddTransient<AcademicTermListView>();
            builder.Services.AddTransient<AcademicTermListViewModel>();

            builder.Services.AddSingleton<CourseListView>();
            builder.Services.AddSingleton<CourseListViewModel>();

            builder.Services.AddSingleton<CourseDetailsView>();
            builder.Services.AddSingleton<CourseDetailsViewModel>();

            builder.Services.AddTransient<AddNoteView>();
            builder.Services.AddTransient<AddNoteViewModel>();

            builder.Services.AddTransient<ViewNotesView>();
            builder.Services.AddTransient<ViewNotesViewModel>();

            builder.Services.AddSingleton<CourseInstructorView>();
            builder.Services.AddSingleton<CourseInstructorViewModel>();

            builder.Services.AddSingleton<AssessmentSelectionView>();
            builder.Services.AddSingleton<AssessmentSelectionViewModel>();

            builder.Services.AddSingleton<PerformanceAssessmentView>();
            builder.Services.AddSingleton<PerformanceAssessmentViewModel>();

            builder.Services.AddSingleton<ObjectiveAssessmentView>();
            builder.Services.AddSingleton<ObjectiveAssessmentViewModel>();

            builder.Services.AddSingleton<ReportView>();
            builder.Services.AddSingleton<ReportViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            MauiApp app = builder.Build();

            return app;
        }
    }
}
