using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour {

    public _manager manager;
    public AudioSource _explode;
    public AudioSource _lock;
    public AudioSource _click;
    public GameObject _playerMesh;
    public GameObject _brokenMesh;

    private void Start()
    {
        _playerMesh = GameObject.Find("Ship001");
        _brokenMesh = GameObject.Find("Ship001_broken");
        _brokenMesh.SetActive(false);
        manager = GameObject.Find("UI").GetComponent<_manager>();
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
            _playerMesh.SetActive(false);
            _brokenMesh.SetActive(true);
            manager.EndLevel(false);
        }
        else if (other.tag == "Lock")
        {
            other.transform.GetComponentInParent<LockControl>().SwitchWalls(true);
            _lock.Play();
        }
    }
}
