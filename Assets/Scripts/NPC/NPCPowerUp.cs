using UnityEngine;

public class NPCPowerUpHandler : MonoBehaviour
{
    [Header("Powerup")]
    public bool hasPowerup = false;
    public string PowerIs = "";

    [Header("FireBall Settings")]
    public GameObject ballPrefab;
    public Transform shootPoint;
    public float shootForce = 500f;
    [SerializeField] private float _XRotation = 0f;


    [Header("Box Settings")]
    public GameObject boxPrefab;
    [SerializeField] private Transform _diamondSpawnPoint;


    [Header("Sticky Settings")]
    public GameObject stickyPrefab;
    public Vector3 stickyOffset = new Vector3(0, 0, -2);

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPowerup)
        {
            if (other.CompareTag("FireBall"))
            {
                hasPowerup = true;
                PowerIs = "FireBall";
                Destroy(other.gameObject);
                UsePower(); 
            }
            else if (other.CompareTag("Box"))
            {
                hasPowerup = true;
                PowerIs = "Box";
                Destroy(other.gameObject);
                UsePower();
            }
            else if (other.CompareTag("Sticky"))
            {
                hasPowerup = true;
                PowerIs = "Sticky";
                Destroy(other.gameObject);
                UsePower();
            }
        }
    }

    void UsePower()
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
    }

    void ShootBall()
    {
        if (ballPrefab != null && shootPoint != null)
        {
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
        if (boxPrefab != null && _diamondSpawnPoint != null)
        {

            Vector3 spawnPos = _diamondSpawnPoint.position;

            Instantiate(boxPrefab, spawnPos, _diamondSpawnPoint.rotation);

        }
        else
        {
            Debug.LogWarning("BoxPrefab or _spawnPoint is not assigned!");
        }
    }

    void PlaceSticky()
    {
        if (stickyPrefab != null)
        {
            Vector3 spawnPos = transform.position + stickyOffset;
            Instantiate(stickyPrefab, spawnPos, Quaternion.identity);
        }
    }
}
