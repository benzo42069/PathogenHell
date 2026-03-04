using PathogenHell.Roguelite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PathogenHell.UI
{
    public class MutationSelectionUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button[] choiceButtons;
        [SerializeField] private TextMeshProUGUI[] labels;
        private MutationManager _mutationManager;
        public bool IsOpen => panel.activeSelf;

        private void Start()
        {
            _mutationManager = FindFirstObjectByType<MutationManager>();
            panel.SetActive(false);
        }

        public void ShowChoices()
        {
            var choices = _mutationManager.RollChoices(3);
            for (var i = 0; i < 3; i++)
            {
                var index = i;
                labels[i].text = $"{choices[i].displayName}\n{choices[i].description}";
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() =>
                {
                    _mutationManager.ApplyMutation(choices[index]);
                    panel.SetActive(false);
                    Time.timeScale = 1f;
                });
            }

            Time.timeScale = 0f;
            panel.SetActive(true);
        }
    }
}
