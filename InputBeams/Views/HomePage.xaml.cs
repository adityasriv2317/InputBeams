using InputBeams.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.System;
using Windows.Gaming.Input;

namespace InputBeams.Views;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel
    {
        get;
    }

    public HomePage()
    {
        ViewModel = App.GetService<HomeViewModel>();
        InitializeComponent();

        // Event: fetch devices
        DeviceStatusText.Text = "Searching for devices...";
        Gamepad.GamepadAdded += OnGamepadAdded;
        Gamepad.GamepadRemoved += OnGamepadRemoved;
    }

    // Event: ToggleSwitch for Enabling/Disabling InputBeams
    private void OnToggleInputBeams(object sender, RoutedEventArgs e)
    {
        if (EnableDisableToggle.IsOn)
        {
            StatusText.Text = "InputBeams is Enabled";
        }
        else
        {
            StatusText.Text = "InputBeams is Disabled";
        }
    }

    // Event: Refresh Button for Checking Connected Devices
    private void RefreshDevices(object sender, RoutedEventArgs e)
    {
        var controllers = GetConnectedControllers();

        _ = DispatcherQueue.TryEnqueue(() =>
        {
            if (controllers.Count > 0)
            {
                Gamepad firstGamepad = controllers[0];
                RawGameController rawController = RawGameController.FromGameController(firstGamepad);

                string deviceName = rawController?.DisplayName ?? "Unknown Controller";
                string inputType = rawController?.AxisCount > 0 ? "Gamepad" : "Other Device";

                // Update UI safely on the main thread
                DeviceNameText.Text = deviceName;
                DeviceTypeText.Text = $"Type: {inputType}";
                DeviceStatusText.Text = "Status: Connected";

                DeviceImage.Visibility = Visibility.Visible;
                DeviceGlyph.Visibility = Visibility.Collapsed;
                InputModeDropdown.Visibility = Visibility.Visible;
                RefreshButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                // No device connected
                DeviceNameText.Text = "No Device Connected";
                DeviceTypeText.Text = "";
                DeviceStatusText.Text = "Status: Disconnected";

                DeviceImage.Visibility = Visibility.Collapsed;
                DeviceGlyph.Visibility = Visibility.Visible;
                InputModeDropdown.Visibility = Visibility.Collapsed;
                RefreshButton.Visibility = Visibility.Visible;
            }
        });
    }


    // Called when a new gamepad is connected
    private void OnGamepadAdded(object sender, Gamepad e)
    {
        RefreshDevices(null, null);
    }

    // Called when a gamepad is disconnected
    private void OnGamepadRemoved(object sender, Gamepad e)
    {
        RefreshDevices(null, null);
    }

    // Function to detect and show available controllers
    private List<Gamepad> GetConnectedControllers()
    {
        List<Gamepad> connectedGamepads = new List<Gamepad>(Gamepad.Gamepads);
        return connectedGamepads;
    }

    // Event: Open Windows Device Settings
    private async void OpenDeviceSettings(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("ms-settings:devices"));
    }

    // Event: Open InputBeams Settings
    private void OpenInputBeamsSettings(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(SettingsPage));
    }

    // Event: Open Documentation
    private void OpenDocumentation(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(DocumentationPage));
    }

    // Event: Open Configuration
    private void OpenConfiguration(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(ConfigurationPage));
    }

    // update connected device input mode
    private void UpdateInputModeDropdown(Gamepad gamepad)
    {
        // Check if the gamepad supports XInput (modern controllers)
        if (gamepad != null)
        {
            InputModeDropdown.Visibility = Visibility.Visible;
            SelectedInputModeText.Text = "Select Input Mode"; // Default text
        }
        else
        {
            InputModeDropdown.Visibility = Visibility.Collapsed;
        }
    }

    private void SetInputMode(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem selectedItem)
        {
            string mode = selectedItem.Tag.ToString();
            SelectedInputModeText.Text = $"Mode: {mode}"; // Update UI
            // Implement actual switching logic (if applicable)
        }
    }

}