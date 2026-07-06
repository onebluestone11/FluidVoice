using System.Collections.Generic;
using System.Threading;

namespace FluidVoice.Services;

public interface IAudioCaptureService
{
    IAsyncEnumerable<AudioChunk> CaptureAsync(CancellationToken cancellationToken);
}
