using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NicknameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _nicknameText;
    [SerializeField] private Button _changeButton;

    private const string NicknameKey = "PlayerNickname";
    private TouchScreenKeyboard _keyboard;

    private void Start()
    {
        LoadNickname();
        _changeButton.onClick.AddListener(OpenKeyboard);
    }

    private void LoadNickname()
    {
        _nicknameText.text = PlayerPrefs.HasKey(NicknameKey) ? PlayerPrefs.GetString(NicknameKey) : "Player";
    }

    private void OpenKeyboard()
    {
        _keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    private void Update()
    {
        if (_keyboard != null && _keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            ApplyNickname(_keyboard.text);
            _keyboard = null; // Reset keyboard reference
        }
    }

    private void ApplyNickname(string newNickname)
    {
        if (!string.IsNullOrEmpty(newNickname))
        {
            PlayerPrefs.SetString(NicknameKey, newNickname);
            PlayerPrefs.Save();
            _nicknameText.text = newNickname;
        }
    }
}
