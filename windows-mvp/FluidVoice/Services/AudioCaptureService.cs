using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluidVoice.Services;

public sealed class AudioCaptureService : IAudioCaptureService
{
    private bool _isStarted;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (_isStarted)
        {
            throw new InvalidOperationException("Audio capture is already started.");
        }
        
        _isStarted = true;
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _isStarted = false;
        return Task.CompletedTask;
    }
}
