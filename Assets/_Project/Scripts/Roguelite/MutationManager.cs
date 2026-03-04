using System.Collections.Generic;
using PathogenHell.Combat;
using PathogenHell.Core;
using PathogenHell.Data;
using UnityEngine;

namespace PathogenHell.Roguelite
{
    public class MutationManager : MonoBehaviour
    {
        private readonly List<MutationDef> _selected = new();
        private PathogenDef _pathogen;
        public PathogenStats CurrentStats { get; private set; }

        public void Initialize(PathogenDef pathogen)
        {
            _pathogen = pathogen;
            CurrentStats = pathogen.baseStats;
        }

        public void ApplyMutation(MutationDef def)
        {
            _selected.Add(def);
            CurrentStats += def.additiveStats;
            if (def.healOnPick > 0f) GetComponent<HealthComponent>()?.Heal(def.healOnPick);

            var weapon = GetComponent<WeaponController>();
            if (weapon != null)
            {
                var rate = _pathogen.fireRate * def.fireRateMultiplier;
                weapon.Configure(_pathogen.primaryProjectile, rate, true);
            }
        }

        public List<MutationDef> RollChoices(int count)
        {
            var pool = GameSession.Instance.catalog.mutations;
            var choices = new List<MutationDef>(count);
            for (var i = 0; i < count; i++)
            {
                choices.Add(pool[Random.Range(0, pool.Count)]);
            }
            return choices;
        }
    }
}
