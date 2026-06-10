using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawner : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _targetTransform != null)
        {
            // Move player to the target GameObject's position
            other.transform.position = _targetTransform.position;

            // Reset velocity if player has a Rigidbody
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
