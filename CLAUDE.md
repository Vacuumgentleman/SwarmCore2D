# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**SwarmCore 2D** is a Unity 2D swarm shooter engine (Vampire Survivors style) with a data-oriented design (DOD) architecture that cleanly separates simulation from rendering. It supports large enemy counts (2048+) via GPU instanced rendering and a fixed-tick simulation loop.

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

Key structs/classes: `SwarmState` (entity data), `EntityAllocator` (ID pool), `SwarmWorld` (container), `SpatialHashGrid` (broad-phase), `DeterministicRNG` (xorshift32 — use `NextFloat()`, not `Float()`).

### Simulation Loop (`SwarmSimulationController.Update`)

Order matters: `SwarmTime.Step()` → spawn → chase AI → separation → hit flash decay → rebuild spatial grid → contact damage → projectile update → `DropSystem.UpdateDrops()`.

### Namespace Conventions

```
SwarmCore2D.Core              — tick system, math, RNG, spatial grid
SwarmCore2D.Simulation        — SwarmState, world, systems
SwarmCore2D.Rendering         — instanced renderers
SwarmCore2D.Combat            — projectiles, weapons
SwarmCore2D.World             — chunks, biomes, infinite world
SwarmCore2D.Drops             — DropPool, DropSystem, DropRenderer
SwarmCore2D.ScriptableObjects — (files here have no namespace)
```

Top-level MonoBehaviours outside these namespaces (`PlayerStats`, `PlayerInventoryUI`, etc.) are intentionally global.

### Rendering Pattern

All renderers use **pre-allocated buffers** (zero GC per frame) with `Graphics.DrawMeshInstanced`. Key: materials must have **Enable GPU Instancing** checked. Shader properties written per-draw via `MaterialPropertyBlock`: `_Frame`, `_Flip`, `_Tint` per instance; `_FrameCount` on the material itself.

The `InstancedBatcher` handles batching into chunks of 1023 (Unity DrawMeshInstanced limit).

### ScriptableObject Configuration

All balancing is data-driven. Key SOs:
- `SwarmProfile` — tick rate, max entities
- `SwarmDifficultyProfile` — spawn phases, stat curves
- `EnemyData` — per-type stats, drop table (`EnemyDropEntry[]`)
- `WeaponStats` — weapon type, attack pattern, projectile config
- `DropData` — type (Coin/Heal/DamageBoost/SpeedBoost), value, duration, material, visual scale

### Singleton Access Pattern

`DropSystem.Instance`, `PlayerStats.Instance`, `PlayerCurrencySystem.Instance`, `EnemyDatabase.Instance`, `PlayerProgress.Instance` — all null-checked before use (`Instance?.Method()`).

### Time

`SwarmTime` is a deterministic tick clock (60 Hz fixed). Use `SwarmTime.FixedDelta` for simulation updates, `Time.deltaTime` only for pure visual/UI things. `DeterministicRNG` seed pattern: `(uint)(SwarmTime.Tick ^ (uint)(id * 2654435761u))`.

## Unity-Specific Notes

- **Item Prefab** uses `SpriteRenderer`, not UI `Image` — use `GetComponentInChildren<SpriteRenderer>()` for slot icons.
- **GridLayoutGroup** handles inventory slot positioning automatically; don't compute column counts manually (`container.rect.width` is 0 before layout runs).
- **DropPool capacity** is 256. **SwarmState capacity** is set by `SwarmProfile.maxEntities` (default `SwarmConstants.MaxEntities = 2048`).
- Scene setup for drops: `DropSystem` and `DropRenderer` as separate GameObjects; `DropRenderer` needs a `Mesh` assigned (same quad as SwarmRenderer).
