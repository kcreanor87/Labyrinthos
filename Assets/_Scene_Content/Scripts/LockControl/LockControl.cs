using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockControl : MonoBehaviour {

    public Animator _triggeredAnim;

    public void Trigger()
    {
        _triggeredAnim.SetBool("Triggered", true);
    }
}
