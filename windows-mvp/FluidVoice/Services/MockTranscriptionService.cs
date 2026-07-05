using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FluidVoice.Services;

public sealed class MockTranscriptionService : ITranscriptionService
{
    private static readonly string[] Phrases =
    [
        "FluidVoice is listening. ",
        "This is simulated Windows dictation. ",
        "The MVP can show text arriving over time. ",
        "Real capture and transcription can be added behind these services later. "
    ];

    public async IAsyncEnumerable<string> StreamTranscriptAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (string phrase in Phrases)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(900), cancellationToken);
                yield return phrase;
            }
        }
    }
}
