using System.Windows;
using FluidVoice.Services;
using FluidVoice.ViewModels;

namespace FluidVoice;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(
            new AudioCaptureService(),
            new MockTranscriptionService(),
            new LocalSettingsService());
    }
}
