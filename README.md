# InputBeams

**InputBeams** is a lightweight Windows application that allows users to control their computer using external game controllers like Xbox, PlayStation (DualShock/DualSense), and other XInput or DInput-compatible devices. It enables gamepads to simulate mouse and keyboard input, providing a fully customizable experience.

## 🎮 Features

- ✅ Detects and connects Xbox, DualShock, and other standard gamepads
- 🖱️ Control mouse pointer using analog sticks
- ⌨️ Map gamepad buttons to keyboard keys
- ⚙️ Invert axis, adjust sensitivity, enable pointer acceleration
- 🔄 Save and load custom configurations
- 🚀 Launch on startup (optional)
- 🧩 Integrates with Game Bar widgets (optional toggle)
- 📊 Real-time preview of controller inputs
- 🧠 Easy-to-use GUI built with WinUI 3, designed with Windows 11 aesthetics

## 🧭 Navigation

- **Home Page**  
  - Enable/disable app  
  - View connected controller info (type, status, input mode)  
  - Adjust mouse and keyboard input options  
  - Access quick links to Settings and Configurations

- **Configuration Page**  
  - Fully customize input mappings (buttons, analog sticks, D-pad)  
  - Edit shortcuts and widget visibility  
  - Save, load, reset or auto-configure profiles  
  - Advanced tweak options (pointer acceleration, gyroscope, axis inversion)

- **Settings Page**  
  - Toggle "Run on Startup"  
  - Manage persistent user preferences  
  - Future support for language and theme settings

## 🧰 Tech Stack

- **Language**: C#
- **Framework**: WinUI 3 (Windows App SDK)
- **Input API**: Windows.Gaming.Input & XInput
- **Storage**: LocalSettings for persistent preferences

## 🚀 Getting Started

1. Clone the repository  
2. Open the solution in Visual Studio 2022 or later  
3. Build and run the application  
4. Connect a supported controller and start mapping!

## 💡 Planned Features

- Custom profile switching based on app/game focus  
- Gamepad vibration feedback  
- On-screen overlay for live key mapping  
- Cloud sync for configs

## 📄 License

This project is licensed under the MIT License.

---

**InputBeams** was built with a vision to give users more control and accessibility using external input devices. Whether you're a gamer, a power user, or someone with unique accessibility needs — InputBeams puts your controller to work beyond the game.

