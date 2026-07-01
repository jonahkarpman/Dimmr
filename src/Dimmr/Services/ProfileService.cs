using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dimmr.Models;

namespace Dimmr.Services;

/// <summary>Loads and saves settings and profiles as JSON under %APPDATA%\Dimmr.</summary>
public sealed class ProfileService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _root;
    private readonly string _profilesDir;
    private readonly string _settingsPath;

    public ProfileService()
    {
        _root = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AppConstants.AppName);
        _profilesDir = Path.Combine(_root, "profiles");
        _settingsPath = Path.Combine(_root, "settings.json");
        Directory.CreateDirectory(_profilesDir);
    }

    public AppSettings LoadSettings()
    {
        if (!File.Exists(_settingsPath))
            return new AppSettings();

        try
        {
            return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_settingsPath)) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public void SaveSettings(AppSettings settings)
        => File.WriteAllText(_settingsPath, JsonSerializer.Serialize(settings, JsonOptions));

    public List<string> ListProfileNames()
        => Directory.EnumerateFiles(_profilesDir, "*.json")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(n => !string.IsNullOrEmpty(n))
            .Select(n => n!)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

    public Profile LoadProfile(string name)
    {
        var path = ProfilePath(name);
        if (!File.Exists(path))
            return new Profile { Name = name };

        try
        {
            return JsonSerializer.Deserialize<Profile>(File.ReadAllText(path)) ?? new Profile { Name = name };
        }
        catch
        {
            return new Profile { Name = name };
        }
    }

    public void SaveProfile(Profile profile)
        => File.WriteAllText(ProfilePath(profile.Name), JsonSerializer.Serialize(profile, JsonOptions));

    private string ProfilePath(string name) => Path.Combine(_profilesDir, name + ".json");
}
