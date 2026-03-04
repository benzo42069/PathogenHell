#if UNITY_EDITOR
using System.Collections.Generic;
using PathogenHell.AI;
using PathogenHell.Combat;
using PathogenHell.Core;
using PathogenHell.Data;
using PathogenHell.Gameplay;
using PathogenHell.Roguelite;
using PathogenHell.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PathogenHell.Tools
{
    public static class ContentSeeder
    {
        [MenuItem("Tools/PathogenHell/Generate Content")]
        public static void Generate()
        {
            EnsureFolders();
            var catalog = CreateOrLoad<ContentCatalog>("Assets/_Project/Data/ContentCatalog.asset");
            catalog.pathogens.Clear(); catalog.projectiles.Clear(); catalog.enemies.Clear(); catalog.bosses.Clear();
            catalog.mutations.Clear(); catalog.traits.Clear(); catalog.worlds.Clear(); catalog.events.Clear(); catalog.waves.Clear(); catalog.metaUpgrades.Clear();

            var projectiles = SeedProjectiles(catalog);
            var enemies = SeedEnemies(catalog, projectiles);
            SeedBosses(catalog, enemies, projectiles);
            SeedMutations(catalog);
            SeedTraits(catalog);
            SeedEvents(catalog);
            var zones = SeedZonesAndWaves(catalog, enemies);
            SeedWorlds(catalog, zones);
            SeedPathogens(catalog, projectiles);
            SeedMeta(catalog);
            EditorUtility.SetDirty(catalog);

            var runtime = CreateRuntimePrefabs();
            CreateScenes(catalog, runtime);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("PathogenHell content generation complete.");
        }

        private static void EnsureFolders()
        {
            string[] folders =
            {
                "Assets/_Project/Data/Pathogens","Assets/_Project/Data/Projectiles","Assets/_Project/Data/Enemies","Assets/_Project/Data/Bosses",
                "Assets/_Project/Data/Mutations","Assets/_Project/Data/Traits","Assets/_Project/Data/Worlds","Assets/_Project/Data/Events",
                "Assets/_Project/Data/Waves","Assets/_Project/Data/MetaProgression","Assets/_Project/Data/Zones","Assets/_Project/Data/Hazards",
                "Assets/_Project/Prefabs/Player","Assets/_Project/Prefabs/Enemies","Assets/_Project/Prefabs/Bosses","Assets/_Project/Prefabs/Projectiles",
                "Assets/_Project/Prefabs/UI","Assets/_Project/Prefabs/World","Assets/_Project/Scenes"
            };
            foreach (var path in folders) if (!AssetDatabase.IsValidFolder(path)) CreateFolderRecursively(path);
        }

        private static void CreateFolderRecursively(string fullPath)
        {
            var parts = fullPath.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        private static T CreateOrLoad<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static Dictionary<string, ProjectileDef> SeedProjectiles(ContentCatalog catalog)
        {
            var names = new[] { "viral-particles", "toxin-droplets", "spore-clusters", "parasite-larvae", "genetic-spikes", "corruption-waves", "bioelectric-arcs", "enzyme-streams" };
            var colors = new[] { Color.cyan, Color.green, Color.magenta, Color.yellow, Color.white, new Color(0.6f,0.2f,0.9f), Color.blue, new Color(1f,0.5f,0.2f)};
            var dict = new Dictionary<string, ProjectileDef>();
            for (var i = 0; i < names.Length; i++)
            {
                var def = CreateOrLoad<ProjectileDef>($"Assets/_Project/Data/Projectiles/{names[i]}.asset");
                def.id = names[i]; def.speed = 7 + i; def.damage = 5 + i; def.lifetime = 2.2f; def.tint = colors[i];
                def.pierce = names[i] == "genetic-spikes" ? 2 : 0;
                def.splitCount = names[i] == "spore-clusters" ? 2 : 0;
                def.homing = names[i] is "parasite-larvae" or "bioelectric-arcs";
                def.statusEffect = names[i] == "toxin-droplets" ? StatusEffectType.Dot : StatusEffectType.None;
                EditorUtility.SetDirty(def);
                catalog.projectiles.Add(def);
                dict[names[i]] = def;
            }
            return dict;
        }

        private static Dictionary<string, EnemyDef> SeedEnemies(ContentCatalog catalog, Dictionary<string, ProjectileDef> proj)
        {
            var entries = new (string id, Faction fac, MovementPattern move, string p, bool elite)[]
            {
                ("neutrophils",Faction.ImmuneSystem,MovementPattern.Chase,"viral-particles",false),
                ("macrophages",Faction.ImmuneSystem,MovementPattern.Straight,"toxin-droplets",false),
                ("natural-killer-cells",Faction.ImmuneSystem,MovementPattern.Sine,"enzyme-streams",false),
                ("antibody-clusters",Faction.ImmuneSystem,MovementPattern.Chase,"parasite-larvae",false),
                ("platelet-barriers",Faction.ImmuneSystem,MovementPattern.Waypoint,"genetic-spikes",false),
                ("antibiotic-capsules",Faction.Pharmaceutical,MovementPattern.Straight,"corruption-waves",false),
                ("antiviral-nanobots",Faction.Pharmaceutical,MovementPattern.Chase,"bioelectric-arcs",false),
                ("antifungal-sprayers",Faction.Pharmaceutical,MovementPattern.Sine,"toxin-droplets",false),
                ("immune-booster-injectors",Faction.Pharmaceutical,MovementPattern.Waypoint,"viral-particles",false),
                ("chemotherapy-waves",Faction.Pharmaceutical,MovementPattern.Straight,"corruption-waves",false),
                ("cytotoxic-t-cells",Faction.ImmuneSystem,MovementPattern.Chase,"genetic-spikes",true),
                ("phagocyte-swarms",Faction.ImmuneSystem,MovementPattern.Sine,"spore-clusters",true),
                ("antibody-storm-nodes",Faction.ImmuneSystem,MovementPattern.Orbit,"parasite-larvae",true),
                ("immune-commanders",Faction.ImmuneSystem,MovementPattern.Waypoint,"bioelectric-arcs",true),
                ("memory-immune-cells",Faction.ImmuneSystem,MovementPattern.Chase,"enzyme-streams",true),
            };

            var dict = new Dictionary<string, EnemyDef>();
            foreach (var e in entries)
            {
                var def = CreateOrLoad<EnemyDef>($"Assets/_Project/Data/Enemies/{e.id}.asset");
                def.id = e.id; def.displayName = e.id; def.faction = e.fac; def.movementPattern = e.move; def.projectile = proj[e.p];
                def.hp = e.elite ? 120f : 40f; def.moveSpeed = e.elite ? 2.4f : 1.8f; def.contactDamage = e.elite ? 16f : 8f; def.elite = e.elite;
                def.tint = e.fac == Faction.ImmuneSystem ? new Color(1f, 0.3f, 0.3f) : new Color(0.4f, 0.6f, 1f);
                EditorUtility.SetDirty(def);
                catalog.enemies.Add(def);
                dict[e.id] = def;
            }
            return dict;
        }

        private static void SeedBosses(ContentCatalog catalog, Dictionary<string, EnemyDef> enemies, Dictionary<string, ProjectileDef> proj)
        {
            CreateBoss(catalog, "macrophage-colossus", enemies["macrophages"], new[] { proj["toxin-droplets"], proj["genetic-spikes"], proj["enzyme-streams"] });
            CreateBoss(catalog, "antibody-supercluster", enemies["antibody-clusters"], new[] { proj["parasite-larvae"], proj["bioelectric-arcs"], proj["corruption-waves"] });
            CreateBoss(catalog, "pharma-overseer", enemies["immune-commanders"], new[] { proj["enzyme-streams"], proj["bioelectric-arcs"], proj["toxin-droplets"] });
        }

        private static void CreateBoss(ContentCatalog catalog, string id, EnemyDef baseEnemy, ProjectileDef[] phases)
        {
            var boss = CreateOrLoad<BossDef>($"Assets/_Project/Data/Bosses/{id}.asset");
            boss.id = id; boss.baseEnemy = baseEnemy; boss.phaseProjectiles = phases; boss.phaseAttackIntervals = new[] { 1.4f, 1f, 0.65f };
            EditorUtility.SetDirty(boss);
            catalog.bosses.Add(boss);
        }

        private static void SeedMutations(ContentCatalog catalog)
        {
            var names = new[]
            {
                "Rapid Replication","Piercing Genome","Homing Instinct","Toxic Membrane","Elastic Flagella","Adaptive Shell",
                "Predatory Burst","Chain Resonance","Viral Bloom","Corrosive Cloud","Metabolic Surge","Resonant Matrix"
            };

            for (var i = 0; i < names.Length; i++)
            {
                var def = CreateOrLoad<MutationDef>($"Assets/_Project/Data/Mutations/{names[i].Replace(" ","-").ToLower()}.asset");
                def.id = names[i].ToLower().Replace(" ", "-"); def.displayName = names[i]; def.description = "Functional run mutation.";
                def.rarity = i > 8 ? Rarity.Legendary : i > 4 ? Rarity.Rare : Rarity.Common;
                def.additiveStats.mobility = i % 3 == 0 ? 0.7f : 0f;
                def.additiveStats.resistance = i % 4 == 0 ? 0.6f : 0f;
                def.fireRateMultiplier = 1f + (i % 2 == 0 ? 0.18f : 0.05f);
                def.addPierce = names[i] == "Piercing Genome" ? 1 : 0;
                def.enableHoming = names[i] == "Homing Instinct";
                def.healOnPick = i == 5 ? 12f : 0f;
                EditorUtility.SetDirty(def);
                catalog.mutations.Add(def);
            }
        }

        private static void SeedTraits(ContentCatalog catalog)
        {
            string[] names = { "Ancient Genome", "Singularity Core", "Hyper Adaptation", "Quantum Plasmid" };
            foreach (var name in names)
            {
                var def = CreateOrLoad<EvolutionTraitDef>($"Assets/_Project/Data/Traits/{name.Replace(" ","-").ToLower()}.asset");
                def.id = name.ToLower().Replace(" ", "-"); def.displayName = name; def.description = "Rare evolution trait.";
                def.additiveStats.evolutionPotential = 1f; def.additiveStats.virulence = 0.5f;
                EditorUtility.SetDirty(def);
                catalog.traits.Add(def);
            }
        }

        private static void SeedEvents(ContentCatalog catalog)
        {
            foreach (EventType type in System.Enum.GetValues(typeof(EventType)))
            {
                var def = CreateOrLoad<RandomEventDef>($"Assets/_Project/Data/Events/{type.ToString().ToLower()}.asset");
                def.id = type.ToString().ToLower(); def.eventType = type; def.description = $"Event: {type}";
                EditorUtility.SetDirty(def);
                catalog.events.Add(def);
            }
        }

        private static List<ZoneDef> SeedZonesAndWaves(ContentCatalog catalog, Dictionary<string, EnemyDef> enemies)
        {
            var result = new List<ZoneDef>();
            WaveDef CreateWave(string id, EnemyDef e, int count)
            {
                var w = CreateOrLoad<WaveDef>($"Assets/_Project/Data/Waves/{id}.asset");
                w.id = id; w.enemy = e; w.count = count; w.spawnInterval = 0.7f; EditorUtility.SetDirty(w); catalog.waves.Add(w); return w;
            }

            var w1 = CreateWave("z1_wave_a", enemies["neutrophils"], 8);
            var w2 = CreateWave("z1_wave_b", enemies["macrophages"], 8);
            var w3 = CreateWave("z2_wave_a", enemies["antibody-clusters"], 10);
            var w4 = CreateWave("z3_wave_a", enemies["antiviral-nanobots"], 12);
            var elite = CreateWave("elite_wave", enemies["cytotoxic-t-cells"], 6);

            ZoneDef CreateZone(string id, ZoneType type, WaveDef[] waves = null, string ev = null, string boss = null)
            {
                var z = CreateOrLoad<ZoneDef>($"Assets/_Project/Data/Zones/{id}.asset");
                z.id = id; z.zoneType = type; z.waves = waves;
                z.eventDef = ev == null ? null : catalog.events.Find(e => e.id == ev);
                z.boss = boss == null ? null : catalog.bosses.Find(b => b.id == boss);
                EditorUtility.SetDirty(z);
                return z;
            }

            result.Add(CreateZone("skin-infection-1", ZoneType.Infection, new[] { w1, w2 }));
            result.Add(CreateZone("skin-infection-2", ZoneType.Infection, new[] { w3 }));
            result.Add(CreateZone("skin-infection-3", ZoneType.Infection, new[] { w4 }));
            result.Add(CreateZone("skin-elite-defense", ZoneType.EliteDefense, new[] { elite }));
            result.Add(CreateZone("mutation-chamber", ZoneType.MutationEvent, null, "mutationchamber"));
            result.Add(CreateZone("skin-boss", ZoneType.Boss, new[] { w2 }, null, "macrophage-colossus"));
            return result;
        }

        private static void SeedWorlds(ContentCatalog catalog, List<ZoneDef> demoZones)
        {
            foreach (WorldType worldType in System.Enum.GetValues(typeof(WorldType)))
            {
                var def = CreateOrLoad<WorldDef>($"Assets/_Project/Data/Worlds/{worldType.ToString().ToLower()}.asset");
                def.id = worldType.ToString();
                def.worldType = worldType;
                def.ambientTint = Color.HSVToRGB(((int)worldType) / 6f, 0.25f, 0.25f);
                def.zones = worldType == WorldType.Skin ? demoZones.ToArray() : new[] { demoZones[0], demoZones[^1] };
                EditorUtility.SetDirty(def);
                catalog.worlds.Add(def);
            }

            var antiseptic = CreateOrLoad<HazardDef>("Assets/_Project/Data/Hazards/antiseptic-bursts.asset");
            antiseptic.id = "antiseptic-bursts"; antiseptic.displayName = "Antiseptic Bursts"; antiseptic.tickDamage = 8f; antiseptic.tint = Color.yellow;
            var shedding = CreateOrLoad<HazardDef>("Assets/_Project/Data/Hazards/skin-shedding-waves.asset");
            shedding.id = "skin-shedding-waves"; shedding.displayName = "Skin Shedding Waves"; shedding.tickDamage = 5f; shedding.tint = Color.gray;
            EditorUtility.SetDirty(antiseptic); EditorUtility.SetDirty(shedding);
            catalog.worlds[0].hazards = new[] { antiseptic, shedding };
        }

        private static void SeedPathogens(ContentCatalog catalog, Dictionary<string, ProjectileDef> proj)
        {
            var entries = new (string id,string name,string p,bool unlocked)[]
            {
                ("influenza-strain","Influenza Strain","viral-particles",true),
                ("streptococcus-colony","Streptococcus Colony","toxin-droplets",true),
                ("protozoa-parasite","Protozoa Parasite","parasite-larvae",true),
                ("retrovirus","Retrovirus","genetic-spikes",false),
                ("fungal-mycelium","Fungal Mycelium","spore-clusters",false),
                ("prion-entity","Prion Entity","corruption-waves",false),
                ("nanovirus","Nanovirus","bioelectric-arcs",false),
                ("chimera-organism","Chimera Organism","enzyme-streams",false),
            };

            foreach (var e in entries)
            {
                var def = CreateOrLoad<PathogenDef>($"Assets/_Project/Data/Pathogens/{e.id}.asset");
                def.id = e.id; def.displayName = e.name; def.primaryProjectile = proj[e.p]; def.unlockedByDefault = e.unlocked;
                def.baseStats = new PathogenStats{mobility = 2f, resistance = 1f, virulence = 1f, infectivity = 1f, replicationRate = 1f, mutationRate=1f,toxicity=1f, adaptation=1f,evolutionPotential=1f};
                def.fireRate = 6f; def.abilityName = "Adaptive Burst"; def.abilityCooldown = 9f; def.abilityDuration = 2.5f; def.passiveDescription = "Species-specific adaptive passive.";
                EditorUtility.SetDirty(def);
                catalog.pathogens.Add(def);
            }
        }

        private static void SeedMeta(ContentCatalog catalog)
        {
            var names = new[] { "Attack Mutations", "Defensive Traits", "Infection Efficiency", "Resistance Genes", "Mutation Discovery", "Aggressive Splicing", "Cellular Fortification", "Bioelectric Focus", "Rapid Bloom", "Adaptive Synthesis" };
            foreach (var name in names)
            {
                var def = CreateOrLoad<MetaUpgradeDef>($"Assets/_Project/Data/MetaProgression/{name.Replace(" ","-").ToLower()}.asset");
                def.id = name.ToLower().Replace(" ", "-"); def.displayName = name; def.description = "Permanent evolution node."; def.cost = 2; def.maxRank = 5;
                def.additiveStatsPerRank = new PathogenStats{virulence = 0.2f, resistance = 0.2f};
                EditorUtility.SetDirty(def);
                catalog.metaUpgrades.Add(def);
            }
        }

        private static (GameObject player, GameObject enemy, ProjectileRuntime projectile) CreateRuntimePrefabs()
        {
            var projectileGo = new GameObject("ProjectileRuntime", typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(ProjectileRuntime));
            projectileGo.GetComponent<CircleCollider2D>().isTrigger = true;
            projectileGo.GetComponent<SpriteRenderer>().sprite = MakeSquareSprite("ProjSprite", Color.white);
            var projectilePrefab = PrefabUtility.SaveAsPrefabAsset(projectileGo, "Assets/_Project/Prefabs/Projectiles/Projectile.prefab");
            Object.DestroyImmediate(projectileGo);

            var player = new GameObject("Player", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(HealthComponent), typeof(WeaponController), typeof(MutationManager), typeof(PlayerController));
            player.tag = "Player";
            player.GetComponent<SpriteRenderer>().sprite = MakeSquareSprite("PlayerSprite", Color.green);
            player.GetComponent<CircleCollider2D>().isTrigger = true;
            player.GetComponent<Rigidbody2D>().gravityScale = 0f;
            var playerPrefab = PrefabUtility.SaveAsPrefabAsset(player, "Assets/_Project/Prefabs/Player/Player.prefab");
            Object.DestroyImmediate(player);

            var enemy = new GameObject("Enemy", typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(HealthComponent), typeof(WeaponController), typeof(EnemyController));
            enemy.tag = "Enemy";
            enemy.GetComponent<SpriteRenderer>().sprite = MakeSquareSprite("EnemySprite", Color.red);
            enemy.GetComponent<CircleCollider2D>().isTrigger = true;
            var enemyPrefab = PrefabUtility.SaveAsPrefabAsset(enemy, "Assets/_Project/Prefabs/Enemies/Enemy.prefab");
            Object.DestroyImmediate(enemy);

            return (playerPrefab, enemyPrefab, projectilePrefab.GetComponent<ProjectileRuntime>());
        }

        private static Sprite MakeSquareSprite(string name, Color c)
        {
            var path = $"Assets/_Project/Art/{name}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;
            var tex = new Texture2D(16, 16);
            var pixels = new Color[16 * 16];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = c;
            tex.SetPixels(pixels); tex.Apply();
            var pngPath = $"Assets/_Project/Art/{name}.png";
            System.IO.File.WriteAllBytes(pngPath, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(pngPath);
            var importer = (TextureImporter)AssetImporter.GetAtPath(pngPath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 16;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);
        }

        private static void CreateScenes(ContentCatalog catalog, (GameObject player, GameObject enemy, ProjectileRuntime projectile) runtime)
        {
            CreateBootScene(catalog);
            CreateMainMenuScene();
            CreateRunScene(runtime);
            CreateMetaScene();
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/_Project/Scenes/Boot.unity", true),
                new EditorBuildSettingsScene("Assets/_Project/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/_Project/Scenes/RunScene_DemoWorld1.unity", true),
                new EditorBuildSettingsScene("Assets/_Project/Scenes/MetaProgression.unity", true),
            };
        }

        private static void CreateBootScene(ContentCatalog catalog)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject("BootRoot");
            var gs = root.AddComponent<GameSession>();
            gs.catalog = catalog;
            root.AddComponent<BootLoader>();
            EditorSceneManager.SaveScene(scene, "Assets/_Project/Scenes/Boot.unity");
        }

        private static void CreateMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var canvas = CreateCanvas();
            var menu = new GameObject("MainMenuController", typeof(MainMenuController));
            var ctrl = menu.GetComponent<MainMenuController>();
            AddButton(canvas.transform, "Start Run", new Vector2(0, 90), ctrl.StartRun);
            AddButton(canvas.transform, "Meta Progression", new Vector2(0, 30), ctrl.OpenMeta);
            AddButton(canvas.transform, "Quit", new Vector2(0, -30), ctrl.Quit);
            var selectGo = new GameObject("CharacterSelect", typeof(CharacterSelectUI));
            selectGo.transform.SetParent(canvas.transform);
            var template = AddButton(canvas.transform, "Template", new Vector2(220, 130), null);
            template.gameObject.SetActive(false);
            var root = new GameObject("CharacterButtons", typeof(RectTransform));
            root.transform.SetParent(canvas.transform);
            var cs = selectGo.GetComponent<CharacterSelectUI>();
            cs.GetType().GetField("buttonRoot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cs, root.transform);
            cs.GetType().GetField("buttonTemplate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cs, template);
            EditorSceneManager.SaveScene(scene, "Assets/_Project/Scenes/MainMenu.unity");
        }

        private static void CreateRunScene((GameObject player, GameObject enemy, ProjectileRuntime projectile) runtime)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)).tag = "MainCamera";
            Object.Instantiate(runtime.player, Vector3.zero, Quaternion.identity);
            var pool = new GameObject("ProjectilePool", typeof(ProjectilePool));
            var poolComp = pool.GetComponent<ProjectilePool>();
            poolComp.GetType().GetField("projectilePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(poolComp, runtime.projectile);

            var spawnerGo = new GameObject("WaveSpawner", typeof(WaveSpawner));
            var spawner = spawnerGo.GetComponent<WaveSpawner>();
            var points = new Transform[4];
            for (var i = 0; i < 4; i++)
            {
                var p = new GameObject($"Spawn_{i}").transform;
                p.position = new Vector3(-6 + i * 4, 5, 0);
                points[i] = p;
            }
            spawner.GetType().GetField("spawnPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(spawner, points);
            spawner.GetType().GetField("enemyPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(spawner, runtime.enemy);

            var runMgr = new GameObject("RunManager", typeof(RunManager)).GetComponent<RunManager>();
            runMgr.GetType().GetField("spawner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(runMgr, spawner);

            var uiCanvas = CreateCanvas();
            var hudGo = new GameObject("HUD", typeof(HUDController));
            hudGo.transform.SetParent(uiCanvas.transform);
            var hud = hudGo.GetComponent<HUDController>();
            var w = AddText(uiCanvas.transform, "World", new Vector2(-280, 160));
            var z = AddText(uiCanvas.transform, "Zone", new Vector2(-280, 130));
            var e = AddText(uiCanvas.transform, "Event", new Vector2(-280, 100));
            var en = AddText(uiCanvas.transform, "Energy", new Vector2(-280, 70));
            SetPrivate(hud, "worldText", w); SetPrivate(hud, "zoneText", z); SetPrivate(hud, "eventText", e); SetPrivate(hud, "energyText", en);
            runMgr.GetType().GetField("hud", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(runMgr, hud);

            var mutGo = new GameObject("MutationUI", typeof(MutationSelectionUI));
            mutGo.transform.SetParent(uiCanvas.transform);
            var panel = new GameObject("Panel", typeof(RectTransform), typeof(Image)); panel.transform.SetParent(mutGo.transform);
            panel.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 220);
            var buttons = new Button[3]; var labels = new TextMeshProUGUI[3];
            for (var i = 0; i < 3; i++)
            {
                buttons[i] = AddButton(panel.transform, $"Choice{i+1}", new Vector2(-220 + i * 220, 0), null);
                labels[i] = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            }
            var mut = mutGo.GetComponent<MutationSelectionUI>();
            SetPrivate(mut, "panel", panel);
            SetPrivate(mut, "choiceButtons", buttons);
            SetPrivate(mut, "labels", labels);
            runMgr.GetType().GetField("mutationUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(runMgr, mut);

            for (var i = 0; i < 8; i++)
            {
                var cell = new GameObject($"Cell_{i}", typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(InfectableCell));
                cell.transform.position = new Vector3(-7 + i * 2, -3.8f, 0);
                cell.GetComponent<SpriteRenderer>().sprite = MakeSquareSprite("CellSprite", new Color(0.5f, 0.8f, 0.5f));
                cell.GetComponent<CircleCollider2D>().isTrigger = true;
            }
            EditorSceneManager.SaveScene(scene, "Assets/_Project/Scenes/RunScene_DemoWorld1.unity");
        }

        private static void CreateMetaScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var canvas = CreateCanvas();
            var go = new GameObject("MetaUI", typeof(MetaProgressionUI));
            go.transform.SetParent(canvas.transform);
            var ui = go.GetComponent<MetaProgressionUI>();
            var points = AddText(canvas.transform, "Evolution Points: 0", new Vector2(0, 160));
            var template = AddButton(canvas.transform, "Upgrade", new Vector2(0, 100), null);
            var root = new GameObject("UpgradeRoot", typeof(RectTransform)); root.transform.SetParent(canvas.transform);
            SetPrivate(ui, "pointsText", points); SetPrivate(ui, "root", root.transform); SetPrivate(ui, "buttonTemplate", template);
            AddButton(canvas.transform, "Back", new Vector2(0, -170), ui.BackToMenu);
            EditorSceneManager.SaveScene(scene, "Assets/_Project/Scenes/MetaProgression.unity");
        }

        private static Canvas CreateCanvas()
        {
            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
            return canvas;
        }

        private static Button AddButton(Transform root, string text, Vector2 anchoredPos, UnityEngine.Events.UnityAction action)
        {
            var go = new GameObject(text, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(root);
            var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(190, 45); rt.anchoredPosition = anchoredPos;
            var label = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            label.transform.SetParent(go.transform); label.rectTransform.sizeDelta = rt.sizeDelta; label.alignment = TextAlignmentOptions.Center; label.text = text; label.fontSize = 20;
            var b = go.GetComponent<Button>(); if (action != null) b.onClick.AddListener(action);
            return b;
        }

        private static TextMeshProUGUI AddText(Transform root, string text, Vector2 pos)
        {
            var go = new GameObject(text, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(root);
            var t = go.GetComponent<TextMeshProUGUI>();
            t.rectTransform.sizeDelta = new Vector2(400, 30); t.rectTransform.anchoredPosition = pos;
            t.fontSize = 22; t.text = text;
            return t;
        }

        private static void SetPrivate(object target, string field, object value)
        {
            target.GetType().GetField(field, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(target, value);
        }
    }
}
#endif
