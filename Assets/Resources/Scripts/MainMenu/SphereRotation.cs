using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SphereRotation : MonoBehaviour {

    public Transform _sphere; 
    public float _baseSpeed = 1f;
    public float _speed = 1.5f;
    private int inputX;

    // Use this for initialization
    void Awake () {
        _sphere = GameObject.Find("WorldContainer_parent").GetComponent<Transform>(); 
	}

    public void FixedUpdate()
    {
        var x = CrossPlatformInputManager.GetAxis("Horizontal_Game");
        var y = CrossPlatformInputManager.GetAxis("Vertical_Game");
        if (Mathf.Abs(x) <= 0.12f && Mathf.Abs(y) <= 0.12f) return;  
        x *= _speed;
        y *= _speed;
        _sphere.Rotate(-y, -x, 0, Space.World);
    }   
}
