using Assets.Scripts.ScriptableObjects.Variables;
using Ricimi;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class ErrorPopupController : NetworkBehaviour
    {
        [SerializeField]
        private ErrorPopupView _errorPopupView;
        [SerializeField]
        private StringVariable _errorMessageVariable;
        [SerializeField]
        private SceneTransition _sceneTransition;

        private void OnEnable()
        {
            _errorPopupView.gameObject.SetActive(false);
            _errorMessageVariable.OnValueChanged += OnErrorOccurred;
            _errorPopupView.OnPressedOk += OnPressedOk;
        }

        private void OnDisable()
        {
            _errorMessageVariable.OnValueChanged -= OnErrorOccurred;
            _errorPopupView.OnPressedOk -= OnPressedOk;
        }

        private void OnPressedOk()
        {
            _errorPopupView.gameObject.SetActive(false);
            NetworkManager.Singleton.Shutdown();
            if (_sceneTransition != null)
            {
                _sceneTransition.PerformTransition();
            }
        }

        private void OnErrorOccurred(string errorMessage)
        {
            _errorPopupView.gameObject.SetActive(true);
            _errorPopupView.SetErrorText(errorMessage);
        }
    }
}