using System;
using System.Diagnostics;
using Microsoft.Win32;
using Dimmr.Models;

namespace Dimmr.Services;

/// <summary>Registers or clears the HKCU Run entry so Dimmr can launch at login.</summary>
public static class StartupManager
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

    public static bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: false);
        return key?.GetValue(AppConstants.AppName) is string;
    }

    public static void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true)
                        ?? Registry.CurrentUser.CreateSubKey(RunKey);
        if (key is null)
            return;

        if (enabled)
        {
            var exe = Process.GetCurrentProcess().MainModule?.FileName;
            if (!string.IsNullOrEmpty(exe))
                key.SetValue(AppConstants.AppName, $"\"{exe}\" --startup");
        }
        else
        {
            key.DeleteValue(AppConstants.AppName, throwOnMissingValue: false);
        }
    }
}
