using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopCollect : MonoBehaviour {

    public int _playerIndex;

    public _CoopManager manager;
    public CoopControls _controls;
    public AudioSource _explode;
    public AudioSource _lock;
    public AudioSource _click;

    public CameraShake _camShake;

    public bool _gameOver;

    public GameObject _collected;

    public string _crateTag;

    private void Start()
    {
        manager = GameObject.Find("UI").GetComponent<_CoopManager>();
        _controls = GameObject.Find("pfbCo-opP" + (_playerIndex + 1)).GetComponent<CoopControls>();
        _playerIndex = _controls._playerIndex;
        _camShake = _controls._cam.GetComponent<CameraShake>();
        _crateTag = "Crate" + _playerIndex;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == _crateTag)
        {           
            Destroy(other.transform.parent.gameObject);
            Instantiate(_collected, other.transform.position, transform.rotation);
            manager.UpdateCrates(_playerIndex);
            //_click.Play();
        }
        else if (other.tag == "Wall" || other.tag == "Hazard")
        {
            _camShake.Shake();
            _controls.ReturnToStart();
        }
    }
}
