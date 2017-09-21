using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSetup : MonoBehaviour {

    public GameObject[] _walls;

	// Use this for initialization
	void Start () {
        _walls = GameObject.FindGameObjectsWithTag("Wall");
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
        }
    }
}
