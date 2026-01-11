namespace C_971.Services
{
    public class PermissionService
    {
        public async Task<bool> HasNotificationPermissionAsync()
        {
#if ANDROID
            if (DeviceInfo.Version.Major >= 13)
            {
                return await Permissions.CheckStatusAsync<Permissions.PostNotifications>() == PermissionStatus.Granted;
            }
#endif
            return true; // iOS and older Android
        }

        public async Task<bool> RequestNotificationPermissionAsync()
        {
#if ANDROID
            if (DeviceInfo.Version.Major >= 13)
            {
                return await Permissions.RequestAsync<Permissions.PostNotifications>() == PermissionStatus.Granted;
            }
#endif
            return true; // iOS and older Android
        }
    }
}
