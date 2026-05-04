# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**SwarmCore 2D** is a Unity 2D swarm shooter engine (Vampire Survivors style) with a data-oriented design (DOD) architecture that cleanly separates simulation from rendering. It supports large enemy counts (2048+) via GPU instanced rendering and a fixed-tick simulation loop. Target: Unity 6 LTS, URP, WebGL-compatible.

## Architecture

### Core Pattern: Data-Oriented Design

The simulation uses **parallel arrays in SwarmState** rather than entity objects:

```csharp
// Always iterate using activeList, not raw index:
int count = state.activeCount;
for (int a = 0; a < count; a++)
{
    int id = state.activeList[a];
    // read/write state.*[id]
}
```

Key types: `SwarmState` (enemy entity data), `EntityAllocator` (ID pool), `SwarmWorld` (container), `SpatialHashGrid` (broad-phase), `DeterministicRNG` (xorshift32 — use `NextFloat()`, not `Float()`), `PlayerStatsRuntime` (runtime multipliers: damage/cooldown/area/speed/duration + extraProjectiles, pierceBonus, critChance, lifeSteal).

### Simulation Loop (`SwarmSimulationController.Update`)

Order matters: `SwarmTime.Step()` → infinite world + chunk update → spawn → chase AI → separation → hit flash decay → rebuild spatial grid → contact damage → projectile update → `DropSystem.UpdateDrops()`.

### Namespace Conventions

```
SwarmCore2D.Core             — SwarmTime, DeterministicRNG, SpatialHashGrid, SwarmConstants
SwarmCore2D.Simulation       — SwarmState, SwarmWorld, EnemySpawnerSystem, EnemyChaseSystem,
                               EnemySeparationSystem, EnemyContactDamageSystem, PlayerMeleeAttackSystem
SwarmCore2D.Rendering        — SwarmRenderer, AttackVisualRenderer, InstancedBatcher
SwarmCore2D.Combat           — AttackVisualState/System, ProjectileSystem, ProjectileRenderer,
                               DamageData, WeaponRuntimeStats
SwarmCore2D.World            — InfiniteWorldSystem, WorldChunkSystem, WorldRenderer, BiomeSystem, ChestSpawner
SwarmCore2D.Drops            — DropPool, DropSystem, DropRenderer
SwarmCore2D.Replay           — ReplayRecorder, ReplayPlayer, InputCommand, InputBuffer
SwarmCore2D.MultiplayerReady — LockstepValidator, SimulationChecksum
SwarmCore2D.ScriptableObjects — (files here have no namespace)
```

**No-namespace (global) MonoBehaviours** — intentionally ungrouped: `PlayerController`, `PlayerStats`, `PlayerStatsRuntime`, `PlayerHealth`, `PlayerCurrencySystem`, `PlayerProgress`, `PlayerWeaponController`, `UpgradeSystem`, `UpgradeUI`, `UpgradeCardUI`, `UpgradeDatabase`, `FloatingTextSpawner`, `ChestController`, `PauseMenu`, `MenuNavigation`, `OptionsMenuNavigation`, `UIInputMode`, and all other UI components.

`ProjectileSystem` and `ProjectileRenderer` live in `Runtime/Combat/Projectiles/`, not `Runtime/Rendering/`.

### Rendering Pattern

All renderers use **pre-allocated buffers** (zero GC per frame) with `Graphics.DrawMeshInstanced`. Materials must have **Enable GPU Instancing** checked. Shader properties via `MaterialPropertyBlock`: `_Frame`, `_Flip`, `_Tint` per instance; `_FrameCount` on the material itself. `InstancedBatcher` handles batching in chunks of 1023 (Unity limit).

### Attack Visuals & Projectile Rendering

All attack visuals (melee slash, area AoE, orbit, projectile sprites) use `AttackVisualSystem` + `AttackVisualRenderer` — no GameObjects, no `Instantiate`/`Destroy`.

- `AttackVisualState` (cap 64): parallel arrays with free-stack allocator, same pattern as `DropPool`
- `AttackVisualSystem`: created in `PlayerWeaponController.Awake()` (not `Start()`) so renderers can find it
- `AttackVisualRenderer`: MonoBehaviour in scene, lazy-finds the system in `LateUpdate()`
- **Orbit visibility**: use `state.scales[id] = 0f` to hide — never call `Deactivate()` on orbit IDs, or they get recycled and cross-contaminate other visuals
- `WeaponStats` fields: `attackVisualMaterial`, `attackVisualFrameCount`, `attackVisualFrameRate`, `attackVisualScale` — existing assets serialize new float/int fields as 0, not the C# default; `EffectiveVisualScale()` in `PlayerWeaponController` returns `1f` when `attackVisualScale == 0f`
- `ProjectileRenderer` groups by `state.material[i]` per projectile — each projectile carries its material from `WeaponStats`

