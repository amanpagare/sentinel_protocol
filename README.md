# Sentinel Protocol — Unity 6 Portfolio Game

A compact 3D stealth/AI sandbox starter project for a Unity technical game development portfolio. The game includes a third-person player controller, orbit camera, patrol / investigate / chase enemy AI, field-of-view perception with obstacle raycasts, simple enemy contact attacks, player health, an objective trigger, a restartable win/lose loop, and an in-game debug overlay.


## Requirements

- Unity 6.x
- Universal Render Pipeline (URP) template recommended
- Unity Input System package
- AI Navigation package (`com.unity.ai.navigation`) for NavMeshSurface
- Git (optional)


## Controls

- WASD / arrow keys: move
- Shift: sprint
- Space: jump
- Mouse: orbit camera
- Escape: unlock cursor; click the Game view to lock again
- Tab: toggle AI debug overlay
- R: restart after win/loss

## Gameplay

Reach the green objective zone while avoiding security sentinels. Enemies patrol between waypoints, detect the player with a field-of-view sensor and line-of-sight raycast, investigate the last known position, and chase when the player is visible. A sentinel within attack range damages the player. The HUD shows health and status. Press Tab to inspect AI states and sensor information.

## Included systems

- `PlayerController`: camera-relative movement, sprint, jump, gravity
- `ThirdPersonCamera`: mouse orbit and smooth follow
- `Health`: reusable health/damage/death component
- `EnemySensor`: view distance, view angle, obstacle raycast, Gizmo visualization
- `EnemyNavigator`: waypoint patrol and NavMesh movement
- `SentinelAI`: Patrol, Investigate, Chase, Attack state machine
- `GameManager`: win/loss and restart loop
- `ObjectiveTrigger`: reach-the-objective win condition
- `HUDController`: runtime HUD and debug overlay
- `ArenaBootstrap`: runtime scene setup, fallback arena, NavMesh surface setup and enemy creation

## Troubleshooting

- **Missing AI Navigation types:** install AI Navigation from Package Manager and allow Unity to recompile.
- **No enemy movement:** make sure the NavMesh is baked. The included `ArenaBootstrap` attempts a runtime build when a `NavMeshSurface` is available. If your package version or platform prevents runtime baking, select the ground and bake a NavMesh using the AI Navigation component.
- **Input namespace errors:** install Input System and enable it in Player Settings.
- **Pink materials:** this project uses basic built-in materials. If importing into a URP project, Unity should render them normally; assign URP/Lit materials if needed.
- **Scene opens with missing references:** use the included scene, or create an empty scene and add `ArenaBootstrap` to an empty GameObject; it can generate a simple arena at runtime.


