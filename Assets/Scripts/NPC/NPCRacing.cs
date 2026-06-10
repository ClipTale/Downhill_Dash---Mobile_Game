using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCRacing : MonoBehaviour
{
    public Transform[] waypoints; 
    public float speed = 10f; 
    public float acceleration = 5f; 
    public float maxSpeed = 30f;
    public float turnSpeed = 5f; 

    public int coinCount;

    private int currentWaypointIndex = 0;
    private Rigidbody rb;

    [SerializeField] private RawImage _npcRawImage;
    private Color _originalColor;

    private bool isStunned = false; 


    void Start()
    {
        // Save the original color of the RawImage
        if (_npcRawImage != null)
        {
            _originalColor = _npcRawImage.color;
        }

        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; // Mantém a gravidade ativa
    }

    void FixedUpdate()
    {
        if (waypoints.Length == 0) return;

        MoveToWaypoint();
        
    }

    void MoveToWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Direção para o próximo waypoint
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        Vector3 desiredVelocity = direction * speed;

        // Aplica aceleração gradual
        rb.velocity = Vector3.Lerp(rb.velocity, desiredVelocity, Time.deltaTime * acceleration);

        // Limita a velocidade máxima
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        // Suaviza a rotação para olhar para o próximo ponto
        if (rb.velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }

        // Se chegou perto do waypoint, passa para o próximo
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 2f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0; // Volta ao primeiro waypoint
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            StartCoroutine(StunNPC(2f, 0.1f)); // Stun por 2 segundos, verificando a cada 0.1s

            // Zera a velocidade do jogador
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Destroi o obstáculo
            Destroy(other.gameObject);
            StartCoroutine(BlinkRed());
        }
        else if (other.CompareTag("Coin")) 
        {
            coinCount++;
            Destroy(other.gameObject); 
        }
    }

    IEnumerator StunNPC(float duration, float resetInterval)
    {
        isStunned = true;
        float timeStunned = 0f;

        while (timeStunned < duration)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            timeStunned += resetInterval;
            yield return new WaitForSeconds(resetInterval);
        }

        isStunned = false;
    }

    public int GetCoins()
    {
        return coinCount; 
    }
    private IEnumerator BlinkRed()
    {
        if (_npcRawImage != null)
        {
            _npcRawImage.color = Color.red;

            yield return new WaitForSeconds(0.1f);

            _npcRawImage.color = _originalColor;
        }
    }
}
