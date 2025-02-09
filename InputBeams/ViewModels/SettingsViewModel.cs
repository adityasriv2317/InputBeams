using System;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InputBeams.Contracts.Services;
using InputBeams.Helpers;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;
using Windows.Storage;

namespace InputBeams.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;

    public event Action VibrationSettingChanged;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _versionDescription;

    [ObservableProperty] // ✅ This auto-generates 'IsVibrationEnabled'
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

        // ✅ Load the saved vibration setting
        isVibrationEnabled = LoadVibrationSetting();

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
            IsVibrationEnabled = !IsVibrationEnabled;
            SaveVibrationSetting(IsVibrationEnabled);

            System.Diagnostics.Debug.WriteLine($"🔔 Vibration setting changed: {IsVibrationEnabled}");

            VibrationSettingChanged?.Invoke(); // Notify HomePage
            GamepadManager.ApplyVibration(IsVibrationEnabled); // ✅ Apply instantly
        });


    }

    public void SaveVibrationSetting(bool isEnabled)
    {
        ApplicationData.Current.LocalSettings.Values["VibrationEnabled"] = isEnabled;
    }

    private bool LoadVibrationSetting()
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue("VibrationEnabled", out object value))
        {
            return (bool)value;
        }
        return true; // Default: enabled
    }

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
