using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpeed : MonoBehaviour {

    public PlayerControls _playerControls;
    public _manager _sceneManager;
    public ParticleSystem _boostParticle;
    public ParticleSystem _boostGaps;
    public ParticleSystem _regParticle;
    public ParticleSystem _regGaps;

    private void Awake()
    {
        _sceneManager = GameObject.Find("UI").GetComponent<_manager>();
        _boostParticle = GameObject.Find("BoostParticles").GetComponent<ParticleSystem>();
        _boostParticle.Stop();
        _boostGaps = GameObject.Find("BoostGaps").GetComponent<ParticleSystem>();
        _boostGaps.Stop();
        _regGaps = GameObject.Find("Jet_gaps").GetComponent<ParticleSystem>();
        _regParticle = GameObject.Find("Jet").GetComponent<ParticleSystem>();
        _playerControls = GameObject.Find("Player").GetComponent<PlayerControls>();
    }

    public void FixedUpdate()
    {
        if (!_sceneManager._inMenu) return;
        if (_playerControls._boost && !_boostParticle.IsAlive())
        {
            _boostGaps.Play();
            _boostParticle.Play();
            _regParticle.Stop();
            _regGaps.Stop();
        }
        else if (!_playerControls._boost)
        {
            _boostParticle.Stop();
            _boostGaps.Stop();
            _regParticle.Play();
            _regGaps.Play();
        }
    }

    public void SpeedUp()
    {
        _playerControls._boost = true;
        _boostParticle.Play();
        _boostGaps.Play();
        _regParticle.Stop();
        _regGaps.Stop();
    }

    public void SlowDown()
    {
        _playerControls._brake = true;
        _boostParticle.Stop();
        _boostGaps.Stop();
        _regParticle.Play();
        _regGaps.Play();
    }

    public void ReturnToNormal()
    {
        _playerControls._boost = false;
        _playerControls._brake = false;
        _boostParticle.Stop();
        _boostGaps.Stop();
        _regParticle.Play();
        _regGaps.Play();
    }
}
