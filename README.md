# Pathogen: Inside the Host (Unity 6000.3.8f1)

Top-down vertical bullet hell roguelite prototype with data-driven content and editor-time content generation.

## Why this implementation
This repository uses **Option B (ContentSeeder)**: all ScriptableObjects, prefabs, and scenes are generated via `Tools -> PathogenHell -> Generate Content`.

This keeps the repo script-centric, idempotent, and easy to regenerate without brittle YAML merge conflicts.

## Open and run
1. Open Unity Hub and add this project folder.
2. Use Unity Editor version **6000.3.8f1**.
3. Open project and let packages import.
4. Run menu command: **Tools → PathogenHell → Generate Content**.
5. Open scene `Assets/_Project/Scenes/Boot.unity` and press Play.

## Controls
- Move: `WASD` / Arrow keys
- Auto-fire: always on
- Mutation selection: click one of 3 choices

## Included systems
- Boot + MainMenu + RunScene_DemoWorld1 + MetaProgression scenes
- ScriptableObject definitions for pathogens, projectiles, enemies, bosses, mutations, traits, worlds, events, zones, waves, hazards, and meta upgrades
- Content catalog registry
- Wave spawning + enemy AI + projectile pooling
- Infectable cells that grant mutation energy
- Event handling (Mutation Chamber, Host Weakness, Evolution Catalyst)
- Mutation pick UI and meta progression spend UI

## Data-driven workflow
All balancing lives in generated ScriptableObjects under `Assets/_Project/Data/*`.

### Add a new pathogen
1. Duplicate an existing `PathogenDef` asset.
2. Set stats, projectile, fire rate, ability/passive text.
3. Add it to `ContentCatalog.pathogens`.

### Add a new enemy
1. Create an `EnemyDef`.
2. Choose faction, movement pattern, projectile, tuning values.
3. Add a `WaveDef` referencing this enemy.

### Add a new projectile
1. Create a `ProjectileDef`.
2. Configure speed/lifetime/damage/pierce/split/homing/status.
3. Assign to pathogens/enemies.

### Add a new mutation
1. Create `MutationDef`.
2. Tune rarity and stat/effect modifiers.
3. Add to `ContentCatalog.mutations`.

## Troubleshooting
- **Null references after pull:** run `Tools -> PathogenHell -> Generate Content` again.
- **No TMP text rendering:** import TMP Essentials once from Unity's prompt.
- **Scene not in build:** regenerate content (seeder sets Build Settings scenes).
- **Old generated assets:** seeder is idempotent and updates existing assets in-place.
