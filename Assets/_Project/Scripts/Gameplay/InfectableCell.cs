using PathogenHell.Data;
using UnityEngine;

namespace PathogenHell.Gameplay
{
    public class InfectableCell : MonoBehaviour
    {
        public CellType cellType;
        public int energyReward = 5;
        private bool _infected;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_infected || !other.CompareTag("Player")) return;
            _infected = true;
            FindAnyObjectByType<RunManager>()?.AddMutationEnergy(energyReward);
            GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}
