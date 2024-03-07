using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.ScriptableObjects.Variables;
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
        [SerializeField]
        private IntVariable _avatarIndexVariable;
        [SerializeField]
        private AvatarModel _avatarModel;
        [SerializeField]
        private AnimatedButton _leftArrowButton;
        [SerializeField]
        private AnimatedButton _rightArrowButton;
        [SerializeField]
        private Image _avatarImage;
        [SerializeField]
        private InputField _playerNameInputField;
        [SerializeField]
        private Slider _volumeSlider;
        [SerializeField]
        private FloatVariable _musicVolumeVariable;
        [SerializeField]
        private Slider _soundFxSlider;
        [SerializeField]
        private FloatVariable _soundFxVolumeVariable;

        private void OnEnable()
        {
            _leftArrowButton.onClick.AddListener(PressLeftArrow);
            _rightArrowButton.onClick.AddListener(PressRightArrow);
            ShowAvatar();
            _playerNameInputField.text = _playerName.Value;
            _volumeSlider.onValueChanged.AddListener(ChangeMusicVolume);
            _volumeSlider.value = _musicVolumeVariable.Value;
            _soundFxSlider.onValueChanged.AddListener(ChangeSoundFxVolume);
            _soundFxSlider.value = _soundFxVolumeVariable.Value;
        }

        private void OnDisable()
        {
            _leftArrowButton.onClick.RemoveAllListeners();
            _rightArrowButton.onClick.RemoveAllListeners();
            _volumeSlider.onValueChanged.RemoveAllListeners();
            _soundFxSlider.onValueChanged.RemoveAllListeners();
        }

        private void ChangeMusicVolume(float newValue)
        {
            _musicVolumeVariable.SetValue(newValue);
        }

        private void ChangeSoundFxVolume(float newValue)
        {
            _soundFxVolumeVariable.SetValue(newValue);
        }

        public void SaveSettings()
        {
            _playerName.SetValue(_playerNameText.text);

            if (TryGetComponent(out Popup popup))
            {
                popup.Close();
            }
        }

        private void PressLeftArrow()
        {
            int nextIndex = _avatarIndexVariable.Value - 1;
            if (nextIndex < 0)
            {
                nextIndex = _avatarModel.Avatars.Length - 1;
            }

            _avatarIndexVariable.SetValue(nextIndex);

            ShowAvatar();
        }

        private void PressRightArrow()
        {
            int nextIndex = _avatarIndexVariable.Value + 1;
            if (nextIndex == _avatarModel.Avatars.Length)
            {
                nextIndex = 0;
            }

            _avatarIndexVariable.SetValue(nextIndex);
            ShowAvatar();
        }

        private void ShowAvatar()
        {
            _avatarImage.sprite = _avatarModel.Avatars[_avatarIndexVariable.Value];
        }
    }
}