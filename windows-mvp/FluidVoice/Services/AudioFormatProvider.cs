namespace FluidVoice.Services;

public sealed class AudioFormatProvider
{
    public int SampleRate { get; } = 16000;
    public int BitsPerSample { get; } = 16;
    public int Channels { get; } = 1;
}
