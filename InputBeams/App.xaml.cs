using InputBeams.Activation;
using InputBeams.Contracts.Services;
using InputBeams.Core.Contracts.Services;
using InputBeams.Core.Services;
using InputBeams.Helpers;
using InputBeams.Models;
using InputBeams.Notifications;
using InputBeams.Services;
using InputBeams.ViewModels;
using InputBeams.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;

namespace InputBeams;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar { get; set; }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
            CreateDefaultBuilder().
            ConfigureLogging(logging =>
            {
                logging.ClearProviders(); // Removes default logging providers
            logging.AddDebug();       // ✅ Enable Debug logging
            logging.AddConsole();     // ✅ Optional: Add Console logging
            }).UseContentRoot(AppContext.BaseDirectory).
    ConfigureServices((context, services) =>
    {

        // Default Activation Handler
        services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers
            services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IPageService, PageService>();

            services.AddTransient<HomeViewModel>();
            services.AddTransient<ConfigurationViewModel>();
            services.AddTransient<DocumentationViewModel>();
            services.AddTransient<SettingsViewModel>();


            // Services
            services.AddSingleton<IAppNotificationService, AppNotificationService>();
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<DocumentationViewModel>();
            services.AddTransient<DocumentationPage>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<HomePage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<ConfigurationViewModel>();
            services.AddTransient<ConfigurationPage>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        App.GetService<IAppNotificationService>().Initialize();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        await App.GetService<IActivationService>().ActivateAsync(args);

        // ✅ Load saved vibration setting safely
        object? obj = ApplicationData.Current.LocalSettings.Values["VibrationEnabled"];
        bool isVibrationEnabled = obj switch
        {
            bool boolValue => boolValue, // ✅ If it's already a bool, use it
            string strValue => bool.TryParse(strValue, out var parsedBool) ? parsedBool : false, // ✅ Convert from string
            int intValue => intValue != 0, // ✅ Convert from integer (1 = true, 0 = false)
            _ => false // 🔴 Default to false if it can't be converted
        };

        System.Diagnostics.Debug.WriteLine($"🚀 App Launched: Applying Vibration = {isVibrationEnabled}");

        // Apply vibration globally
        GamepadManager.ApplyVibration(isVibrationEnabled);
    }

}
