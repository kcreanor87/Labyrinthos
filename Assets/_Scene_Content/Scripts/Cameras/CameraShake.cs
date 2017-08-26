using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public bool Shaking;
    private float ShakeDecay;
    public float ShakeIntensity;
    private Vector3 OriginalPos;
    private Quaternion OriginalRot;

    public Transform _transform;

    public bool _singlePlayer;

    void Start()
    {
        Shaking = false;
        if (_singlePlayer) _transform = Camera.main.transform;
        else _transform = transform;
    }
    
    void FixedUpdate()
    {
        if (ShakeIntensity > 0)
        {
            _transform.localPosition = OriginalPos + Random.insideUnitSphere * ShakeIntensity;
            _transform.localRotation = new Quaternion(OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
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
        OriginalPos = _transform.localPosition;
        OriginalRot = _transform.localRotation;
        if (_singlePlayer)
        {
            ShakeIntensity = 0.3f;
            ShakeDecay = 0.015f;
            Shaking = true;
        }
        else
        {
            ShakeIntensity = 0.02f;
            ShakeDecay = 0.001f;
            Shaking = true;
        }
        
    }
}
