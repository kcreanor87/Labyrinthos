using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerControls : MonoBehaviour {

    public Transform _sphere;
    public float _baseSpeed = 1f;
    public float _speed = 1f;
    private int inputX;
    public _manager manager;
    public Ghosts _ghost;
    public RectTransform _joystickTransform;
    public bool _brake;
    public bool _boost;
    public float _boostAmount = 1.8f;
    public float _brakeAmount = 0.3f;

    // Use this for initialization
    void Start () {
        manager = GameObject.Find("UI").GetComponent<_manager>();
        _ghost = GameObject.Find("UI").GetComponent<Ghosts>();
        _sphere = GameObject.Find("World001Container").GetComponentInChildren<Transform>();
	}

    public void FixedUpdate()
    {
        if (manager._inMenu) return;
        var x = CrossPlatformInputManager.GetAxis("Horizontal");
        var y = CrossPlatformInputManager.GetAxis("Vertical");
        if (Mathf.Abs(x) <= 0.12f && Mathf.Abs(y) <= 0.12f) return;        
        if (_brake && _speed > _brakeAmount)
        {
            _speed -= Time.deltaTime * 2;
        }
        else if (_boost && _speed < _boostAmount)
        {
            _speed += Time.deltaTime * 2;
        }
        else
        {
            _speed = Mathf.MoveTowards(_baseSpeed, _speed, 2 * Time.deltaTime);
        }
        var lookVec = new Vector3(y, x, 4096);
        x *= _speed;
        y *= _speed;
        _ghost._xRot = x;
        _ghost._yRot = y;
        _sphere.Rotate(y, x, 0, Space.World);
        transform.rotation = Quaternion.LookRotation(lookVec, Vector3.forward);
        _ghost._shipRot = transform.localRotation.eulerAngles.z;
    }
}
