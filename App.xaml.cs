using C_971.Services;


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

            PermissionService? permissionService = Handler?.MauiContext?.Services.GetService<PermissionService>();
            if (permissionService != null)
            {
                _ = await permissionService.RequestNotificationPermissionAsync();
            }
        }
    }
}