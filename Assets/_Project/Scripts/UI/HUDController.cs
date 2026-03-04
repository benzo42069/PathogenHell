using TMPro;
using UnityEngine;

namespace PathogenHell.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI worldText;
        [SerializeField] private TextMeshProUGUI zoneText;
        [SerializeField] private TextMeshProUGUI eventText;
        [SerializeField] private TextMeshProUGUI energyText;

        public void SetWorld(string world) => worldText.text = $"World: {world}";
        public void SetZone(string zone) => zoneText.text = $"Zone: {zone}";
        public void SetEvent(string ev) => eventText.text = $"Event: {ev}";
        public void SetEnergy(int energy) => energyText.text = $"Mutation Energy: {energy}";
    }
}
