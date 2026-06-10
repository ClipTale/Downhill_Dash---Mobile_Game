using UnityEngine;

public class CPUTimer : MonoBehaviour
{
    private float _elapsedTime = 0f;
    private bool _raceFinished = false;

    void Update()
    {
        if (!_raceFinished)
        {
            _elapsedTime += Time.deltaTime;
        }
    }

    public float GetElapsedTime()
    {
        return _elapsedTime;
    }


}
