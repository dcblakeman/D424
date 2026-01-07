using Android.App;
using Android.Runtime;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using System.Collections.Generic; // Required for IList
[assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage, MaxSdkVersion = 32)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadMediaAudio)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadMediaImages)]
[assembly: UsesPermission(Android.Manifest.Permission.ReadMediaVideo)]

namespace C_971.Platforms.Android
{
    [Application]
    public class MainApplication(IntPtr handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override void OnCreate()
        {
            base.OnCreate();
            LocalNotificationCenter.CreateNotificationChannels(new List<NotificationChannelRequest>());
        }
    }
}
