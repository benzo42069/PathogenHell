using UnityEngine;
using UnityEngine.SceneManagement;

namespace PathogenHell.Core
{
    public class BootLoader : MonoBehaviour
    {
        [SerializeField] private string mainMenuScene = "MainMenu";

        private void Start()
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}
