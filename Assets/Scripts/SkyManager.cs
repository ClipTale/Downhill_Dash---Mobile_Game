using UnityEngine;

public class SkyManager : MonoBehaviour
{
    public float _skySpeed;


    void Update()
    {

        RenderSettings.skybox.SetFloat("_Rotation", Time.time * _skySpeed);
    }
}
