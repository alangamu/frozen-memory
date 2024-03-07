using Assets.Scripts.ScriptableObjects.Events;
using Assets.Scripts.ScriptableObjects.Variables;
using UnityEngine;

namespace Assets.Scripts
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private FloatVariable _musicVolumeVariable;
        [SerializeField]
        private FloatVariable _soundFxVolumeVariable;
        [SerializeField]
        private AudioSource _musicAudioSource;
        [SerializeField]
        private AudioSource _soundFxAudioSource;
        [SerializeField]
        private AudioClipGameEvent _playAudioGameEvent;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _musicVolumeVariable.OnValueChanged += OnMusicVolumeChanged;
            _soundFxVolumeVariable.OnValueChanged += OnSoundFxVolumeChanged;
            _musicAudioSource.volume = _musicVolumeVariable.Value;
            _soundFxAudioSource.volume = _soundFxVolumeVariable.Value;
            _playAudioGameEvent.OnRaise += PlayAudioGameEvent;
        }

        private void OnDisable()
        {
            _musicVolumeVariable.OnValueChanged -= OnMusicVolumeChanged;
            _soundFxVolumeVariable.OnValueChanged -= OnSoundFxVolumeChanged;
            _playAudioGameEvent.OnRaise -= PlayAudioGameEvent;
        }

        private void PlayAudioGameEvent(AudioClip audioClip)
        {
            _soundFxAudioSource.clip = audioClip;
            _soundFxAudioSource.Play();
        }

        private void OnSoundFxVolumeChanged(float volumeValue)
        {
            _soundFxAudioSource.volume = volumeValue;
        }

        private void OnMusicVolumeChanged(float volumeValue)
        {
            _musicAudioSource.volume = volumeValue;
        }
    }
}