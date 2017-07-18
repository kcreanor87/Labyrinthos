using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerControls : MonoBehaviour {

    public Transform _sphere;
    public GameObject _particleReference;   
    public float _baseSpeed = 1f;
    public float _speed = 1f;
    private int inputX;
    public _manager manager;
    public Ghosts _ghost;
    public RectTransform _joystickTransform;
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
        _sphere = GameObject.Find("World001Container").GetComponentInChildren<Transform>();
        _particleReference = GameObject.Find("ParticleReference");        
        _particleReference.transform.SetParent(_sphere);        
        CameraCheck();
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
            _speed = Mathf.MoveTowards(_baseSpeed, _speed, 4 * Time.deltaTime);
            
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
