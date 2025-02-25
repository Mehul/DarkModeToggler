using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Principal;
using System.Drawing.Drawing2D;
using System.IO;

namespace DarkModeToggler
{
    public class DarkModeApp : ApplicationContext
    {
        // Constants for the Windows Registry
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryValueName = "AppsUseLightTheme";
        private const string SystemRegistryValueName = "SystemUsesLightTheme";

        private NotifyIcon trayIcon;
        private bool isLightMode;
        private bool isElevated;

        // Store icons to prevent garbage collection
        private Icon lightModeIcon;
        private Icon darkModeIcon;

        public DarkModeApp()
        {
            // Create custom icons
            lightModeIcon = IconHelper.CreateToggleIcon(true);
            darkModeIcon = IconHelper.CreateToggleIcon(false);

            // Check if running with admin privileges
            isElevated = IsAdministrator();

            // Initialize the system tray icon
            trayIcon = new NotifyIcon()
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Visible = true,
                Text = "Dark Mode Toggler" + (isElevated ? "" : " (Limited Mode)")
            };

            // Check current theme
            isLightMode = GetCurrentTheme();
            UpdateIcon();

            // Set up the context menu
            trayIcon.ContextMenuStrip.Items.Add("Toggle Theme", null, ToggleTheme);
            trayIcon.ContextMenuStrip.Items.Add("Toggle App Theme Only", null, ToggleAppThemeOnly);
            trayIcon.ContextMenuStrip.Items.Add("Toggle System Theme Only", null, ToggleSystemThemeOnly);
            trayIcon.ContextMenuStrip.Items.Add("-"); // Separator

            if (!isElevated)
            {
                trayIcon.ContextMenuStrip.Items.Add("Restart as Administrator", null, RestartAsAdmin);
                trayIcon.ContextMenuStrip.Items.Add("-"); // Separator
            }

            trayIcon.ContextMenuStrip.Items.Add("Exit", null, Exit);

            // Enable single-click toggle for both themes
            trayIcon.MouseClick += (s, e) => {
                if (e.Button == MouseButtons.Left)
                    ToggleTheme(s, e);
            };
        }

        private bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private void RestartAsAdmin(object sender, EventArgs e)
        {
            // Get the path of the current executable
            string exePath = Process.GetCurrentProcess().MainModule.FileName;

            // Prepare process start info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = exePath,
                Verb = "runas" // This requests elevation
            };

            try
            {
                Process.Start(startInfo);
                Application.Exit(); // Close current instance
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to restart with elevated privileges: {ex.Message}",
                                "Elevation Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private bool GetCurrentTheme()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(RegistryValueName);
                        return value != null && (int)value == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading theme setting: {ex.Message}", "Theme Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return true; // Default to light mode if registry value not found
        }

        private void SetTheme(bool lightMode, bool appsTheme = true, bool systemTheme = true)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true))
                {
                    if (key != null)
                    {
                        if (appsTheme)
                            key.SetValue(RegistryValueName, lightMode ? 1 : 0, RegistryValueKind.DWord);

                        if (systemTheme)
                            key.SetValue(SystemRegistryValueName, lightMode ? 1 : 0, RegistryValueKind.DWord);

                        isLightMode = lightMode;
                        UpdateIcon();

                        // Use a more robust theme change notification method
                        NotifyThemeChange();
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (!isElevated)
                {
                    var result = MessageBox.Show(
                        "This operation requires administrator privileges. Would you like to restart the application as administrator?",
                        "Elevation Required",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        RestartAsAdmin(null, null);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Failed to change theme settings due to insufficient permissions, even with administrator privileges.",
                        "Permission Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing theme: {ex.Message}", "Theme Change Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NotifyThemeChange()
        {
            try
            {
                // Method 1: Send WM_SETTINGCHANGE message
                SendSettingChangeMessage();

                // Method 2: Alternative approach using SHChangeNotify (shell notification)
                SHChangeNotify(0x08000000, 0, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error notifying system of theme change: {ex.Message}",
                               "Theme Notification Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Warning);
            }
        }

        [DllImport("shell32.dll")]
        private static extern void SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        private void SendSettingChangeMessage()
        {
            // This sends a message to Windows to refresh the theme
            const int WM_SETTINGCHANGE = 0x001A;

            IntPtr HWND_BROADCAST = (IntPtr)0xffff;

            NativeMethods.SendMessageTimeout(
                HWND_BROADCAST,
                WM_SETTINGCHANGE,
                IntPtr.Zero,
                "ImmersiveColorSet",
                NativeMethods.SendMessageTimeoutFlags.SMTO_NORMAL,
                1000,
                out IntPtr _);
        }

        private void UpdateIcon()
        {
            // Use custom icons for light and dark mode
            trayIcon.Icon = isLightMode ? lightModeIcon : darkModeIcon;

            trayIcon.Text = $"Dark Mode Toggler ({(isLightMode ? "Light Mode" : "Dark Mode")})" +
                            (isElevated ? " (Admin)" : "");
        }

        private void ToggleTheme(object sender, EventArgs e)
        {
            SetTheme(!isLightMode, true, true);
        }

        private void ToggleAppThemeOnly(object sender, EventArgs e)
        {
            SetTheme(!isLightMode, true, false);
        }

        private void ToggleSystemThemeOnly(object sender, EventArgs e)
        {
            SetTheme(!isLightMode, false, true);
        }

        private void Exit(object sender, EventArgs e)
        {
            // Clean up resources
            lightModeIcon?.Dispose();
            darkModeIcon?.Dispose();
            trayIcon.Visible = false;
            Application.Exit();
        }
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SendMessageTimeout(
            IntPtr hWnd,
            int Msg,
            IntPtr wParam,
            [MarshalAs(UnmanagedType.LPWStr)] string lParam,
            SendMessageTimeoutFlags fuFlags,
            uint uTimeout,
            out IntPtr lpdwResult);

        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }
    }
}