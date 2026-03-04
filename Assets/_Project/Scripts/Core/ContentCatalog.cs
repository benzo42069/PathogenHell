using System.Collections.Generic;
using PathogenHell.Data;
using UnityEngine;

namespace PathogenHell.Core
{
    [CreateAssetMenu(menuName = "PathogenHell/ContentCatalog")]
    public class ContentCatalog : ScriptableObject
    {
        public List<PathogenDef> pathogens = new();
        public List<ProjectileDef> projectiles = new();
        public List<EnemyDef> enemies = new();
        public List<BossDef> bosses = new();
        public List<MutationDef> mutations = new();
        public List<EvolutionTraitDef> traits = new();
        public List<WorldDef> worlds = new();
        public List<RandomEventDef> events = new();
        public List<WaveDef> waves = new();
        public List<MetaUpgradeDef> metaUpgrades = new();

        public PathogenDef FindPathogen(string id) => pathogens.Find(p => p.id == id);
    }
}
