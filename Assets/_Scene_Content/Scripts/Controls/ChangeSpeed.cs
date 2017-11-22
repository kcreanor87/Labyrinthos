using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpeed : MonoBehaviour {

    private PlayerControls _playerControls;
    private CameraShake _camShake;
    private _manager _sceneManager;
    private GameObject _boostParticle;

    private void Awake()
    {
        FindGOs();
        _boostParticle.SetActive(false);
    }

    void FindGOs()
    {
        //Collect necessary GameObjects
        _sceneManager = GameObject.Find("UI").GetComponent<_manager>();
        _camShake = Camera.main.GetComponent<CameraShake>();
        _boostParticle = GameObject.Find("BoostParticles");
        _playerControls = GameObject.Find("pfbPlayer001").GetComponent<PlayerControls>();
    }

    public void FixedUpdate()
    {
        //Do nothing if a menu is open
        if (_sceneManager._inMenu) return;
        //If the level is ending, apply brakes
        if (_sceneManager._ending){
            SlowDown();
            return;
        }
        //Check which axis is active and and call relevant function
        if (Input.GetAxis("SpeedChange") <= -0.05f && !_sceneManager._ending) SpeedUp();
        else if (Input.GetAxis("SpeedChange") >= 0.05f) SlowDown();
        else ReturnToNormal();
        //Enable disable boost particle effects
        if (_playerControls._boost && !_boostParticle.activeInHierarchy){
            _boostParticle.SetActive(true);            
        }
        else if (!_playerControls._boost){
            _boostParticle.SetActive(false);
        }
    }

    //Toggle boost bool in PlayerControls, set camera shaking and enable particles
    public void SpeedUp()
    {        
        if (_playerControls._brake) return;        
        _playerControls._boost = true;
        if (!_boostParticle.activeInHierarchy && !_sceneManager._ending) _camShake.Shake();
        _boostParticle.SetActive(true);        
    }

    //Toggle brake bool in PlayerControls and disable particles
    public void SlowDown()
    {        
        if (_playerControls._boost) return;        
        _playerControls._brake = true;
        _boostParticle.SetActive(false);
    }

    //Reset bools
    public void ReturnToNormal()
    {        
        _playerControls._boost = false;
        _playerControls._brake = false;
        _boostParticle.SetActive(false);
    }
}
