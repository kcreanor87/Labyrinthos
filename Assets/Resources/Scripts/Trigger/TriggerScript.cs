using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour {

    public Animator _anim;

    public bool _bool;

    public int _stage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") Triggered();
    }

    public void Triggered()
    {
        if (_bool)
        {
            _anim.SetBool("Triggered", true);
        }
        else
        {
            _anim.SetInteger("Stage", _stage);
        }
    }
}
