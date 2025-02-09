using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InputBeams.Contracts.Services;
using InputBeams.Helpers;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;
using Windows.Storage;
using Microsoft.Win32;

namespace InputBeams.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private const string VibrationSettingKey = "VibrationEnabled";
    private const string StartupSettingKey = "RunOnStartup";

    public event Action? VibrationSettingChanged;
    private const string StartupKey = "InputBeams"; // ✅ Unique key for Run on Startup
    private const string RunOnStartupKey = "RunOnStartupEnabled";

    [ObservableProperty]
    private bool isRunOnStartupEnabled;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _versionDescription;

    [ObservableProperty]
    private bool isVibrationEnabled;

    public ICommand SwitchThemeCommand
    {
        get;
    }
    public ICommand ToggleVibrationCommand
    {
        get;
    }

    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();

        // ✅ Fire-and-forget to load settings asynchronously
        _ = LoadSettingsAsync();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(async (param) =>
        {
            if (ElementTheme != param)
            {
                ElementTheme = param;
                await _themeSelectorService.SetThemeAsync(param);
            }
        });

        ToggleVibrationCommand = new RelayCommand(() =>
        {
            IsVibrationEnabled = !IsVibrationEnabled; // ✅ Auto-Saves via OnIsVibrationEnabledChanged
        });

        // ✅ Run on Startup
        Task.Run(async () => await LoadSettingsAsync());
        isRunOnStartupEnabled = ApplicationData.Current.LocalSettings.LoadSetting(RunOnStartupKey, false);
    }

    /// <summary>
    /// ✅ Loads stored settings asynchronously
    /// </summary>
    private async Task LoadSettingsAsync()
    {
        // 🛠️ Ensure settings are correctly casted
        IsVibrationEnabled = ApplicationData.Current.LocalSettings.LoadSetting(VibrationSettingKey, true);
        IsRunOnStartupEnabled = ApplicationData.Current.LocalSettings.LoadSetting(StartupSettingKey, false);

        // isrunonstartupenabled
        IsRunOnStartupEnabled = IsAppInStartup();
    }

    /// <summary>
    /// ✅ Saves Vibration setting when toggled
    /// </summary>
    partial void OnIsVibrationEnabledChanged(bool value)
    {
        Task.Run(() =>
        {
            ApplicationData.Current.LocalSettings.SaveSetting(VibrationSettingKey, value);
            System.Diagnostics.Debug.WriteLine($"🔔 Vibration setting changed: {value}");
            GamepadManager.ApplyVibration(value);
        });

        VibrationSettingChanged?.Invoke();
    }

    /// <summary>
    /// ✅ Saves Run on Startup setting when toggled
    /// </summary>
    partial void OnIsRunOnStartupEnabledChanged(bool value)
    {
        // Save to local settings
        ApplicationData.Current.LocalSettings.SaveSetting(RunOnStartupKey, value);

        // Apply startup setting
        SetStartup(value);
        System.Diagnostics.Debug.WriteLine($"🖥️ Run on Startup changed: {value}");
    }

    private bool IsAppInStartup()
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
        {
            return key?.GetValue(StartupKey) != null;
        }
    }

    private void SetStartup(bool enable)
    {
        string appPath = Path.Combine(AppContext.BaseDirectory, "InputBeams.exe");

        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
        {
            if (enable)
            {
                key.SetValue(StartupKey, $"\"{appPath}\"");
            }
            else
            {
                key.DeleteValue(StartupKey, false);
            }
        }
    }


    /// <summary>
    /// ✅ Gets the app version description
    /// </summary>
    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;
            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
