# Clash of Critters Fishing Bot 🎣

A simple WPF-based fishing bot for **Clash of Critters** using image recognition and automated controls.

## Features

- 🎣 Automatic fishing detection
- 🔍 Pixel color recognition for finding fishing events
- 🖥️ Designed for BlueStacks emulator
- ⚡ Start/Stop control with keyboard shortcut support
- 🪟 WPF user interface

## Requirements

- Windows 10/11
- .NET Desktop Runtime
- BlueStacks 5

## BlueStacks Setup

The bot was developed and tested with the following settings:

- **DPI:** 240
- **Device:** Samsung Galaxy S20 Ultra
- **Window mode:** Enabled
- **Orientation:** Portrait

Make sure BlueStacks matches these settings for the best results.

## Usage

1. Open BlueStacks.
2. Navigate to the **Fishing Contest** screen in Clash of Critters.
3. Start the bot application.
4. Press **Start** or use **F6**.
5. The bot will automatically detect fishing actions and perform the required steps.

To stop the bot:
- Press **Stop**
- Or press **F6** again (depending on your configuration)

## How it works

The bot captures the screen and searches for specific pixel colors to detect game states.

The detection process:
1. Takes a screenshot of the emulator window.
2. Scans the image pixels.
3. Searches for matching colors within a tolerance range.
4. Executes the required action when a match is found.

## Project Information

- Language: C#
- Framework: WPF (.NET)
- Image processing: BitmapSource / Pixel scanning
- Platform: Windows

## Disclaimer

This project was created for educational purposes to experiment with:
- WPF development
- Screen capturing
- Image recognition
- Automation

Use at your own risk. Automated tools may violate the terms of service of some applications.

## License

This project is licensed under the MIT License.
