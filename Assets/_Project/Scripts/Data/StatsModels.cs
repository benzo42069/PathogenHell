using System;
using UnityEngine;

namespace PathogenHell.Data
{
    [Serializable]
    public struct PathogenStats
    {
        public float virulence;
        public float replicationRate;
        public float mutationRate;
        public float mobility;
        public float resistance;
        public float adaptation;
        public float infectivity;
        public float toxicity;
        public float evolutionPotential;

        public static PathogenStats operator +(PathogenStats a, PathogenStats b)
        {
            return new PathogenStats
            {
                virulence = a.virulence + b.virulence,
                replicationRate = a.replicationRate + b.replicationRate,
                mutationRate = a.mutationRate + b.mutationRate,
                mobility = a.mobility + b.mobility,
                resistance = a.resistance + b.resistance,
                adaptation = a.adaptation + b.adaptation,
                infectivity = a.infectivity + b.infectivity,
                toxicity = a.toxicity + b.toxicity,
                evolutionPotential = a.evolutionPotential + b.evolutionPotential,
            };
        }
    }

    [CreateAssetMenu(menuName = "PathogenHell/Defs/ProjectileDef")]
    public class ProjectileDef : ScriptableObject
    {
        public string id;
        public DamageType damageType;
        public float speed = 8f;
        public float lifetime = 2f;
        public float damage = 5f;
        public int pierce;
        public int bounce;
        public int splitCount;
        public bool homing;
        public float homingTurnRate;
        public StatusEffectType statusEffect;
        public float statusStrength;
        public float collisionRadius = 0.2f;
        public Color tint = Color.white;
    }

    [CreateAssetMenu(menuName = "PathogenHell/Defs/PathogenDef")]
    public class PathogenDef : ScriptableObject
    {
        public string id;
        public string displayName;
        public bool unlockedByDefault;
        public PathogenStats baseStats;
        public ProjectileDef primaryProjectile;
        public float fireRate = 6f;
        public string abilityName;
        public float abilityCooldown = 8f;
        public float abilityDuration = 3f;
        public string passiveDescription;
        public Sprite icon;
    }

    [CreateAssetMenu(menuName = "PathogenHell/Defs/EnemyDef")]
    public class EnemyDef : ScriptableObject
    {
        public string id;
        public string displayName;
        public Faction faction;
        public MovementPattern movementPattern;
        public float hp = 25f;
        public float moveSpeed = 2f;
        public float contactDamage = 10f;
        public float attackInterval = 1.5f;
        public ProjectileDef projectile;
        public float dropChance = 0.2f;
        public bool elite;
        public Color tint = Color.red;
    }

    [CreateAssetMenu(menuName = "PathogenHell/Defs/BossDef")]
    public class BossDef : ScriptableObject
    {
        public string id;
        public EnemyDef baseEnemy;
        public float[] phaseThresholds = new[] { 0.7f, 0.35f };
        public ProjectileDef[] phaseProjectiles;
        public float[] phaseAttackIntervals;
    }

    [CreateAssetMenu(menuName = "PathogenHell/Defs/MutationDef")]
    public class MutationDef : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Rarity rarity;
        public PathogenStats additiveStats;
        public float fireRateMultiplier = 1f;
        public int addPierce;
        public int addSplit;
        public bool enableHoming;
        public float healOnPick;
    }

    [CreateAssetMenu(menuName = "PathogenHell/Defs/EvolutionTraitDef")]
    public class EvolutionTraitDef : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public PathogenStats additiveStats;
    }

    [CreateAssetMenu(menuName = "PathogenHell/Defs/HazardDef")]
    public class HazardDef : ScriptableObject
    {
        public string id;
        public string displayName;
        public float tickDamage = 4f;
        public float lifetime = 4f;
        public float radius = 1.25f;
        public Color tint = Color.yellow;
    }

    [CreateAssetMenu(menuName = "PathogenHell/Defs/RandomEventDef")]
    public class RandomEventDef : ScriptableObject
    {
        public string id;
        public EventType eventType;
        [TextArea] public string description;
    }

    [CreateAssetMenu(menuName = "PathogenHell/Defs/WaveDef")]
    public class WaveDef : ScriptableObject
    {
        public string id;
        public EnemyDef enemy;
        public int count = 10;
        public float spawnInterval = 0.75f;
    }

    [CreateAssetMenu(menuName = "PathogenHell/Defs/ZoneDef")]
    public class ZoneDef : ScriptableObject
    {
        public string id;
        public ZoneType zoneType;
        public WaveDef[] waves;
        public BossDef boss;
        public RandomEventDef eventDef;
    }

    [CreateAssetMenu(menuName = "PathogenHell/Defs/WorldDef")]
    public class WorldDef : ScriptableObject
    {
        public string id;
        public WorldType worldType;
        public Color ambientTint = Color.gray;
        public ZoneDef[] zones;
        public HazardDef[] hazards;
    }

    [CreateAssetMenu(menuName = "PathogenHell/Defs/MetaUpgradeDef")]
    public class MetaUpgradeDef : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public int cost = 2;
        public int maxRank = 5;
        public PathogenStats additiveStatsPerRank;
    }
}
