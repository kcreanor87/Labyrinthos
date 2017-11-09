using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSetup : MonoBehaviour {

    public GameObject[] _walls;
    public GameObject[] _grounds;


    // Use this for initialization
    void Start () {
        _walls = GameObject.FindGameObjectsWithTag("Wall");
        _grounds = GameObject.FindGameObjectsWithTag("Ground");
        Setup();
	}

    void Setup()
    {
        foreach (GameObject _wall in _walls)
        {
            _wall.AddComponent<MeshCollider>();
            Rigidbody rb = _wall.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            _wall.AddComponent<FadingTexture>();
        }

        foreach (GameObject _ground in _grounds)
        {
            _ground.AddComponent<MeshCollider>();
            Rigidbody rb = _ground.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
