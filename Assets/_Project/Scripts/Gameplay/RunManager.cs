using System.Collections;
using PathogenHell.Core;
using PathogenHell.Data;
using PathogenHell.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PathogenHell.Gameplay
{
    public class RunManager : MonoBehaviour
    {
        [SerializeField] private WaveSpawner spawner;
        [SerializeField] private MutationSelectionUI mutationUI;
        [SerializeField] private HUDController hud;
        [SerializeField] private int mutationEnergy;

        private int _zonesCleared;

        private IEnumerator Start()
        {
            var world = GameSession.Instance.selectedWorld ?? GameSession.Instance.catalog.worlds[0];
            hud.SetWorld(world.id);
            foreach (var zone in world.zones)
            {
                yield return RunZone(zone);
                _zonesCleared++;
                mutationUI.ShowChoices();
                while (mutationUI.IsOpen) yield return null;
            }

            EndRun(true);
        }

        private IEnumerator RunZone(ZoneDef zone)
        {
            hud.SetZone(zone.id);
            if (zone.eventDef != null)
            {
                ApplyEvent(zone.eventDef.eventType);
                yield return new WaitForSeconds(2f);
                yield break;
            }

            if (zone.waves != null)
            {
                foreach (var wave in zone.waves)
                {
                    yield return spawner.RunWave(wave);
                    while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0) yield return null;
                }
            }
        }

        private void ApplyEvent(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.MutationChamber:
                case EventType.EvolutionCatalyst:
                    mutationEnergy += 25;
                    break;
                case EventType.HostWeakness:
                    FindObjectOfType<PlayerController>()?.GetComponent<Combat.HealthComponent>().Heal(20f);
                    break;
            }
            hud.SetEvent(eventType.ToString());
        }

        public void AddMutationEnergy(int amount)
        {
            mutationEnergy += amount;
            hud.SetEnergy(mutationEnergy);
        }

        public void EndRun(bool victory)
        {
            var points = _zonesCleared * 2 + (victory ? 5 : 0);
            GameSession.Instance.AddEvolutionPoints(points);
            PlayerPrefs.SetInt("PH_LastRunPoints", points);
            SceneManager.LoadScene("MainMenu");
        }
    }
}
