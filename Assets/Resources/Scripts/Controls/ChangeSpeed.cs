﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpeed : MonoBehaviour {

    public PlayerControls _playerControls;
    public ParticleSystem _boostParticle;

    private void Start()
    {
        _boostParticle = GameObject.Find("BoostParticles").GetComponent<ParticleSystem>();
        _boostParticle.Stop();
        _playerControls = GameObject.Find("Player").GetComponent<PlayerControls>();
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