### Weapon System

`WeaponStats` drives `WeaponRuntimeStats` (runtime copy loaded in `InitializeWeapons()`). Attack types: `Melee`, `Projectile`, `Area`, `Orbit`. Direction modes: `Clockwise`, `Alternating`, `Volley`.

`WeaponStats.scaledBy*` flags gate which `PlayerStatsRuntime` multipliers apply to a weapon:
- `scaledByMight` → `damageMultiplier`
- `scaledByAmount` → `extraProjectiles`
- `scaledByArea` → `areaMultiplier`
- `scaledBySpeed` → `speedMultiplier`
- `scaledByDuration` → `durationMultiplier`

### Progression & Upgrade System

`PlayerProgress` (XP/level) fires `OnLevelUp` → `UpgradeSystem.OpenSelection()` pauses simulation and shows the upgrade panel → `UpgradeUI.GenerateOptions()` pulls random options from `UpgradeDatabase.Instance` → player picks → `UpgradeCardUI.OnClick()` applies stat changes to `PlayerStatsRuntime` or mutates a `WeaponRuntimeStats`. `UpgradeData` SO: `isGlobal` (vs per-weapon), `type` enum, stat delta.

**Tier/Rarity system** lives in `UpgradeUI`: `TierDefinition[]` tiers (Common 40%×1.0 → Legendary 2%×3.0). `UpgradeUI.RollTier()` does weighted random selection; `UpgradeCardUI.Setup()` calls it and multiplies `upgrade.value * tier.multiplier` for the effective upgrade value. Per-upgrade level tracking is a static `Dictionary<(UpgradeData, weaponIndex), int>` in `UpgradeCardUI`; global upgrades use `weaponIndex = -1`. Call `UpgradeCardUI.ClearAllLevels()` on game reset.

The upgrade card button uses a UI `Image` for the `cardBackground` field (assign in Inspector). `UpgradeCardUI.Awake()` sets that Image as `Button.targetGraphic`; if `cardBackground` is null it falls back to reusing or adding a transparent `Image` with `raycastTarget = true` — **the raycastTarget must be true or the GraphicRaycaster will never deliver clicks to the button**.

### Drop & Chest System

- `DropSystem` (singleton): `SpawnDrops()` / `SpawnFromTable()` roll `EnemyDropEntry[]` tables with `DeterministicRNG`. Drop types: `Coin`, `Heal`, `DamageBoost`, `SpeedBoost`, `WeaponUnlock` (calls `PlayerWeaponController.Instance?.AddWeapon()`).
- `ChestController`: player walks in range → E key → `DropSystem.SpawnFromTable()` + `FloatingTextSpawner` notification. Requires `Cainos.PixelArtPlatformer_VillageProps.Chest` component on the prefab; `ChestController` grabs it via `GetComponent<>` in `Awake()`.
- `FloatingTextSpawner` (singleton): `Spawn(Vector2, string, Color)` — TMP pool of 10 (Inspector-configurable), auto-fades and floats upward.

### Replay & Multiplayer Infrastructure

Not yet wired to the active game loop — infrastructure only:
- `SwarmCore2D.Replay`: `ReplayRecorder` accumulates `InputCommand` list; `ReplayPlayer` replays them deterministically.
- `SwarmCore2D.MultiplayerReady`: `SimulationChecksum.Compute(SwarmState)` + `LockstepValidator` for frame-by-frame determinism verification.

### ScriptableObject Configuration

All balancing is data-driven. Key SOs:
- `SwarmProfile` — tick rate, max entities
- `SwarmDifficultyProfile` — spawn phases, stat curves
- `SwarmRenderProfile` — render settings
- `EnemyData` — per-type stats, drop table (`EnemyDropEntry[]`)
- `WeaponStats` — attack type/pattern, projectile config, attack visual (material/frameCount/frameRate/scale), `scaledBy*` flags
- `DropData` — type, value, duration, collect radius, material, visual scale; `weaponToUnlock` for `WeaponUnlock` type
- `UpgradeData` — `isGlobal`, upgrade type, stat delta
- `ChestData` — tier name, prefab (needs `Cainos.Chest`), interact radius, loot table (`EnemyDropEntry[]`)
- `ChestSpawnProfile` — chest spawn rules for `ChestSpawner`
- `BiomeData`, `PropData` — infinite world/biome configuration

