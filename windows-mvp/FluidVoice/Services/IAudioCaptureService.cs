using System.Threading;
using System.Threading.Tasks;

namespace FluidVoice.Services;

public interface IAudioCaptureService
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync();
}
