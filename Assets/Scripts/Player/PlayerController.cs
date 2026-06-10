using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("Movimentação")]
    public float maxSpeed;
    public float acceleration = 5f;
    public float lateralSpeed = 2.5f;
    public float forwardForce = 10f;

    [Header("Boost")]
    public float boostDuration = 2f;   
    public float boostForce = 20f;     
    public float boostBarFillRate = 0.2f;
    public float boostBarMax = 1f;  

    [Header("Braking (Swipe para baixo)")]
    public float swipeDownThreshold = 50f;
    public float maxBrakeAccumulationTime = 1f;
    public float brakeForce = 15f;

    [Header("Camera FOV")]
    public Camera mainCamera;
    public float defaultFOV = 60f;
    public float boostedFOV = 80f;
    public float brakingFOV = 50f;
    public float fovLerpSpeed = 5f;

    [Header("Boost UI")]
    public UnityEngine.UI.Slider boostSlider;

    private Rigidbody rb;
    private float screenCenterX;
    private float moveDirection = 0f;

    private bool isBoosting = false;
    private float boostTimer = 0f;
    private float boostBar = 0f;

    private float lastTapTime = 0f;
    [SerializeField] private bool isGrounded = true;
    private bool isBrakingActive = false;
    private float brakeAccumulation = 0f;
    private Vector2 initialTouchPos;

    [SerializeField] private AudioSource _getHitSoundEffect;
    [SerializeField] private AudioSource _boostSoundEffect;
    [SerializeField] private AudioSource _groundedSoundEffect; 

    private bool isSoundPlaying = false; 
    private bool _isPaused = false;

    [SerializeField] private RawImage _playerRawImage;
    private Color _originalColor;

    [SerializeField] private UnityEngine.UI.Image _boostIndicatorImage;

    void Start()
    {
        if (_playerRawImage != null)
        {
            _originalColor = _playerRawImage.color;
        }

        rb = GetComponent<Rigidbody>();
        screenCenterX = Screen.width / 2;
        rb.useGravity = true;

        if (mainCamera != null)
            mainCamera.fieldOfView = defaultFOV;

        if (boostSlider != null)
        {
            boostSlider.maxValue = boostBarMax;
            boostSlider.value = boostBar;
        }
    }

    void Update()
    {
        HandleInput();
        HandleBoostBar();
        HandleCameraFOV();
        UpdateBoostUI();

        // Handle Pause/Unpause
        if (Time.timeScale == 0)
        {
            if (!_isPaused)
            {
                _groundedSoundEffect.Pause();
                _isPaused = true;
            }
            return; // Prevent further execution when paused
        }

        if (_isPaused)
        {
            // Resume the correct sound when unpausing
            if (isGrounded)
            {
                if (!_groundedSoundEffect.isPlaying)
                    _groundedSoundEffect.UnPause();
            }

            _isPaused = false;
        }

        // Handle sound transitions when grounded or in air
        if (isGrounded)
        {
            if (!_groundedSoundEffect.isPlaying)
            {
                _groundedSoundEffect.loop = true;
                _groundedSoundEffect.Play();
            }

            // Adjust pitch and volume based on speed
            float speedPercentage = rb.velocity.magnitude / maxSpeed;
            _groundedSoundEffect.pitch = Mathf.Lerp(0.8f, 1.5f, speedPercentage); // Adjust pitch range as needed
            _groundedSoundEffect.volume = Mathf.Lerp(0.003f, 0.015f, speedPercentage); // Adjust volume range as needed
        }
        else
        {
            if (_groundedSoundEffect.isPlaying)
                _groundedSoundEffect.Stop();
        }
    }

    void FixedUpdate()
    {
        // Impulsiona o objeto para frente
        if (!isBoosting && rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.forward * forwardForce, ForceMode.Acceleration);
        }
        else if (isBoosting)
        {
            float currentBoostForce = boostForce * (boostTimer / boostDuration);
            rb.AddForce(transform.forward * currentBoostForce, ForceMode.Acceleration);
        }

        // Movimento lateral
        rb.AddForce(Vector3.right * moveDirection * lateralSpeed, ForceMode.Acceleration);

        // Aplica a frenagem apenas se o objeto estiver se movendo para frente
        if (isBrakingActive)
        {
            float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
            if (forwardSpeed > 0)
            {
                float brakeMultiplier = Mathf.Clamp01(brakeAccumulation / maxBrakeAccumulationTime);
                // Calcula a desaceleração para este FixedUpdate
                float deceleration = brakeForce * brakeMultiplier * Time.fixedDeltaTime;
                
                // Se a desaceleração ultrapassar a velocidade atual, zera a velocidade forward
                if(forwardSpeed - deceleration <= 0)
                {
                    Vector3 lateralAndVertical = rb.velocity - transform.forward * forwardSpeed;
                    rb.velocity = lateralAndVertical;
                }
                else
                {
                    rb.AddForce(-transform.forward * brakeForce * brakeMultiplier, ForceMode.Acceleration);
                }
            }
            else
            {
                // Se já estiver parado ou indo para trás, reseta a frenagem
                isBrakingActive = false;
                brakeAccumulation = 0f;
            }
        }
    }

    void HandleInput()
    {
        bool inputActive = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputActive = true;

            if (touch.phase == TouchPhase.Began)
            {
                initialTouchPos = touch.position;

                if (Time.time - lastTapTime < 0.3f)
                {
                    if (boostBar >= boostBarMax && !isBoosting)
                    {
                        ActivateBoost();
                    }
                }
                lastTapTime = Time.time;
            }

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if (touch.position.x < screenCenterX - 50)
                    moveDirection = -1f;
                else if (touch.position.x > screenCenterX + 50)
                    moveDirection = 1f;
                else
                    moveDirection = 0f;

                float deltaY = initialTouchPos.y - touch.position.y;
                float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
                if (deltaY > swipeDownThreshold && forwardSpeed > 0)
                {
                    isBrakingActive = true;
                    brakeAccumulation += Time.deltaTime;
                    brakeAccumulation = Mathf.Clamp(brakeAccumulation, 0f, maxBrakeAccumulationTime);
                }
                else
                {
                    isBrakingActive = false;
                    brakeAccumulation = 0f;
                }
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isBrakingActive = false;
                brakeAccumulation = 0f;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                inputActive = true;
                initialTouchPos = Input.mousePosition;

                if (Time.time - lastTapTime < 0.3f)
                {
                    if (boostBar >= boostBarMax && !isBoosting)
                    {
                        ActivateBoost();
                    }
                }
                lastTapTime = Time.time;
            }

            if (Input.GetMouseButton(0))
            {
                inputActive = true;
                Vector3 mousePos = Input.mousePosition;

                if (mousePos.x < screenCenterX - 50)
                    moveDirection = -1f;
                else if (mousePos.x > screenCenterX + 50)
                    moveDirection = 1f;
                else
                    moveDirection = 0f;

                float deltaY = initialTouchPos.y - mousePos.y;
                float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
                if (deltaY > swipeDownThreshold && forwardSpeed > 0)
                {
                    isBrakingActive = true;
                    brakeAccumulation += Time.deltaTime;
                    brakeAccumulation = Mathf.Clamp(brakeAccumulation, 0f, maxBrakeAccumulationTime);
                }
                else
                {
                    isBrakingActive = false;
                    brakeAccumulation = 0f;
                }
            }
            else
            {
                moveDirection = 0f;
                if (!inputActive)
                {
                    isBrakingActive = false;
                    brakeAccumulation = 0f;
                }
            }
        }
    }

    void ActivateBoost()
    {
        if (Time.timeScale == 1)
        {
            _boostSoundEffect.Play();
        isBoosting = true;
        boostTimer = boostDuration;
        Debug.Log("Boost ativado!");
        }
    }

    void HandleBoostBar()
    {
        if (isBoosting)
        {
            boostBar -= (boostBarMax / boostDuration) * Time.deltaTime;
            boostBar = Mathf.Clamp(boostBar, 0f, boostBarMax);

            if (boostBar <= 0f)
            {
                isBoosting = false;
                Debug.Log("Boost finalizado.");
            }
        }
        else
        {
            if (!isGrounded)
            {
                boostBar += boostBarFillRate * Time.deltaTime;
                boostBar = Mathf.Clamp(boostBar, 0f, boostBarMax);
            }
        }
    }

    void UpdateBoostUI()
    {
        if (boostSlider != null)
        {
            boostSlider.value = boostBar;
        }

        // Change the boost indicator image color when boost is full
        if (_boostIndicatorImage != null)
        {
            if (boostBar >= boostBarMax)
            {
                _boostIndicatorImage.color = Color.yellow;
            }
            else
            {
                _boostIndicatorImage.color = new Color32(165, 170, 108, 255); // #A5AA6C
            }
        }
    }

    void HandleCameraFOV()
    {
        if (mainCamera == null)
            return;

        float targetFOV = defaultFOV;

        if (isBrakingActive)
            targetFOV = brakingFOV;
        else if (isBoosting)
            targetFOV = boostedFOV;

        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Player is grounded!");

        }

    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Player left the ground!");

        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {


            _getHitSoundEffect.Play();
            // Reseta a velocidade do jogador
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Destroi o obstáculo
            Destroy(other.gameObject);

            StartCoroutine(BlinkRed());
        }
    }

    private IEnumerator BlinkRed()
    {
        if (_playerRawImage != null)
        {
            // Change to red
            _playerRawImage.color = Color.red;

            // Wait for 0.1 seconds
            yield return new WaitForSeconds(0.1f);

            // Revert to the original color
            _playerRawImage.color = _originalColor;
        }
    }

}