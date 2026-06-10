using UnityEngine;
using UnityEngine.UI;

public class MusicToggle : MonoBehaviour
{
    [SerializeField] private Toggle _musicToggle;
    [SerializeField] private AudioSource _audioSource;

    private const string MusicPrefKey = "MusicEnabled";

    private void Start()
    {
        bool isMusicOn = PlayerPrefs.GetInt(MusicPrefKey, 1) == 1;
        _musicToggle.isOn = isMusicOn;

        // Apply the saved state to the AudioSource
        if (isMusicOn)
        {
            _audioSource.Play();
        }
        else
        {
            _audioSource.Stop();
        }

        _musicToggle.onValueChanged.AddListener(ToggleMusic);
    }

    private void ToggleMusic(bool isOn)
    {
        if (isOn)
        {
            _audioSource.Play();
        }
        else
        {
            _audioSource.Stop();
        }

        // Save the new state to PlayerPrefs
        PlayerPrefs.SetInt(MusicPrefKey, isOn ? 1 : 0);
        PlayerPrefs.Save(); // Ensure it's written immediately
    }

    private void OnDestroy()
    {
        // Remove listener to avoid memory leaks
        _musicToggle.onValueChanged.RemoveListener(ToggleMusic);
    }
}
