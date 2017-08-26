using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockControl : MonoBehaviour {

    public GameObject _baseWall;
    public GameObject _deactivatedWall;
    public GameObject _lock;
    public float _timer = 3.0f;

	// Use this for initialization
	void Start () {
        _deactivatedWall.SetActive(false);		
	}

    public void SwitchWalls(bool active) {
        _baseWall.SetActive(!active);
        _deactivatedWall.SetActive(active);
        _lock.SetActive(!active);
        if (active) StartCoroutine(Timer());
    }

    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(_timer);
        SwitchWalls(false);
    }
}
