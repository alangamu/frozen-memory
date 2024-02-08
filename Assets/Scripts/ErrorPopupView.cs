using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ErrorPopupView : MonoBehaviour
    {
        [SerializeField]
        private Text _errorText;

        public void SetErrorText(string errorText)
        {
            _errorText.text = errorText;
        }
    }
}