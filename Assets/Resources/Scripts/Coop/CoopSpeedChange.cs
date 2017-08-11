using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopSpeedChange : MonoBehaviour {

    public int _playerIndex;
    public CoopControls _playerControls;
    public _CoopManager _sceneManager;
    public ParticleSystem _boostParticle;

    private void Awake()
    {
        _playerControls = GameObject.Find("pfbCo-opP" + (_playerIndex + 1)).GetComponent<CoopControls>();
        _playerIndex = _playerControls._playerIndex;
        _sceneManager = GameObject.Find("UI").GetComponent<_CoopManager>();
        _boostParticle = GameObject.Find("BoostParticles" + _playerIndex).GetComponent<ParticleSystem>();
        _boostParticle.Stop();
        
    }

    public void FixedUpdate()
    {
        if (_sceneManager._inMenu) return;
        if (Input.GetAxis("SpeedChange" + _playerIndex) <= -0.05f) SpeedUp();
        else if (Input.GetAxis("SpeedChange" + _playerIndex) >= 0.05f) SlowDown();
        else ReturnToNormal();
        if (_playerControls._boost && !_boostParticle.IsAlive())
        {
            _boostParticle.Play();
        }
        else if (!_playerControls._boost)
        {
            _boostParticle.Stop();
        }
    }

    public void SpeedUp()
    {
        _playerControls._boost = true;
        _boostParticle.Play();
    }

    public void SlowDown()
    {
        _playerControls._brake = true;
        _boostParticle.Stop();
    }

    public void ReturnToNormal()
    {
        _playerControls._boost = false;
        _playerControls._brake = false;
        _boostParticle.Stop();
    }
}
