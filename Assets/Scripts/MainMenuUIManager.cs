using Assets.Scripts.ScriptableObjects.Variables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class MainMenuUIManager : MonoBehaviour
    {
        [SerializeField]
        private string _startupSceneName = "Startup";
        [SerializeField]
        private BoolVariable _isStartupLoaded;

        private void Start()
        {
            if (!_isStartupLoaded.Value)
            {
                LoadScene(_startupSceneName);
            }
        }

        private void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }
}