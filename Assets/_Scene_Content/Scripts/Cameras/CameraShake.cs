using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    //Controls the camera shake when colliding with an object or beginning a boost
    //Shake intensity controls the amount the camera shifts, decay controls how long it lasts
    //Overwritten for multiplayer for now

    private bool Shaking;
    public bool _singlePlayer;

    public float ShakeDecay;
    public float ShakeIntensity;

    private Vector3 OriginalPos;
    private Quaternion OriginalRot;       
    
    void FixedUpdate()
    {
        if (ShakeIntensity > 0)
        {
            transform.localPosition = OriginalPos + Random.insideUnitSphere * ShakeIntensity;
            transform.localRotation = new Quaternion(OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
            OriginalRot.y + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
            OriginalRot.z + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
            OriginalRot.w + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f);

            ShakeIntensity -= ShakeDecay;
        }
        else if (Shaking)
        {
            Shaking = false;
        }
    }

    public void Shake()
    {
        OriginalPos = transform.localPosition;
        OriginalRot = transform.localRotation;

        if (_singlePlayer)
        {
            ShakeIntensity = 0.03f;
            ShakeDecay = 0.0015f;
            Shaking = true;
            return;
        }
        ShakeIntensity = 0.02f;
        ShakeDecay = 0.001f;
        Shaking = true;
    }
}
