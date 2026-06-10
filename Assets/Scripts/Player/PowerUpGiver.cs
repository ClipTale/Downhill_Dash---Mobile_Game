using UnityEngine;

public class PowerUpGiver : MonoBehaviour
{
    private PowerUpActivation PowerUp;

    void Start()
    {
        PowerUp = GetComponent<PowerUpActivation>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!PowerUp.hasPowerup)
        {
            if (other.CompareTag("FireBall"))
            {
                PowerUp.ActivatePowerUp("FireBall");
                Destroy(other.gameObject);
                
            }
            else if (other.CompareTag("Box"))
            {
                PowerUp.ActivatePowerUp("Box");
                Destroy(other.gameObject);
                
            }
            else if (other.CompareTag("Sticky"))
            {
                PowerUp.ActivatePowerUp("Sticky");
                Destroy(other.gameObject);
                
            }
            
        }
    }
}
