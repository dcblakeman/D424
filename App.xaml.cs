using C_971.Services;
using C_971.Views;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;


namespace C_971
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();

            var permissionService = Handler?.MauiContext?.Services.GetService<PermissionService>();
            if (permissionService != null)
            {
                await permissionService.RequestNotificationPermissionAsync();
            }
        }
    }
}