using InputBeams.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.Gaming.Input;
using Windows.Gaming.UI;

namespace InputBeams.Views
{
    public sealed partial class HomePage : Page
    {
        public HomeViewModel ViewModel
        {
            get;
        }

        private readonly SettingsViewModel _settingsViewModel;
        private List<Gamepad> connectedGamepads = new();

        public HomePage()
        {
            ViewModel = App.GetService<HomeViewModel>();
            _settingsViewModel = App.GetService<SettingsViewModel>();

            InitializeComponent();

            // Initialize UI
            DeviceStatusText.Text = "Searching for devices...";

            // Subscribe to gamepad events
            Gamepad.GamepadAdded += OnGamepadAdded;
            Gamepad.GamepadRemoved += OnGamepadRemoved;

            // Subscribe to vibration setting changes
            _settingsViewModel.VibrationSettingChanged += OnVibrationSettingChanged;

            System.Diagnostics.Debug.WriteLine("✅ HomePage subscribed to VibrationSettingChanged");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            System.Diagnostics.Debug.WriteLine("🔄 HomePage loaded, checking devices...");

            // Refresh gamepad list
            RefreshDevices(null, null);

            // Re-subscribe to prevent duplicate subscriptions
            _settingsViewModel.VibrationSettingChanged -= OnVibrationSettingChanged;
            _settingsViewModel.VibrationSettingChanged += OnVibrationSettingChanged;
        }

        // 🔥 Triggered when vibration setting changes
        private void OnVibrationSettingChanged()
        {
            System.Diagnostics.Debug.WriteLine("🔔 Vibration setting changed, refreshing devices...");
            ApplyVibrationSetting();
        }

        // ✅ Refresh connected devices
        private void RefreshDevices(object sender, RoutedEventArgs e)
        {
            connectedGamepads = new List<Gamepad>(Gamepad.Gamepads);

            _ = DispatcherQueue.TryEnqueue(() =>
            {
                if (connectedGamepads.Count > 0)
                {
                    var gamepad = connectedGamepads[0]; // First detected gamepad

                    // 🆕 Displaying the name & type of the controller
                    string deviceName = GetControllerName(gamepad);
                    string inputMode = GetInputMode(gamepad);

                    DeviceNameText.Text = deviceName;
                    DeviceStatusText.Text = $"Status: Connected ({inputMode})";

                    DeviceImage.Visibility = Visibility.Visible;
                    DeviceGlyph.Visibility = Visibility.Collapsed;
                    InputModeDropdown.Visibility = Visibility.Visible;
                    RefreshButton.Visibility = Visibility.Collapsed;

                    ApplyVibrationSetting(); // ✅ Apply vibration when a device is detected
                }
                else
                {
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

        // ✅ Detects common controller types (Xbox, PlayStation, etc.)
        private string GetControllerName(Gamepad gamepad)
        {
            var gamepads = RawGameController.RawGameControllers;
            foreach (var rawGamepad in gamepads)
            {
                if (rawGamepad.SimpleHapticsControllers.Count > 0)
                {
                    if (rawGamepad.DisplayName.Contains("Xbox", StringComparison.OrdinalIgnoreCase))
                        DeviceTypeText.Text = "Xbox Controller";
                    else if (rawGamepad.DisplayName.Contains("DualShock", StringComparison.OrdinalIgnoreCase) ||
                        rawGamepad.DisplayName.Contains("PS", StringComparison.OrdinalIgnoreCase))
                        DeviceTypeText.Text = "PlayStation Controller";
                    else
                        DeviceTypeText.Text = "Generic Gamepad";
                }
            }
            return RawGameController.RawGameControllers[0].DisplayName;
        }

        // ✅ Determines whether the controller is using XInput or DInput
        private string GetInputMode(Gamepad gamepad)
        {
            return gamepad switch
            {
                _ when Gamepad.Gamepads.Contains(gamepad) => "X-Input",
                _ => "D-Input"
            };
        }


        // ✅ Apply vibration settings
        private void ApplyVibrationSetting()
        {
            if (connectedGamepads.Count > 0)
            {
                var gamepad = connectedGamepads[0]; // Use first connected gamepad
                bool isVibrationEnabled = _settingsViewModel.IsVibrationEnabled;

                System.Diagnostics.Debug.WriteLine($"🎮 Applying Vibration: {isVibrationEnabled}");

                gamepad.Vibration = isVibrationEnabled
                    ? new GamepadVibration { LeftMotor = 0.5, RightMotor = 0.5 }
                    : new GamepadVibration();
            }
        }

        // Event: Gamepad connected
        private void OnGamepadAdded(object sender, Gamepad e)
        {
            RefreshDevices(null, null);
            ApplyVibrationSetting();
        }

        // Event: Gamepad disconnected
        private void OnGamepadRemoved(object sender, Gamepad e)
        {
            RefreshDevices(null, null);
            ApplyVibrationSetting();
        }

        // Toggle InputBeams state
        private void OnToggleInputBeams(object sender, RoutedEventArgs e)
        {
            StatusText.Text = EnableDisableToggle.IsOn ? "InputBeams is Enabled" : "InputBeams is Disabled";
        }

        // Open Windows Device Settings
        private async void OpenDeviceSettings(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:devices"));
        }

        // Navigate to Settings Page
        private void OpenInputBeamsSettings(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        // Navigate to Documentation Page
        private void OpenDocumentation(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DocumentationPage));
        }

        // Navigate to Configuration Page
        private void OpenConfiguration(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ConfigurationPage));
        }

        // Update connected device input mode
        private void UpdateInputModeDropdown(Gamepad gamepad)
        {
            InputModeDropdown.Visibility = gamepad != null ? Visibility.Visible : Visibility.Collapsed;
            SelectedInputModeText.Text = gamepad != null ? "Select Input Mode" : "";
        }

        // Set input mode
        private void SetInputMode(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem selectedItem)
            {
                SelectedInputModeText.Text = $"Mode: {selectedItem.Tag}";
            }
        }
    }
}
