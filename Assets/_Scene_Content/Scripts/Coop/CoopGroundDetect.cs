using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopGroundDetect : MonoBehaviour {

    public CameraShake _camShake;
    public CoopControls _controlManager;
    public _CoopManager _manager;
    public bool _grounded = true;
    public bool _gameOver;

    public int _playerIndex;

    private void Start()
    {
        _controlManager = GameObject.Find("pfbCo-opP" + (_playerIndex + 1)).GetComponent<CoopControls>();
        _manager = GameObject.Find("UI").GetComponent<_CoopManager>();
        _camShake = _controlManager._camShake;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground" && !_manager._ending) _grounded = false;
    }

    private void OnTriggerStay(Collider other)
    {
        _grounded = (other.tag == "Ground");
    }

    private void Update()
    {
        if (!_grounded)
        {
            if (!_gameOver && !_manager._ending)
            {
                _camShake.Shake();
                _controlManager.ReturnToStart();
                _grounded = true;
            }
            else
            {
                _grounded = true;
            }
        }
    }
}
