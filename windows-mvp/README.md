# FluidVoice Windows MVP

This is the Windows MVP implementation for FluidVoice.

## Build Command
```powershell
dotnet build .\FluidVoice.sln
```

## Run Command
```powershell
dotnet run --project .\FluidVoice\FluidVoice.csproj
```

## Current Mock Behavior
Currently, the application uses mock services:
- **AudioCaptureService**: Starts and stops without actually capturing system audio.
- **MockTranscriptionService**: Streams a predefined set of phrases over time to simulate a real transcription feed.
- **LocalSettingsService**: An in-memory settings service for configuration.

## Next Real-Audio/Transcription Steps
1. Replace `AudioCaptureService` with a real Windows audio capture implementation (e.g., using WASAPI).
2. Replace `MockTranscriptionService` with a real service that takes audio streams and calls a transcription provider or runs a local model.
3. Replace `LocalSettingsService` with a persistent file-based or Windows Registry/AppConfig based storage.

## No-Secrets Rule
**IMPORTANT**: Do not commit secrets, API keys, or credentials to this repository. All real transcription API keys or sensitive configurations must be loaded from local environment variables or external secure storage that is not tracked by Git.
