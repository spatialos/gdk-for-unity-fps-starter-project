# Changelog

## Unreleased

## `0.1.3` - 2018-11-26

### Added

- `MetricsSendSystem.cs` now sends metrics for Frames Per Second (FPS) and Unity heap usage.
- Added sound effects when shooting.

### Changed

- Improved the method of calculating load and FPS.
- Upgraded to GDK for Unity version `0.1.3`
- `MapBuilder.cs` performance and visual improvements, now generates worlds around the GameObject's position rather than at origin.

## `0.1.2` - 2018-11-02

### Changed

- Updated to GDK for Unity version `0.1.2`.

### Fixed

- Fixed an issue where newer versions of Unity would fail to open the project with errors like: `ParticleSystem` not found.

### Removed

- Removed `GroundChecker` and replaced with a cheaper ground checking functionality in the base `CharacterControllerMotor` class.
- Removed unused code and resources related to the "desert" in map generation.

## `0.1.1` - 2018-10-19

### Added

- Added external worker configurations for MacOS for all server workers in the project.

### Changed

- Updated to GDK for Unity version `0.1.1`.
- Improved Simulated Players; they fall off the navmesh less often.

### Fixed

- Fixed a bug that caused spawn points to be in the air, walls, or floor.
- Fixed a cross-worker spawning issue that resulted in players not being able to move and float in the air.

## `0.1.0` - 2018-10-10

The initial alpha release of the SpatialOS GDK for Unity FPS Starter Project.
