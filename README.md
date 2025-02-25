# DarkModeToggler

DarkModeToggler is a lightweight Windows tray utility designed to simplify toggling the system’s dark mode setting. Instead of navigating through multiple settings pages or relying on scheduled tasks, this tool provides a quick, one-click solution directly from the tray.

---

## Overview

Developed from a personal need for easy dark mode control, DarkModeToggler was prototyped in about an hour using Claude sonnet 3.7 on a free tier. This project not only addresses a common inconvenience but also demonstrates how AI-assisted development can rapidly deliver practical solutions.

---

## Features

- **Quick Toggle:** Switch between dark and light modes with a single click from the system tray.
- **Minimal Footprint:** A small utility that runs in the background without consuming excessive resources.
- **Simplicity:** Intuitive design that eliminates the need to dig through Windows settings.
- **Open Source:** Fully transparent codebase inviting community review and contributions.

---

## Installation

### Prerequisites

- **.NET platform:** Ensure the latest version of the .NET platform is installed.
- **Operating System:** Windows 10 or later.

### Build and Run Steps

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/DarkModeToggler.git
   cd DarkModeToggler
   ```

2. **Build the Application:**
   ```bash
   dotnet build
   ```

3. **Run the Application:**
   ```bash
   dotnet run
   ```
   On launch, the application minimizes to the system tray, ready for quick toggling.

---

## Usage

- **Launching:** Running the app sends it to the system tray.
- **Toggling Modes:** Click the tray icon to instantly switch between dark and light modes.
- **Context Menu:** Right-click the tray icon for additional options, such as exiting the application.

---

## Future Enhancements

- **Automated Scheduling:** Add functionality to schedule dark mode toggling automatically.
- **Keyboard Shortcuts:** Introduce global hotkeys for even quicker access.
- **Extended Customization:** Provide more control over dark mode behavior and integration with system themes.

*Note: These are ideas for future development and are open for community contributions.*

---

## Contributing

Contributions are welcome! To contribute:
1. Fork the repository.
2. Create a branch for your feature or fix.
3. Submit a pull request with a clear explanation of your changes.
4. Follow the existing coding style for consistency.

---

## License

This project is open source and distributed under the [Apache License 2.0](LICENSE).

### License Considerations

The Apache 2.0 License offers a permissive framework similar to the MIT License but includes explicit terms for patent rights, offering greater legal protection. It's a strong choice for open-source projects that encourage reuse, contributions, and commercial use while safeguarding contributors from potential patent claims.

---

## Acknowledgments

DarkModeToggler was born from a desire for a more efficient way to manage Windows appearance settings. This project highlights the potential of rapid prototyping with AI tools and thanks the community of developers and contributors who continue to make such innovations possible.
