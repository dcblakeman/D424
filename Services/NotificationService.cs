using C_971.Models;
using Plugin.LocalNotification;

namespace C_971.Services
{
    public class NotificationService
    {
        private readonly PermissionService _permission;

        public NotificationService(PermissionService permission)
        {
            _permission = permission;
        }
        public async Task<bool> ScheduleCourseStartReminderAsync(Course course, DateTime reminderDate)
        {

            await Shell.Current.DisplayAlertAsync("Testing", $"Scheduling notification for {reminderDate:yyyy-MM-dd HH:mm:ss}", "OK");

            await Shell.Current.DisplayAlertAsync("Testing", $"Current time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", "OK");

            if (!await EnsurePermissionAsync())
            {
                return false;
            }

            NotificationRequest request = new()
            {
                NotificationId = 1000 + course.Id,
                Title = "Course Starting Tomorrow",
                Subtitle = course.Name,
                Description = $"Course '{course.Name}' starts on {course.StartDate:MM/dd/yyyy}",
                Schedule = { NotifyTime = reminderDate }
            };

            _ = await LocalNotificationCenter.Current.Show(request);
            return true;
        }

        public async Task<bool> ScheduleCourseEndReminderAsync(Course course, DateTime reminderDate)
        {
            if (!await EnsurePermissionAsync())
            {
                return false;
            }

            NotificationRequest request = new()
            {
                NotificationId = 2000 + course.Id,
                Title = "Course Ending Soon",
                Subtitle = course.Name,
                Description = $"Course '{course.Name}' ends on {course.EndDate:MM/dd/yyyy}",
                Schedule = { NotifyTime = reminderDate }
            };

            _ = await LocalNotificationCenter.Current.Show(request);
            return true;
        }

        public async Task CancelNotificationAsync(int notificationId)
        {
            _ = LocalNotificationCenter.Current.Cancel(notificationId);
        }

        private async Task<bool> EnsurePermissionAsync()
        {
            return await _permission.HasNotificationPermissionAsync() ||
                   await _permission.RequestNotificationPermissionAsync();
        }

        internal async Task<bool> ScheduleAssessmentReminderAsync(CourseAssessment assessment, DateTime reminderDate)
        {
            if (!await EnsurePermissionAsync())
            {
                return false;
            }

            NotificationRequest request = new()
            {
                NotificationId = assessment.Id,
                Title = "Assessment Due Soon",
                Subtitle = assessment.Name,
                Description = $"Your {assessment.Type} assessment '{assessment.Name}' is due on {assessment.EndDate:MM/dd/yyyy}",
                Schedule =
                {
                    NotifyTime = reminderDate
                },
                Android =
                {
                    ChannelId = "assessment_reminders",
                    Priority = Plugin.LocalNotification.AndroidOption.AndroidPriority.High,
                    AutoCancel = true
                }
            };

            _ = await LocalNotificationCenter.Current.Show(request);
            return true;
        }

        public async Task<bool> ScheduleAssessmentStartReminderAsync(CourseAssessment assessment, DateTime reminderDate)
        {
            if (!await EnsurePermissionAsync())
            {
                return false;
            }

            NotificationRequest request = new()
            {
                NotificationId = assessment.Id,
                Title = "Assessment Starting Soon",
                Subtitle = assessment.Name,
                Description = $"{assessment.Type} '{assessment.Name}' starts on {assessment.StartDate:MM/dd/yyyy}",
                Schedule = { NotifyTime = reminderDate }
            };

            _ = await LocalNotificationCenter.Current.Show(request);
            return true;
        }

        public async Task<bool> ScheduleAssessmentDueReminderAsync(CourseAssessment assessment, DateTime reminderDate)
        {
            if (!await EnsurePermissionAsync())
            {
                return false;
            }

            NotificationRequest request = new()
            {
                NotificationId = 10000 + assessment.Id, // Different offset for due date reminders
                Title = "Assessment Due Soon",
                Subtitle = assessment.Name,
                Description = $"{assessment.Type} '{assessment.Name}' is due on {assessment.EndDate:MM/dd/yyyy}",
                Schedule = { NotifyTime = reminderDate }
            };

            _ = await LocalNotificationCenter.Current.Show(request);
            return true;
        }

        public async Task<bool> ScheduleCourseDueReminderAsync(Course newCourse, DateTime reminderDate)
        {
            if (!await EnsurePermissionAsync())
            {
                return false;
            }

            NotificationRequest request = new()
            {
                NotificationId = 3000 + newCourse.Id,
                Title = "Course Due Soon",
                Subtitle = newCourse.Name,
                Description = $"Course '{newCourse.Name}' is due on {newCourse.EndDate:MM/dd/yyyy}",
                Schedule = { NotifyTime = reminderDate }
            };
            _ = await LocalNotificationCenter.Current.Show(request);
            return true;
        }
    }
}
