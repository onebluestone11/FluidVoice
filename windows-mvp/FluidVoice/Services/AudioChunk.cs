using System;

namespace FluidVoice.Services;

public sealed record AudioChunk(
    byte[] PcmBytes,
    int SampleRate,
    int BitsPerSample,
    int Channels,
    DateTimeOffset CapturedAt);
