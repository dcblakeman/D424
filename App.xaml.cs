using C_971.Services;


namespace C_971
{
    public partial class App : Application
    {
        private readonly AppShell _appShell;
        private readonly PermissionService _permissionService;

        public App(AppShell appShell, PermissionService permissionService)
        {
            InitializeComponent();
            _appShell = appShell;
            _permissionService = permissionService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_appShell);
        }

        protected override async void OnStart()
        {
            base.OnStart();
            _ = await _permissionService.RequestNotificationPermissionAsync();
        }
    }
}
