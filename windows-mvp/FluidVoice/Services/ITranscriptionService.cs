using System.Collections.Generic;
using System.Threading;

namespace FluidVoice.Services;

public interface ITranscriptionService
{
    IAsyncEnumerable<string> StreamTranscriptAsync(CancellationToken cancellationToken);
}
