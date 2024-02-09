using Ricimi;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ErrorPopupView : MonoBehaviour
    {
        public event Action OnPressedOk;

        [SerializeField]
        private AnimatedButton _okButton;
        [SerializeField]
        private Text _errorText;

        public void SetErrorText(string errorText)
        {
            _errorText.text = errorText;
        }

        private void OnEnable()
        {
            _okButton.onClick.AddListener(() => {
                OnPressedOk?.Invoke();
            });
        }

        private void OnDisable()
        {
            _okButton.onClick.RemoveAllListeners();
        }
    }
}