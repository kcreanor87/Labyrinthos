using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpeed : MonoBehaviour {

    public PlayerControls _playerControls;

    private void Start()
    {
        _playerControls = GameObject.Find("Player").GetComponent<PlayerControls>();
    }

    public void SpeedUp()
    {
        _playerControls._boost = true;
    }

    public void SlowDown()
    {
        _playerControls._brake = true;
    }

    public void ReturnToNormal()
    {
        _playerControls._boost = false;
        _playerControls._brake = false;
    }
}
