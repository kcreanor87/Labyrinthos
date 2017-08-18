using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopSpeedChange : MonoBehaviour {

    public int _playerIndex;
    public CoopControls _playerControls;
    public CameraShake _camShake;
    public _CoopManager _sceneManager;
    public GameObject _boostParticle;

    private void Awake()
    {
        _playerControls = GameObject.Find("pfbCo-opP" + (_playerIndex + 1)).GetComponent<CoopControls>();
        _playerIndex = _playerControls._playerIndex;
        _camShake = GameObject.Find("P" + (_playerIndex + 1) + "cam").GetComponent<CameraShake>();
        _sceneManager = GameObject.Find("UI").GetComponent<_CoopManager>();
        _boostParticle = GameObject.Find("BoostParticles" + (_playerIndex+1));
        _boostParticle.SetActive(false);
        
    }

    public void FixedUpdate()
    {
        if (_sceneManager._inMenu) return;
        if (_sceneManager._ending)
        {
            SlowDown();
            return;
        }
        if (Input.GetAxis("SpeedChange" + _playerIndex) <= -0.05f) SpeedUp();
        else if (Input.GetAxis("SpeedChange" + _playerIndex) >= 0.05f) SlowDown();
        else ReturnToNormal();
        if (_playerControls._boost && !_boostParticle.activeInHierarchy)
        {
            _boostParticle.SetActive(true);
        }
        else if (!_playerControls._boost)
        {
            _boostParticle.SetActive(false);
        }
    }

    public void SpeedUp()
    {
        _playerControls._boost = true;
        if (!_boostParticle.activeInHierarchy && !_sceneManager._ending && !_playerControls._resetting) _camShake.Shake();
        _boostParticle.SetActive(true);
    }

    public void SlowDown()
    {
        _playerControls._brake = true;
        _boostParticle.SetActive(false);
    }

    public void ReturnToNormal()
    {
        _playerControls._boost = false;
        _playerControls._brake = false;
        _boostParticle.SetActive(false);
    }
}
