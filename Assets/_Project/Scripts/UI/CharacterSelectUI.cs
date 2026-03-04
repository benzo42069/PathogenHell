using PathogenHell.Core;
using PathogenHell.Data;
using UnityEngine;
using UnityEngine.UI;

namespace PathogenHell.UI
{
    public class CharacterSelectUI : MonoBehaviour
    {
        [SerializeField] private Transform buttonRoot;
        [SerializeField] private Button buttonTemplate;

        private void Start()
        {
            buttonTemplate.gameObject.SetActive(false);
            foreach (PathogenDef def in GameSession.Instance.catalog.pathogens)
            {
                var button = Instantiate(buttonTemplate, buttonRoot);
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = def.displayName;
                button.onClick.AddListener(() => GameSession.Instance.selectedPathogen = def);
            }
        }
    }
}
