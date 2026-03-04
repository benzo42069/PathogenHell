using PathogenHell.Core;
using PathogenHell.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PathogenHell.UI
{
    public class MetaProgressionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pointsText;
        [SerializeField] private Transform root;
        [SerializeField] private Button buttonTemplate;

        private void Start()
        {
            buttonTemplate.gameObject.SetActive(false);
            Refresh();

            foreach (MetaUpgradeDef def in GameSession.Instance.catalog.metaUpgrades)
            {
                var b = Instantiate(buttonTemplate, root);
                b.gameObject.SetActive(true);
                b.GetComponentInChildren<TextMeshProUGUI>().text = $"{def.displayName} ({def.cost})";
                b.onClick.AddListener(() =>
                {
                    GameSession.Instance.TryBuyMetaUpgrade(def);
                    Refresh();
                });
            }
        }

        public void BackToMenu() => SceneManager.LoadScene("MainMenu");

        private void Refresh() => pointsText.text = $"Evolution Points: {GameSession.Instance.evolutionPoints}";
    }
}
