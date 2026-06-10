using UnityEngine;
using TMPro; 

public class CoinCollector : MonoBehaviour
{
    public int coinCount = 0; 
    public TextMeshProUGUI coinText; 

    [SerializeField] private AudioSource _pickUpCoinSoundEffect;


    void Start()
    {
        UpdateCoinUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin")) 
        {
            _pickUpCoinSoundEffect.Play();
            coinCount++;
            UpdateCoinUI();
            Destroy(other.gameObject); 
        }
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text =  coinCount.ToString();
        }
    }
    public int GetCoins()
    {
        return coinCount; 
    }

}
