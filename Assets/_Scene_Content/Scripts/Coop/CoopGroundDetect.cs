using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopGroundDetect : MonoBehaviour {

    public CameraShake _camShake;
    public CoopControls manager;
    public bool _grounded = true;
    public bool _gameOver;

    public int _playerIndex;

    private void Start()
    {
        manager = GameObject.Find("pfbCo-opP" + (_playerIndex + 1)).GetComponent<CoopControls>();
        _camShake = manager._camShake;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground" || other.tag == "Wall") _grounded = false;
    }

    private void OnTriggerStay(Collider other)
    {
        _grounded = (other.tag == "Ground" || other.tag == "Wall");
    }

    private void Update()
    {
        if (!_grounded)
        {
            if (!_gameOver && !manager._destroyed)
            {
                _camShake.Shake();
                manager.ReturnToStart();
                _grounded = true;
            }
            else
            {
                _grounded = true;
            }
        }
    }
}
