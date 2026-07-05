namespace FluidVoice.Services;

public sealed class AudioCaptureService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }
}
