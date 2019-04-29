# Changelog

## Unreleased

### Added

- Added session-based gameplay, hidden behind the `Use Session Based Flow` feature flag.

### Changed

- Changed connection and player-spawn flow.
- Updated the available prefabs and textures.

## `0.2.1` - 2019-04-15

### Changed

- Updated the `ConnectionController` and `ClientWorkerConnector` to make full use of the updated Player Lifecycle Feature Module.
- Reactive Components are no longer used. They have been disabled by adding the `DISABLE_REACTIVE_COMPONENTS` define symbol.
- Updated to Unity version `2018.3.11`.
- Updated to use the GDK for Unity version `0.2.1`.

### Internal

- Fixed package dependencies.
- Fixed camera clipping.
- Removed unused camera.

## `0.2.0` - 2019-03-19

### Added

- Tweaked the Deployment Launcher window:
    - Added ability to choose deployment region (US, EU).
    - Added ability to force-upload an assembly.
    - Automatically open the SpatialOS Console page for a launched deployment.
- New way to control tile distribution and types in the world via TileTypeVolume component:
    - Create a collection of tile types with a TileTypeCollection asset (Assets > Create > Improbable > Tile Type Collection).
    - Place GameObjects with TileTypeVolume components into a prefab to control where to spawn those tiles in the world.
    - Specify the above TileTypeVolumes prefab to use inside MapBuilderSettings config object.
    - Specify a 'default' TileTypeCollection to use in the MapBuilderSettings to use if no volume found at a tile location.
    - Preexisting tile types: 'Default', 'Mountain', 'Residential', 'Structure', 'Tower', 'Wild'

### Changed

- Reduced `fps_simulated_players_creation_interval` from 60 to 5.
- Disabled the `Generate Map` button in the MapBuilder window if `MapBuilderSettings` is not set.
- The four middle tiles of the world are no longer forced to be of certain tile type.
- World tile prefabs improved.
	- Now make use of nested prefabs.
	- Completely new set of default tiles used to populate the world.
- Updated the build configuration asset.
- Reduced the amount of health that gets regenerated.
- Map generation is now asynchronous.

### Fixed

- Fixed a bug where the `SimulatedPlayerCoordinatorWorkerConnector` would throw errors, if you stopped the application while it was spawning more simulated players.

## `0.1.5` - 2019-02-21

### Added

- Added a Deployment Launcher window. This allows you to upload assemblies and launch deployments from the Unity Editor.
- Added two launch configurations: `cloud_launch_large_sim_players.json` and `cloud_launch_small_sim_players.json` for simulated player deployments.
- Added cloud support for Android workers.
- Added cloud support for iOS workers.
- Added a `MapBuilderSettings` scriptable object containing the default number of layers for the small and large sized worlds.
- Upgraded the project to be compatible with `2018.3.5f1`.

### Fixed

- The Editor now imports NavMeshes correctly when you first open the FPS project.

### Changed

- A simulated player now connects as a regular `UnityClient` worker rather than a `SimulatedPlayer` worker.
- In cloud deployments, `SimulatedPlayerCoordinator` workers are ran in a separate deployment to the `UnityGameLogic` workers.
    - Note that simulated players started by the coordinators still connect into the deployment with the `UnityGameLogic` workers.
- Updated all launch configurations.
	- Replaced transition runtime v2 templates from all launch configurations.
	- Updated cloud launch configurations to use the new `w4_r1000_e1` template.
	- Updated simulated players launch configurations to use the new `sim_players` template.
	- Updated `default_launch.json` to support `SimulatedPlayerCoordinator`.
	- Reduced checkout radius of `UnityClient` and `UnityGameLogic` workers from 210 to 150.
- Increased the level size from 868x868 to 1732x1732.
- Changed how level generation and `MapBuilder.cs` works.
	- `LoadWorld()` now generates the level GameObject through `MapBuilder`, instead of instantiating it from a prefab.

### Removed

- Removed the `SimulatedPlayer` worker type.
- Removed all `FPS-Start_*` level prefabs.

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
