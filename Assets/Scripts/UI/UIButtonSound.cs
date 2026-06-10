using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private float _volume = 0.5f; 

    private static AudioSource _audioSource; 

    private void Awake()
    {
        if (_audioSource == null)
        {
            GameObject soundObject = new GameObject("UIButtonSoundPlayer");
            _audioSource = soundObject.AddComponent<AudioSource>();
            DontDestroyOnLoad(soundObject); // Keeps it between scenes
        }

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        if (_clickSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_clickSound, _volume);
        }
    }
}
