namespace FluidVoice.Services;

public interface ISettingsService
{
    string GetSetting(string key, string defaultValue = "");
    void SetSetting(string key, string value);
}
