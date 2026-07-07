using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FluidVoice.Services;

public sealed class JsonSettingsService : ISettingsService
{
    private readonly string _filePath;
    private readonly object _lock = new();
    private Dictionary<string, string> _settings = new();

    public JsonSettingsService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dirPath = Path.Combine(appData, "FluidVoice");
        _filePath = Path.Combine(dirPath, "settings.json");

        LoadSettings();
    }

    private void LoadSettings()
    {
        lock (_lock)
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    var deserialized = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (deserialized != null)
                    {
                        _settings = deserialized;
                    }
                }
            }
            catch (Exception)
            {
                // Graceful fallback: default to empty settings dictionary
                _settings = new Dictionary<string, string>();
            }
        }
    }

    public string GetSetting(string key, string defaultValue = "")
    {
        lock (_lock)
        {
            return _settings.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }

    public void SetSetting(string key, string value)
    {
        lock (_lock)
        {
            _settings[key] = value;
            SaveSettings();
        }
    }

    private void SaveSettings()
    {
        try
        {
            var dirPath = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception)
        {
            // Graceful fallback: do not crash if settings save fails
        }
    }
}
