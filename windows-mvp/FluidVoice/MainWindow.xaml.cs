using System;
using System.Windows;
using FluidVoice.Services;
using FluidVoice.ViewModels;

namespace FluidVoice;

public partial class MainWindow : Window
{
    private readonly ISettingsService _settingsService;

    public MainWindow()
    {
        InitializeComponent();
        
        _settingsService = new JsonSettingsService();
        
        RestoreWindowSettings();

        DataContext = new MainViewModel(
            new AudioCaptureService(),
            new MockTranscriptionService(),
            _settingsService);
    }

    private void RestoreWindowSettings()
    {
        try
        {
            var widthStr = _settingsService.GetSetting("WindowWidth");
            var heightStr = _settingsService.GetSetting("WindowHeight");

            if (double.TryParse(widthStr, out double width) && width >= MinWidth)
            {
                Width = width;
            }

            if (double.TryParse(heightStr, out double height) && height >= MinHeight)
            {
                Height = height;
            }
        }
        catch (Exception)
        {
            // Graceful fallback to default design sizes
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        try
        {
            _settingsService.SetSetting("WindowWidth", Width.ToString());
            _settingsService.SetSetting("WindowHeight", Height.ToString());
        }
        catch (Exception)
        {
            // Graceful fallback
        }

        base.OnClosed(e);
    }
}
