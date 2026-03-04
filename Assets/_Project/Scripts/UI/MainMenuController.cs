using PathogenHell.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PathogenHell.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public void StartRun() => SceneManager.LoadScene("RunScene_DemoWorld1");
        public void OpenMeta() => SceneManager.LoadScene("MetaProgression");
        public void Quit() => Application.Quit();

        public void EnsureDefaults()
        {
            var gs = GameSession.Instance;
            if (gs.selectedPathogen == null) gs.selectedPathogen = gs.catalog.pathogens[0];
            if (gs.selectedWorld == null) gs.selectedWorld = gs.catalog.worlds[0];
        }
    }
}
