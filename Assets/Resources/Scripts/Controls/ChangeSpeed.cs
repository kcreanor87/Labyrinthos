using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpeed : MonoBehaviour {

    public PlayerControls _playerControls;
    public CameraShake _camShake;
    public _manager _sceneManager;
    public GameObject _boostParticle;
    public GameObject _regParticle;

    private void Awake()
    {
        _sceneManager = GameObject.Find("UI").GetComponent<_manager>();
        _camShake = Camera.main.GetComponent<CameraShake>();
        _boostParticle = GameObject.Find("BoostParticles");
        _regParticle = GameObject.Find("Jet");
        _playerControls = GameObject.Find("pfbPlayer001").GetComponent<PlayerControls>();
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
        if (Input.GetAxis("SpeedChange") <= -0.05f && !_sceneManager._ending) SpeedUp();
        else if (Input.GetAxis("SpeedChange") >= 0.05f) SlowDown();
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
        if (!_boostParticle.activeInHierarchy && !_sceneManager._ending) _camShake.Shake();
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