### UI Navigation System (Mouse + Keyboard/Gamepad)

`UIInputMode` is a `DontDestroyOnLoad` singleton auto-created via `[RuntimeInitializeOnLoadMethod(AfterSceneLoad)]` — never place it in a scene manually. It detects the last active input device and fires `OnModeChanged` when switching. Navigation scripts subscribe to react:

- **Mouse mode**: cursor visible, EventSystem selection is left completely untouched (pointer clicks work without a selected object).
- **Keyboard/Gamepad mode**: cursor hidden, one button is always kept selected; arrow/WASD/dpad navigate; Space (keyboard) or A-button (gamepad) confirm.
- `UIInputMode.SetMode()` must **never** call `EventSystem.SetSelectedGameObject()` — doing so can interrupt pointer click delivery in the same frame.

**Scripts:**
- `MenuNavigation` — attach to any menu root Canvas/panel. Subscribes to `OnModeChanged`; `Update()` restores selection if lost while in keyboard mode. In mouse mode it does nothing to the EventSystem.
- `OptionsMenuNavigation` — for panels with `Slider[]` + a back `Button`. Explicit `Navigation.Mode` with `selectOnLeft/Right = null` so sliders change value on left/right instead of navigating. B/Escape always invoke back regardless of mode.
- `UpgradeUI.SetupCardNavigation()` — sets up vertical Explicit navigation on upgrade cards; only `SetSelectedGameObject` if `UIInputMode.Current == Keyboard`.

**InputSystem gotcha:** both Demo scenes reference the Unity package's `DefaultInputActions` (GUID `ca9f5fa95ffab41fb9a615ab714db018`), **not** the project's `InputSystem_Actions.inputactions`. Edits to the project's asset have no effect on those scenes. Space is not in DefaultInputActions Submit, so Space-to-confirm must always be handled manually in code.

### Singleton Access Pattern

Always null-check before use (`Instance?.Method()`):

| Singleton | Notes |
|-----------|-------|
| `DropSystem.Instance` | |
| `PlayerStats.Instance` | holds `PlayerStatsRuntime stats` |
| `PlayerCurrencySystem.Instance` | |
| `PlayerProgress.Instance` | XP/level, fires `OnLevelUp` |
| `PlayerWeaponController.Instance` | set in `Awake()`, cleared in `OnDestroy()` |
| `EnemyDatabase.Instance` | |
| `UpgradeUI.Instance` | |
| `FloatingTextSpawner.Instance` | |
| `UIInputMode.Current` / `UIInputMode.OnModeChanged` | static, no Instance field; auto-created at runtime |

### Time & Pausing

`SwarmTime` is a deterministic tick clock (60 Hz fixed). Use `SwarmTime.FixedDelta` for simulation updates, `Time.deltaTime` only for visual/UI. `DeterministicRNG` seed pattern: `(uint)(SwarmTime.Tick ^ (uint)(id * 2654435761u))`.

**Pausing**: `SwarmSimulationController.SetPaused(bool)` gates its own `Update`. `PlayerWeaponController.Update` checks `SwarmTime.Paused` separately. `UpgradeSystem` sets both via `SetPaused()` when opening the upgrade panel — keep these in sync when adding new pause callsites.

## Unity-Specific Notes

- **Item Prefab** uses `SpriteRenderer`, not UI `Image` — use `GetComponentInChildren<SpriteRenderer>()` for slot icons.
- **GridLayoutGroup** handles inventory slot positioning automatically; don't compute column counts manually (`container.rect.width` is 0 before layout runs).
- **DropPool capacity** is 256. **AttackVisualState capacity** is 64. **SwarmState capacity** is set by `SwarmProfile.maxEntities` (default `SwarmConstants.MaxEntities = 2048`). **FloatingTextSpawner pool** defaults to 10 (Inspector field).
- Scene setup for drops: `DropSystem` and `DropRenderer` as separate GameObjects; `DropRenderer` needs a `Mesh` assigned (same quad as SwarmRenderer).
- Scene setup for attack visuals: `AttackVisualRenderer` as a separate GameObject with `Mesh` assigned; auto-finds `AttackVisualSystem` via `PlayerWeaponController` at runtime.
- `ProjectileSystem.Spawn()`: `duration <= 0f` and `maxRange <= 0f` are treated as `Infinity` — don't pass 0 expecting "no limit" without this being intentional.
- Chest prefab must have `Cainos.PixelArtPlatformer_VillageProps.Chest` attached; `ChestController` gets it via `GetComponent<>` in `Awake()`.
