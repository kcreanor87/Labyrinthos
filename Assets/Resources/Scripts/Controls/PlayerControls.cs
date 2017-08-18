using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerControls : MonoBehaviour {

    public Transform _lookPos;
    public Transform _ship;
    public GameObject _particleReference;   
    public float _baseSpeed = 1f;
    public float _speed = 0.8f;
    private int inputX;
    public _manager manager;
    public Ghosts _ghost;
    public bool _brake;
    public bool _boost;
    public float _cameraBase = 56.0f;
    public float _cameraZoomIn = 50.0f;
    public float _cameraZoomOut = 62.0f;
    public float _boostAmount = 1.4f;
    public float _brakeAmount = 0.15f;

    // Use this for initialization
    void Awake () {
        manager = GameObject.Find("UI").GetComponent<_manager>();
        _ghost = GameObject.Find("UI").GetComponent<Ghosts>();
        _lookPos = GameObject.Find("LookPos").GetComponent<Transform>();
        _ship = GameObject.Find("Player").GetComponent<Transform>();
        CameraCheck();
	}

    public void FixedUpdate()
    {
        if (manager._inMenu) return;
        if (manager._ending) _boost = false;
        var x = Input.GetAxis("Horizontal_Game");
        var y = Input.GetAxis("Vertical_Game");
        var pos = new Vector3((Screen.width / 2) + (x * (Screen.height/2)), (Screen.height / 2)  + (-y * (Screen.height / 2)), 16f);
        _lookPos.position = Camera.main.ScreenToWorldPoint(pos);
        if (Mathf.Abs(x) <= 0.12f && Mathf.Abs(y) <= 0.12f) return;        
        if (_brake && _speed >= _brakeAmount)
        {
            _speed -= Time.deltaTime * 2;
            if (manager._ending) _brakeAmount = 0.0f;            
        }
        else if (_boost && _speed <= _boostAmount)
        {
            _speed += Time.deltaTime * 2;            
        }
        else if (!_brake && !_boost)
        {
            _speed = Mathf.MoveTowards(_baseSpeed, _speed, 4 * Time.deltaTime);
        }
        x *= _speed;
        y *= _speed;
        _ghost._xRot = x;
        _ghost._yRot = y;
        transform.Rotate(y, -x, 0, Space.Self);

         if (_speed >=  0.15f) _ship.LookAt(_lookPos, transform.forward);

        _ghost._shipRot = _ship.localRotation.eulerAngles;
    }

    public void CameraCheck() {
        StartCoroutine(CameraZoom());
    }

    public IEnumerator CameraZoom()
    {
        if (_boost && Camera.main.fieldOfView < _cameraZoomOut && !manager._inMenu) Camera.main.fieldOfView++;
        else if (_brake && Camera.main.fieldOfView > _cameraZoomIn && !manager._inMenu) Camera.main.fieldOfView--;
        else if (Camera.main.fieldOfView != _cameraBase && !_boost && !_brake)
        {
            if (Camera.main.fieldOfView > _cameraBase) Camera.main.fieldOfView--;
            else Camera.main.fieldOfView++;
        }
        yield return new WaitForSeconds(0.03f);
        CameraCheck();
    }    
}
