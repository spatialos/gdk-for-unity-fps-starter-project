# Changelog

## Unreleased

### Added

- Added a Deployment Launcher window. This allows you to upload assemblies and launch deployments from the Unity Editor.
- Added two launch configurations: `cloud_launch_large_sim_players.json` and `cloud_launch_small_sim_players.json` for simulated player deployments. 

### Changed

- A simulated player now connects as a regular `UnityClient` worker rather than a `SimulatedPlayer` worker.
- In cloud deployments, `SimulatedPlayerCoordinator` workers are ran in a separate deployment to the `UnityGameLogic` workers.
    - Note that simulated players started by the coordinators still connect into the deployment with the `UnityGameLogic` workers.

### Removed

- Removed the `SimulatedPlayer` worker type.

## `0.1.4` - 2019-01-28

### Added

- Added mobile support for local deployments.

### Changed

- Updated all the launch configs to better match the world sizes and to use the new Runtime.
- `MapBuilder.cs` performance and visual improvements, now generates worlds around the GameObject's position rather than at origin.
- Upgraded the Worker SDK version to `13.5.1`. This is a stable Worker SDK release! :tada:
- Upgraded the project to be compatible with `2018.3.2f1`.
- Upgraded the postprocessing package to `2.1.2`.
- Upgraded the package manager package to `2.0.3`.
- Upgraded to GDK for Unity version `0.1.4`.

### Fixed

- Simulated Players can now rotate about the X axis, enabling them to shoot targets above or below the plane they are on.
- Fixed a bug where you could start each built-out worker only once on OSX.

### Removed

- Removed GameObjects containing `ObjectPooler` from all scenes, `ObjectPooler` instance is now created at runtime.
- Removed `HDRenderPipeline`.

## `0.1.3` - 2018-11-26

### Added

- `MetricsSendSystem.cs` now sends metrics for Frames Per Second (FPS) and Unity heap usage.
- Added sound effects when shooting.

### Changed

- Improved the method of calculating load and FPS.
- Upgraded to GDK for Unity version `0.1.3`

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
