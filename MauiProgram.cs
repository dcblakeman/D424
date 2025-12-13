using C_971.Models;
using C_971.Services;
using C_971.ViewModels;
using C_971.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
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
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register database service and interface
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<IDatabaseService>(provider => provider.GetRequiredService<DatabaseService>());

            builder.Services.AddTransient<AcademicTermListView>();
            builder.Services.AddTransient<AcademicTermListViewModel>();

            builder.Services.AddTransient<CourseListView>();
            builder.Services.AddTransient<CourseListViewModel>();

            builder.Services.AddTransient<CourseDetailsView>();
            builder.Services.AddTransient<CourseDetailsViewModel>();

            builder.Services.AddTransient<AddNoteView>();
            builder.Services.AddTransient<AddNoteViewModel>();

            builder.Services.AddTransient<ViewNotesView>();
            builder.Services.AddTransient<ViewNotesViewModel>();

            builder.Services.AddTransient<CourseInstructorView>();
            builder.Services.AddTransient<CourseInstructorViewModel>();

            builder.Services.AddTransient<AssessmentsView>();
            builder.Services.AddTransient < AssessmentsViewModel>();

            var app = builder.Build();

            return app;
        }
    }
}
