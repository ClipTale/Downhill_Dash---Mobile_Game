using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject pauseButton;
    public GameObject firstPictureToTake;
    public GameObject objectToDisable; 
    public TextMeshProUGUI countdownText;
    private bool isPaused = false;
    private Coroutine countdownCoroutine; 
    public bool _PauseGameAtStart = true;

    private float _elapsedTime = 0f;
    public TextMeshProUGUI timerText;

    [SerializeField] private AudioSource _countdownBeep;
    [SerializeField] private AudioSource _countdownEndSound;


    void Start()
    {
        if (_PauseGameAtStart)
        {
            Time.timeScale = 0; // Pause the game at the start ffor countdown
        }
        pauseMenuUI.SetActive(false);
        countdownText.gameObject.SetActive(false);
        pauseButton.SetActive(true); // Ensure the pause button is visible at the start
        countdownCoroutine = StartCoroutine(GameStartCountdown()); // Start countdown

        StartCoroutine(GameTimer());
    }
    private IEnumerator GameTimer()
    {
        while (true)
        {
            if (Time.timeScale > 0)
            {
                _elapsedTime += Time.deltaTime;

                // Calculate minutes and seconds
                int minutes = Mathf.FloorToInt(_elapsedTime / 60);
                int seconds = Mathf.FloorToInt(_elapsedTime % 60);

                // Update timer display in "M:SS" format
                timerText.text = $"{minutes}:{seconds:D2}"; // D2 ensures two-digit seconds
            }
            yield return null;
        }
    }
    public float GetElapsedTime()
    {
        return _elapsedTime;
    }

    private IEnumerator GameStartCountdown()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            _countdownBeep.Play();

            yield return new WaitForSecondsRealtime(1f); // Countdown runs even when the game is paused
        }
        _countdownEndSound.Play();
        countdownText.gameObject.SetActive(false);
        pauseButton.SetActive(true);

        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false); // Disable the specified GameObject
        }

        Time.timeScale = 1; // Resume the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Ensure time is running before restarting
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reloads the current scene
    }

    public void OpenSettingsMenu()
    {
        firstPictureToTake.SetActive(true);
        pauseMenuUI.SetActive(false);
    }

    public void CloseSettingsMenu()
    {
        firstPictureToTake.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void TogglePause()
    {
        if (!isPaused)
        {
            Time.timeScale = 0;
            pauseMenuUI.SetActive(true);
            //pauseButton.SetActive(false);

            if (countdownCoroutine != null) // Stop countdown if running
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
            }

            isPaused = true;
        }
        else
        {
            pauseMenuUI.SetActive(false);
            pauseButton.SetActive(true);
            firstPictureToTake.SetActive(false);
            isPaused = false;

            countdownCoroutine = StartCoroutine(GameStartCountdown()); // Restart countdown when unpaused
        }
    }

    public void ResumeGame()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        pauseMenuUI.SetActive(false);
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.gameObject.SetActive(false);
        firstPictureToTake.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1;
        isPaused = false;
    }
}
