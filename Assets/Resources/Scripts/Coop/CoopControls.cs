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
    public GameObject _entryParticles;
    public float _baseSpeed = 1f;
    public float _speed = 0.8f;
    private int inputX;
    public _CoopManager manager;
    public RectTransform _joystickTransform;
    public bool _brake;
    public bool _boost;
    public float _cameraBase = 56.0f;
    public float _cameraZoomIn = 50.0f;
    public float _cameraZoomOut = 62.0f;
    public float _boostAmount = 1.4f;
    public float _brakeAmount = 0.15f;

    public Animator _playerAnim;

    public Quaternion _startRot;

    public bool _resetting;

    // Use this for initialization
    void Awake()
    {
        manager = GameObject.Find("UI").GetComponent<_CoopManager>();
        _cam = transform.GetComponentInChildren<Camera>();
        _lookPos = GameObject.Find("LookPos" + _playerIndex).GetComponent<Transform>();
        _ship = GameObject.Find("Player" + (_playerIndex + 1)).GetComponent<Transform>();
        _playerAnim = gameObject.GetComponentInChildren<Animator>();
        _startRot = GameObject.Find("Start_P" + (_playerIndex + 1)).transform.rotation;
        _entryParticles = transform.Find("Player" + (_playerIndex + 1)).Find("EntryParticles").gameObject;
        CameraCheck();
    }

    public void FixedUpdate()
    {
        if (manager._inMenu || _resetting) return;
        var pathX = "Joy" + _playerIndex + "X";
        var pathY = "Joy" + _playerIndex + "Y";
        var x = Input.GetAxis(pathX);
        var y = Input.GetAxis(pathY);
        float lookOrigin = _playerIndex > 0 ? ((Screen.width / 4) * 3) : (Screen.width / 4);
        var pos = new Vector3(lookOrigin + (x * (Screen.width / 4)), (Screen.width / 4) + (-y * (Screen.width / 4)), 16f);
        _lookPos.position = _cam.ScreenToWorldPoint(pos);
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
        x *= _speed;
        y *= _speed;
        transform.Rotate(y, -x, 0, Space.Self);
        _ship.LookAt(_lookPos, transform.forward);
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
        _playerAnim.Play("PlayerOutro");
        _resetting = true;
        _entryParticles.SetActive(false);
        StartCoroutine(ResetPosition());
    }

    public IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(1.0f);
        transform.rotation = _startRot;
        _playerAnim.Play("PlayerIntro");
        _entryParticles.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _resetting = false;
    }
}
