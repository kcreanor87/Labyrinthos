using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour {

    public _manager manager;
    public AudioSource _explode;
    public AudioSource _lock;
    public AudioSource _click;

    public CameraShake _camShake;

    public bool _gameOver;

    private void Start()
    {
        manager = GameObject.Find("UI").GetComponent<_manager>();
        _camShake = Camera.main.GetComponent<CameraShake>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Crate")
        {           
            Destroy(other.transform.parent.gameObject);
            other.transform.parent.parent.Find("Collected").GetComponent<ParticleSystem>().Play();
            manager.UpdateCrates();
            _click.Play();
        }
        else if (other.tag == "Wall" || other.tag == "Hazard")
        {
            if (!_gameOver)
            {
                _camShake.Shake();
                manager.EndLevel(false);
                _gameOver = true;
            }            
        }
        else if (other.tag == "Lock")
        {
            other.transform.GetComponentInParent<LockControl>().SwitchWalls(true);
            _lock.Play();
        }
    }
}
