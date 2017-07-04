using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour {

    public _manager manager;
    public AudioSource _explode;
    public AudioSource _lock;
    public AudioSource _click;

    private void Start()
    {
        manager = GameObject.Find("UI").GetComponent<_manager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Crate")
        {           
            Destroy(other.transform.parent.gameObject);
            other.transform.parent.parent.FindChild("Collected").GetComponent<ParticleSystem>().Play();
            manager.UpdateCrates();
            _click.Play();
        }
        else if (other.tag == "Wall")
        {
            manager.EndLevel(false);
        }
        else if (other.tag == "Lock")
        {
            other.transform.GetComponentInParent<LockControl>().SwitchWalls(true);
            _lock.Play();
        }
    }
}
