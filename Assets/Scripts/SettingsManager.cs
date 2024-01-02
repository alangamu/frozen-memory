using Ricimi;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField]
        private Text _playerNameText;
        [SerializeField]
        private StringVariable _playerName;

        public void SaveSettings()
        {
            _playerName.SetValue(_playerNameText.text);

            if (TryGetComponent(out Popup popup))
            {
                popup.Close();
            }
        }
    }
}