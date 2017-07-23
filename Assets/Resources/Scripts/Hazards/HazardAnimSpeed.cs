using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardAnimSpeed : MonoBehaviour {

    public float _animSpeed = 1.0f;
    public Animator _anim;

	// Use this for initialization
	void Start () {
        _anim = gameObject.GetComponent<Animator>();
        _anim.speed = _animSpeed;
	}
}
