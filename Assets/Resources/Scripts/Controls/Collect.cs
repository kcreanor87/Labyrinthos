using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour {

    public _manager manager;
    public ParticleSystem _explosion;
    public AudioSource _explode;
    public AudioSource _lock;
    public AudioSource _click;

    private void Start()
    {
        manager = GameObject.Find("UI").GetComponent<_manager>();
        _explosion = transform.parent.FindChild("Explosion").GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Crate")
        {           
            Destroy(other.transform.parent.gameObject);
            manager.UpdateCrates();
            _click.Play();
        }
        else if (other.tag == "Wall")
        {
            manager.EndLevel(false);
            _explosion.Play();
            _explode.Play();
        }
        else if (other.tag == "Lock")
        {
            other.transform.GetComponentInParent<LockControl>().SwitchWalls(true);
            _lock.Play();
        }
    }
}
