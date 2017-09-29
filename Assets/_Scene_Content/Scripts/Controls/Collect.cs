using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour {

    public _manager manager;

    public CameraShake _camShake;

    public bool _gameOver;

    public GameObject _collected;

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
            Instantiate(_collected, other.transform.position, transform.rotation);
            manager.UpdateCrates();
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
        else if (other.tag == "Trigger")
        {
            other.gameObject.GetComponent<LockControl>().Trigger();
            Destroy(other.transform.parent.gameObject);
            Instantiate(_collected, other.transform.position, transform.rotation);
        }
    }
}
