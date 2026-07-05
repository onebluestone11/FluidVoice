using System.Collections.Generic;

namespace FluidVoice.Services;

public sealed class LocalSettingsService : ISettingsService
{
    // A simple in-memory settings service for the MVP
    private readonly Dictionary<string, string> _settings = new();

    public string GetSetting(string key, string defaultValue = "")
    {
        return _settings.TryGetValue(key, out string? value) ? value : defaultValue;
    }

    public void SetSetting(string key, string value)
    {
        _settings[key] = value;
    }
}
