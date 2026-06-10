using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Configuração")]
    public GameObject[] powerUpPrefabs; 
    public int powerUpCount = 100; 
    public GameObject groundObject; 
    public LayerMask groundLayer; 
    public float spawnHeightOffset = 0.5f; 
    public Vector3 spawnBoundsOffset = new Vector3(2f, 0f, 2f); 
    public float _powerUpXRotation = 25f;
    private Bounds groundBounds;

    void Start()
    {
        if (groundObject == null)
        {
            Debug.LogError("O objeto do chão não foi atribuído no Spawner!");
            return;
        }

        // Obtém os limites do chão
        Collider groundCollider = groundObject.GetComponent<Collider>();
        if (groundCollider != null)
        {
            groundBounds = groundCollider.bounds;
        }
        else
        {
            Debug.LogError("O objeto do chão precisa ter um Collider (BoxCollider ou MeshCollider)!");
            return;
        }

        SpawnPowerUps();
    }

    void SpawnPowerUps()
    {
        for (int i = 0; i < powerUpCount; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();
            if (spawnPosition != Vector3.zero)
            {
                GameObject powerUpPrefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
                Instantiate(powerUpPrefab, spawnPosition, Quaternion.Euler(_powerUpXRotation, 0, 0));
            }
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        for (int attempt = 0; attempt < 10; attempt++) // Tenta spawnar no máximo 10 vezes
        {
            float x = Random.Range(groundBounds.min.x + spawnBoundsOffset.x, groundBounds.max.x - spawnBoundsOffset.x);
            float z = Random.Range(groundBounds.min.z + spawnBoundsOffset.z, groundBounds.max.z - spawnBoundsOffset.z);
            Vector3 rayOrigin = new Vector3(x, groundBounds.max.y + 10f, z); // Raycast de cima para baixo

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                return hit.point + Vector3.up * spawnHeightOffset; // Ajusta a altura
            }
        }
        return Vector3.zero; // Se não encontrar uma posição válida
    }
}
