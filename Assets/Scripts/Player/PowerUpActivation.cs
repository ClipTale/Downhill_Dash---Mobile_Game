using UnityEngine;

public class PowerUpActivation : MonoBehaviour
{
    [Header("Powerup UI Indicators")]
    [SerializeField] private GameObject _fireBallIndicator; 
    [SerializeField] private GameObject _boxIndicator; 
    [SerializeField] private GameObject _stickyIndicator; 

    [Header("Powerup")]
    public bool hasPowerup = false;
    public string PowerIs = "";

    [Header("FireBall Settings")]
    public GameObject ballPrefab;
    public Transform shootPoint;
    public float shootForce = 500f;
    [SerializeField] private float _XRotation = 0f;

    [Header("Box Settings")]
    public GameObject _diamondPrefab;
    [SerializeField] private Transform _diamondAndOilSpawnPoint;

    [Header("Sticky Settings")]
    public GameObject stickyPrefab;

    [Header("Giroscópio Settings")]
    public float gyroThreshold = 10f;

    private bool actionTriggered = false;
    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f;

    [SerializeField] private AudioSource _pickUpPowerUpSoundEffect;
    [SerializeField] private AudioSource _diamondPowerUpSoundEffect;
    [SerializeField] private AudioSource _oilPowerUpSoundEffect;
    [SerializeField] private AudioSource _shockPowerUpSoundEffect;

    void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }

        // Hide all power-up indicators at start
        if (_fireBallIndicator) _fireBallIndicator.SetActive(false);
        if (_boxIndicator) _boxIndicator.SetActive(false);
        if (_stickyIndicator) _stickyIndicator.SetActive(false);
    }

    void Update()
    {
        if (!hasPowerup)
        {
            return;
        }

        float gyroX = Input.gyro.rotationRateUnbiased.x;

        if (gyroX < -gyroThreshold && !actionTriggered)
        {
            UsePower();
            actionTriggered = true;
        }

        if (gyroX > -gyroThreshold * 0.5f)
        {
            actionTriggered = false;
        }

        //// Detecta duplo clique do mouse
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (Time.time - lastClickTime < doubleClickThreshold)
        //    {
        //        UsePower();
        //    }
        //    lastClickTime = Time.time;
        //}
    }

    public void ActivatePowerUp(string powerType)
    {
        _pickUpPowerUpSoundEffect.Play();
        hasPowerup = true;
        PowerIs = powerType;

        // Hide all indicators first
        if (_fireBallIndicator) _fireBallIndicator.SetActive(false);
        if (_boxIndicator) _boxIndicator.SetActive(false);
        if (_stickyIndicator) _stickyIndicator.SetActive(false);

        // Show the correct power-up indicator
        if (PowerIs == "FireBall" && _fireBallIndicator) _fireBallIndicator.SetActive(true);
        if (PowerIs == "Box" && _boxIndicator) _boxIndicator.SetActive(true);
        if (PowerIs == "Sticky" && _stickyIndicator) _stickyIndicator.SetActive(true);
    }

    void UsePower()
    {
        if (Time.timeScale == 1)
        {
            if (PowerIs == "FireBall")
            {
                ShootBall();
            }
            else if (PowerIs == "Box")
            {
                PlaceBox();
            }
            else if (PowerIs == "Sticky")
            {
                PlaceSticky();
            }

            hasPowerup = false;
            PowerIs = "";

            // Hide all indicators after using the power-up
            if (_fireBallIndicator) _fireBallIndicator.SetActive(false);
            if (_boxIndicator) _boxIndicator.SetActive(false);
            if (_stickyIndicator) _stickyIndicator.SetActive(false);
        }
    }

    void ShootBall()
    {
        if (ballPrefab != null && shootPoint != null)
        {
            _shockPowerUpSoundEffect.Play();
            Quaternion customRotation = Quaternion.Euler(_XRotation, shootPoint.rotation.eulerAngles.y, shootPoint.rotation.eulerAngles.z);

            GameObject ball = Instantiate(ballPrefab, shootPoint.position, customRotation);
            Destroy(ball, 3f);
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(shootPoint.forward * shootForce);
            }
        }
    }

    void PlaceBox()
    {
        if (_diamondPrefab != null && _diamondAndOilSpawnPoint != null)
        {
            _diamondPowerUpSoundEffect.Play();

            Vector3 spawnPos = _diamondAndOilSpawnPoint.position;

            Instantiate(_diamondPrefab, spawnPos, _diamondAndOilSpawnPoint.rotation);

        }
        else
        {
            Debug.LogWarning("BoxPrefab or _spawnPoint is not assigned!");
        }
    }

    void PlaceSticky()
    {
        if (stickyPrefab != null && _diamondAndOilSpawnPoint != null)
        {
            _oilPowerUpSoundEffect.Play();

            Vector3 spawnPos = _diamondAndOilSpawnPoint.position;

            Instantiate(stickyPrefab, spawnPos, _diamondAndOilSpawnPoint.rotation);

        }
        else
        {
            Debug.LogWarning("StickyPrefab or _stickySpawnPoint is not assigned!");
        }
    }

}
