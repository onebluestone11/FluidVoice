using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace FluidVoice.Services;

public sealed class AudioCaptureService : IAudioCaptureService
{
    public async IAsyncEnumerable<AudioChunk> CaptureAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var capture = new WasapiCapture();
        var channel = Channel.CreateUnbounded<AudioChunk>(new UnboundedChannelOptions
        {
            SingleWriter = true,
            SingleReader = true
        });

        void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            if (e.BytesRecorded > 0)
            {
                var pcmBytes = new byte[e.BytesRecorded];
                Array.Copy(e.Buffer, pcmBytes, e.BytesRecorded);

                var chunk = new AudioChunk(
                    pcmBytes,
                    capture.WaveFormat.SampleRate,
                    capture.WaveFormat.BitsPerSample,
                    capture.WaveFormat.Channels,
                    DateTimeOffset.UtcNow
                );

                channel.Writer.TryWrite(chunk);
            }
        }

        void OnRecordingStopped(object? sender, StoppedEventArgs e)
        {
            channel.Writer.TryComplete(e.Exception);
        }

        capture.DataAvailable += OnDataAvailable;
        capture.RecordingStopped += OnRecordingStopped;

        capture.StartRecording();

        try
        {
            await foreach (var chunk in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return chunk;
            }
        }
        finally
        {
            capture.StopRecording();
            capture.DataAvailable -= OnDataAvailable;
            capture.RecordingStopped -= OnRecordingStopped;
        }
    }
}
