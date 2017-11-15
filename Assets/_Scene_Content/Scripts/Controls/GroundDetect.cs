using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetect : MonoBehaviour {

    public CameraShake _camShake;
    public _manager manager;
    public bool _grounded = true;
    public bool _gameOver;

    private void Start()
    {
        manager = GameObject.Find("UI").GetComponent<_manager>();
        _camShake = Camera.main.GetComponent<CameraShake>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground") _grounded = false;
        if (!manager._ending) print(other.name);
    }

    private void OnTriggerStay(Collider other)
    {
        _grounded = (other.tag == "Ground" || other.tag == "Wall"); 
    }

    private void Update()
    {
        if (!_grounded)
        {
            if (!_gameOver)
            {
                _camShake.Shake();
                manager.EndLevel(false);
                _gameOver = true;
            }
        }
    }
}
