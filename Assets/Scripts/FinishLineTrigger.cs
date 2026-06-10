using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FinishLineTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _scoreboardCanvas;
    [SerializeField] private TMP_Text _finalTimeText;
    [SerializeField] private TMP_Text _cpuTimeText;
    [SerializeField] private TMP_Text _winnerText;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_Text _bestTimeText;
    [SerializeField] private TMP_Text _playerPositionText;
    [SerializeField] private TMP_Text _cpuPositionText;
    [SerializeField] private TMP_Text _playerCoinsText; 
    [SerializeField] private TMP_Text _cpuCoinsText;    
    [SerializeField] private RawImage _winnerImage;
    [SerializeField] private Texture2D _cpuImage;

    [SerializeField] private PauseMenu _pauseMenu;
    [SerializeField] private CPUTimer _cpuController;
    [SerializeField] private CoinCollector _playerCoinCollector; 
    [SerializeField] private NPCRacing _cpuCoinCollector; 

    private const string NicknameKey = "PlayerNickname";
    private const string ImagePathKey = "CapturedImagePath";
    private const string BestTimeKey = "BestTime";
    private string _mapName;

    private float _playerFinishTime = -1f;
    private float _cpuFinishTime = -1f;
    private bool _playerFinished = false;
    private bool _cpuFinished = false;

    public GameObject _pauseButton;

    [SerializeField] private AudioSource _finishGameSoundEffect;

    private void Start()
    {
        _scoreboardCanvas.SetActive(false);
        _mapName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name; // Get current map name
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_playerFinished)
        {
            _playerFinishTime = _pauseMenu.GetElapsedTime();
            _playerFinished = true;
            ShowScoreboard();
        }
        else if (other.CompareTag("NPC") && !_cpuFinished)
        {
            _cpuFinishTime = _cpuController.GetElapsedTime();
            _cpuFinished = true;
        }
    }

    private void ShowScoreboard()
    {
        _finishGameSoundEffect.Play();
        Time.timeScale = 0;
        _scoreboardCanvas.SetActive(true);
        _pauseButton.SetActive(false);

        // Load player's nickname
        string playerName = PlayerPrefs.GetString(NicknameKey, "Player");
        _playerNameText.text = playerName;

        // Display player’s final time
        DisplayTime(_playerFinishTime, _finalTimeText);

        // Display CPU’s final time (if available)
        if (_cpuFinished)
        {
            DisplayTime(_cpuFinishTime, _cpuTimeText);
        }
        else
        {
            _cpuTimeText.text = "-:--"; // CPU hasn't finished yet
        }

        // Retrieve and display coin counts 
        _playerCoinsText.text = _playerCoinCollector.GetCoins().ToString();
        _cpuCoinsText.text = _cpuCoinCollector.GetCoins().ToString();

        // Determine winner based on who has the lower finish time
        if (_cpuFinished && _cpuFinishTime < _playerFinishTime)
        {
            _winnerText.text = "CPU";
            _playerPositionText.text = "2";
            _cpuPositionText.text = "1";
            _winnerImage.texture = _cpuImage; // Show CPU's image
        }
        else
        {
            _winnerText.text = playerName;
            _playerPositionText.text = "1";
            _cpuPositionText.text = "2";
            LoadWinnerImage(); // Show player's image
        }

        // Save best time
        CheckAndSaveBestTime(_playerFinishTime);

        // Display best time
        DisplayBestTime();
    }

    private void DisplayTime(float time, TMP_Text text)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        text.text = $"{minutes}:{seconds:D2}";
    }

    private void CheckAndSaveBestTime(float playerTime)
    {
        string bestTimeKey = $"{BestTimeKey}_{_mapName}"; // Unique key per map

        if (!PlayerPrefs.HasKey(bestTimeKey) || playerTime < PlayerPrefs.GetFloat(bestTimeKey))
        {
            PlayerPrefs.SetFloat(bestTimeKey, playerTime);
            PlayerPrefs.Save();
        }
    }

    private void DisplayBestTime()
    {
        string bestTimeKey = $"{BestTimeKey}_{_mapName}";

        if (PlayerPrefs.HasKey(bestTimeKey))
        {
            float bestTime = PlayerPrefs.GetFloat(bestTimeKey);
            DisplayTime(bestTime, _bestTimeText);
        }
        else
        {
            _bestTimeText.text = "-:--";
        }
    }

    private void LoadWinnerImage()
    {
        if (PlayerPrefs.HasKey(ImagePathKey))
        {
            string imagePath = PlayerPrefs.GetString(ImagePathKey);
            if (System.IO.File.Exists(imagePath))
            {
                byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);
                _winnerImage.texture = texture;
            }
        }
    }
}
