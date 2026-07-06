using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FluidVoice.Services;

namespace FluidVoice.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly IAudioCaptureService _audioCaptureService;
    private readonly ITranscriptionService _transcriptionService;
    private readonly ISettingsService _settingsService;
    private CancellationTokenSource? _dictationCancellation;
    private bool _isDictating;
    private double _audioLevel;
    private string _transcriptText = "Press Start Dictation to begin mock transcription.";
    private string _statusText = "Ready";

    public MainViewModel(
        IAudioCaptureService audioCaptureService,
        ITranscriptionService transcriptionService,
        ISettingsService settingsService)
    {
        _audioCaptureService = audioCaptureService;
        _transcriptionService = transcriptionService;
        _settingsService = settingsService;
        StartDictationCommand = new RelayCommand(StartDictation, () => !IsDictating);
        StopDictationCommand = new RelayCommand(StopDictation, () => IsDictating);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand StartDictationCommand { get; }

    public ICommand StopDictationCommand { get; }

    public bool IsDictating
    {
        get => _isDictating;
        private set
        {
            if (SetProperty(ref _isDictating, value))
            {
                RaiseCommandStatesChanged();
            }
        }
    }

    public string TranscriptText
    {
        get => _transcriptText;
        private set => SetProperty(ref _transcriptText, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public double AudioLevel
    {
        get => _audioLevel;
        private set => SetProperty(ref _audioLevel, value);
    }

    private async void StartDictation()
    {
        if (IsDictating)
        {
            return;
        }

        _dictationCancellation = new CancellationTokenSource();
        IsDictating = true;
        StatusText = "Listening with mock transcription...";
        TranscriptText = string.Empty;

        try
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var chunk in _audioCaptureService.CaptureAsync(_dictationCancellation.Token))
                    {
                        float maxVal = 0f;
                        if (chunk.BitsPerSample == 32)
                        {
                            for (int i = 0; i < chunk.PcmBytes.Length; i += 4)
                            {
                                float sample = Math.Abs(BitConverter.ToSingle(chunk.PcmBytes, i));
                                if (sample > maxVal) maxVal = sample;
                            }
                        }
                        else if (chunk.BitsPerSample == 16)
                        {
                            for (int i = 0; i < chunk.PcmBytes.Length; i += 2)
                            {
                                float sample = Math.Abs((float)BitConverter.ToInt16(chunk.PcmBytes, i) / 32768f);
                                if (sample > maxVal) maxVal = sample;
                            }
                        }
                        
                        if (maxVal > 1.0f) maxVal = 1.0f;
                        AudioLevel = maxVal;
                    }
                }
                catch (OperationCanceledException) { }
            });

            await foreach (string phrase in _transcriptionService.StreamTranscriptAsync(_dictationCancellation.Token))
            {
                TranscriptText += phrase;
            }
        }
        catch (OperationCanceledException)
        {
            // Stop Dictation uses cancellation for the normal stop path.
        }
        finally
        {
            _dictationCancellation?.Dispose();
            _dictationCancellation = null;
            IsDictating = false;
            AudioLevel = 0.0;
            StatusText = TranscriptText.Length == 0 ? "Ready" : "Stopped";
        }
    }

    private void StopDictation()
    {
        if (!IsDictating)
        {
            return;
        }

        StatusText = "Stopping...";
        _dictationCancellation?.Cancel();
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    private static void RaiseCommandStatesChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}

internal sealed class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public RelayCommand(Action execute, Func<bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute();
    }

    public void Execute(object? parameter)
    {
        _execute();
    }
}
