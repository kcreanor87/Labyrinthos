using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CoopControls : MonoBehaviour
{

    public int _playerIndex;

    public Camera _cam;

    public Transform _lookPos;
    public Transform _ship;
    public Transform _lookDirGO;
    public Transform _focalPoint;
    public GameObject _entryParticles;
    public float _baseSpeed = 1f;
    public float _speed = 0.8f;
    private int inputX;
    public _CoopManager manager;
    public bool _brake;
    public bool _boost;
    public float _cameraBase = 56.0f;
    public float _cameraZoomIn = 50.0f;
    public float _cameraZoomOut = 62.0f;
    public float _boostAmount = 1.4f;
    public float _brakeAmount = 0.45f;
    public float _maxBrake = 0.45f;

    public Animator _playerAnim;

    public bool _resetting;
    public bool _destroyed;

    public CameraShake _camShake;

    // Use this for initialization
    void Awake()
    {
        manager = GameObject.Find("UI").GetComponent<_CoopManager>();
        _cam = transform.GetComponentInChildren<Camera>();
        _lookPos = GameObject.Find("LookPos" + _playerIndex).GetComponent<Transform>();
        _ship = GameObject.Find("Player" + (_playerIndex + 1)).GetComponent<Transform>();
        _playerAnim = gameObject.GetComponentInChildren<Animator>();
        _entryParticles = transform.Find("Player" + (_playerIndex + 1)).Find("EntryParticles").gameObject;
        _camShake = _cam.GetComponent<CameraShake>();
        _lookDirGO = transform.Find("LookDir" + (_playerIndex + 1)).GetComponent<Transform>();
        _focalPoint = _lookDirGO.Find("FocalPoint" + (_playerIndex + 1)).GetComponent<Transform>();
        CameraCheck();
    }

    public void FixedUpdate()
    {
        if (manager._inMenu || _resetting) return;
        if (manager._ending || _destroyed) _boost = false;
        var pathX = "Joy" + _playerIndex + "X";
        var pathY = "Joy" + _playerIndex + "Y";
        var x = Input.GetAxis(pathX);
        var y = Input.GetAxis(pathY);
        float lookOrigin = _playerIndex > 0 ? ((Screen.width / 4) * 3) : (Screen.width / 4);
        var pos = new Vector3(lookOrigin + (x * (Screen.width / 4)), (Screen.height / 2) + (-y * (Screen.width / 4)), 16.5f);
        _lookPos.position = _cam.ScreenToWorldPoint(pos);
        if (Mathf.Abs(x) <= 0.12f && Mathf.Abs(y) <= 0.12f) return;
        if ((_brake || _destroyed) && _speed >= _brakeAmount)
        {
            _speed -= Time.deltaTime * 2;
            if (_speed < _brakeAmount) _speed = _brakeAmount;
            if (manager._ending||_destroyed) _brakeAmount = 0.0f;
        }
        else if (_boost && _speed < _boostAmount)
        {
            _speed += Time.deltaTime * 2;
        }
        else if (!_brake && !_boost)
        {
            _speed = Mathf.MoveTowards(_baseSpeed, _speed, 4 * Time.deltaTime);
            _brakeAmount = _maxBrake;
        }
        x *= _speed;
        y *= _speed;
        transform.Rotate(y, -x, 0, Space.Self);

        var targetPoint = _lookPos.position - _lookDirGO.position;
        var targetRotation = Quaternion.LookRotation(targetPoint, Vector3.forward);
        _lookDirGO.rotation = Quaternion.Slerp(_lookDirGO.rotation, targetRotation, Time.deltaTime * 25.0f);

        if (!_destroyed)  _ship.LookAt(_focalPoint, transform.forward);
    }

    public void CameraCheck()
    {
        StartCoroutine(CameraZoom());
    }

    public IEnumerator CameraZoom()
    {
        if (_boost && _cam.fieldOfView < _cameraZoomOut && !manager._inMenu) _cam.fieldOfView++;
        else if (_brake && _cam.fieldOfView > _cameraZoomIn && !manager._inMenu) _cam.fieldOfView--;
        else if (_cam.fieldOfView != _cameraBase && !_boost && !_brake)
        {
            if (_cam.fieldOfView > _cameraBase) _cam.fieldOfView--;
            else _cam.fieldOfView++;
        }
        yield return new WaitForSeconds(0.03f);
        CameraCheck();
    }

    public void ReturnToStart()
    {
        _destroyed = true;
        _playerAnim.Play("PlayerOutro");
        _entryParticles.SetActive(false);
        StartCoroutine(ResetPosition());
    }

    public IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(0.45f);
        _resetting = true;
        yield return new WaitForSeconds(0.25f);        
        _playerAnim.Play("PlayerIntro");
        _entryParticles.SetActive(true);
        transform.rotation = Quaternion.identity;
        _ship.eulerAngles = new Vector3(-90, 180, 0);
        yield return new WaitForSeconds(0.9f);
        _destroyed = false;
        _resetting = false;
        
    }
}
