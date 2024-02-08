using UnityEngine;

namespace Assets.Scripts
{
    public class ErrorPopupController : MonoBehaviour
    {
        [SerializeField]
        private ErrorPopupView _errorPopupView;
        [SerializeField]
        private StringVariable _errorMessageVariable;

        private void OnEnable()
        {
            _errorPopupView.gameObject.SetActive(false);
            _errorMessageVariable.OnValueChanged += OnErrorOccurred;
        }

        private void OnDisable()
        {
            _errorMessageVariable.OnValueChanged += OnErrorOccurred;
        }

        private void OnErrorOccurred(string errorMessage)
        {
            _errorPopupView.gameObject.SetActive(true);
            _errorPopupView.SetErrorText(errorMessage);
        }
    }
}