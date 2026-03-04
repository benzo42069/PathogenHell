using System.Collections.Generic;
using PathogenHell.Data;
using UnityEngine;

namespace PathogenHell.Core
{
    public class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }
        public ContentCatalog catalog;
        public PathogenDef selectedPathogen;
        public WorldDef selectedWorld;
        public int evolutionPoints;
        public readonly Dictionary<string, int> purchasedMetaRanks = new();

        private const string EvolutionPointsKey = "PH_EvolutionPoints";

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            evolutionPoints = PlayerPrefs.GetInt(EvolutionPointsKey, 0);
        }

        public void AddEvolutionPoints(int points)
        {
            evolutionPoints += points;
            PlayerPrefs.SetInt(EvolutionPointsKey, evolutionPoints);
            PlayerPrefs.Save();
        }

        public bool TryBuyMetaUpgrade(MetaUpgradeDef def)
        {
            purchasedMetaRanks.TryGetValue(def.id, out var rank);
            if (rank >= def.maxRank || evolutionPoints < def.cost)
            {
                return false;
            }

            evolutionPoints -= def.cost;
            purchasedMetaRanks[def.id] = rank + 1;
            PlayerPrefs.SetInt(EvolutionPointsKey, evolutionPoints);
            PlayerPrefs.Save();
            return true;
        }
    }
}
