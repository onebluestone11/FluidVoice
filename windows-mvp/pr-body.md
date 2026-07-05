## Description
Advance Windows MVP service boundaries. This PR introduces clear interfaces for audio capture and transcription to prepare for real implementations, improves lifecycle stability, adds an in-memory local settings service, and introduces developer documentation.

## Type of Change
- [ ] Bug fix
- [x] New feature (MVP boundary advancement)
- [x] Refactoring
- [x] Documentation update

## Scope
Changes are strictly isolated to the `/windows-mvp/` directory. No protected macOS files or shared generic sources were modified.

## Testing
- Build validated successfully using `dotnet build .\windows-mvp\FluidVoice.sln`.
- Verified that existing mock dictation functionality dependencies are properly injected.

## Screenshots / Video
No UI changes were introduced. Screenshot not captured.

## Boundaries
- No secrets, credentials, or API keys were added or requested.
- Preserved existing architecture and repo boundaries.

## Risks / Notes
- The audio capture and transcription services remain mocks but are now hidden behind `IAudioCaptureService` and `ITranscriptionService`.
- Added a simple `LocalSettingsService` to serve as a configuration boundary before introducing real storage.

## Follow-up
- Implement real audio capture in `AudioCaptureService` (e.g. WASAPI/NAudio).
- Implement real transcription mapping in a new `TranscriptionService` connected to an external provider or local model.
- Replace `LocalSettingsService` with AppData/Registry or config-file backed storage.
