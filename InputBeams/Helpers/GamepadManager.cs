using System.Collections.Generic;
using Windows.Gaming.Input;

namespace InputBeams.Helpers
{
    public static class GamepadManager
    {
        private static List<Gamepad> connectedGamepads = new();

        public static void RefreshGamepads()
        {
            connectedGamepads = new List<Gamepad>(Gamepad.Gamepads);
        }

        public static void ApplyVibration(bool isEnabled)
        {
            RefreshGamepads(); // Ensure we have the latest gamepad list

            if (connectedGamepads.Count > 0)
            {
                var gamepad = connectedGamepads[0];

                if (isEnabled)
                {
                    gamepad.Vibration = new GamepadVibration
                    {
                        LeftMotor = 0.5, // Medium intensity
                        RightMotor = 0.5
                    };
                }
                else
                {
                    gamepad.Vibration = new GamepadVibration(); // Disable vibration
                }
            }
        }
    }
}
