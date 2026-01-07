using C_971.Models;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Text;

namespace C_971.Services
{
    public class NotificationService
    {
        private readonly PermissionService _permission;

        public NotificationService(PermissionService permission)
        {
            _permission = permission;
        }
        public async Task<bool> ScheduleAssessmentReminderAsync(CourseAssessment assessment, DateTime reminderDate)
        {
            if (!await EnsurePermissionAsync()) return false;

            var request = new NotificationRequest
            {
                NotificationId = assessment.Id,
                Title = "Assessment Due Soon",
                Subtitle = assessment.Name,
                Description = $"{assessment.Type} '{assessment.Name}' is due {assessment.EndDate:MM/dd/yyyy}",
                Schedule = { NotifyTime = reminderDate }
            };

            await LocalNotificationCenter.Current.Show(request);
            return true;
        }


        public async Task<bool> ScheduleCourseReminderAsync(Course course, DateTime reminderDate)
        {
            if (!await EnsurePermissionAsync()) return false;

            var request = new NotificationRequest
            {
                NotificationId = 1000 + course.Id, // Offset to avoid conflicts
                Title = "Course Starting Soon",
                Subtitle = course.Name,
                Description = $"Course '{course.Name}' starts {course.StartDate:MM/dd/yyyy}",
                Schedule = { NotifyTime = reminderDate }
            };

            await LocalNotificationCenter.Current.Show(request);
            return true;
        }

        public async Task CancelNotificationAsync(int notificationId)
        {
            LocalNotificationCenter.Current.Cancel(notificationId);
        }

        private async Task<bool> EnsurePermissionAsync()
        {
            return await _permission.HasNotificationPermissionAsync() ||
                   await _permission.RequestNotificationPermissionAsync();
        }
    }
}
