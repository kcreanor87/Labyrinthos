using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ghosts : MonoBehaviour {

    public int _level;

    public Transform _sphere;
    public bool _activeGhost;
    public int _totalFrames;

    public Transform _ghostGO;
    public Transform _ghostShip;
    public int _ghostIndex;

    public _manager _Pmanager;

    public List<float> _playerTimeX = new List<float>();
    public List<float> _playerTimeY = new List<float>();
    public List<float> _playerRot = new List<float>();
    public List<float> _ghostTimeX = new List<float>();
    public List<float> _ghostTimeY = new List<float>();
    public List<float> _ghostRot = new List<float>();
     
    public float _xRot;
    public float _yRot;

    public float _shipRot;

    public GameObject _ghostParticleReference;

    private void Awake()
    {
        _level = SceneManager.GetActiveScene().buildIndex - 2;
        _sphere = GameObject.Find("World001Container").GetComponentInChildren<Transform>();
        _ghostGO = GameObject.Find("Ghost").GetComponent<Transform>();
        _ghostShip = _ghostGO.Find("GhostGO").GetComponentInChildren<Transform>();
        _ghostParticleReference = GameObject.Find("GhostParticleReference");
        _ghostParticleReference.transform.SetParent(_sphere);
        _ghostGO.gameObject.SetActive(false);
        _Pmanager = gameObject.GetComponent<_manager>();
        LoadGhosts();
    }

    public void StartGhost()
    {
        if (_activeGhost) _ghostGO.gameObject.SetActive(true);
    }

    public void LoadGhosts()
    {
       if (PlayerPrefs.HasKey("GhostLevelx" + _level + "_0"))
        {
            _activeGhost = true;
            for (int j = 0; j < Mathf.FloorToInt(_playerManager._times[_level] * 60.0f); j++)
            {
                _ghostTimeX.Add(PlayerPrefs.GetFloat("GhostLevelx" + _level + "_" + j));
                _ghostTimeY.Add(PlayerPrefs.GetFloat("GhostLevely" + _level + "_" + j));
                _ghostRot.Add(PlayerPrefs.GetFloat("GhostLevelRot" + _level + "_" + j));
            }
            _totalFrames = _ghostTimeX.Count;
        }
        else
        {
            _ghostGO.gameObject.SetActive(false);
        }
    }

    public void SaveGhost()
    {
        print("level " + _level + " Ghost saved");
        for (int j = 0; j < _ghostIndex; j++)
        {
            PlayerPrefs.DeleteKey(("GhostLevelx" + _level + "_" + j));
            PlayerPrefs.DeleteKey(("GhostLevely" + _level + "_" + j));
            PlayerPrefs.DeleteKey(("GhostLevelRot" + _level + "_" + j));
        }
        for (int j = 0; j < _playerTimeX.Count; j++)
        {
            PlayerPrefs.SetFloat(("GhostLevelx" + _level + "_" + j), _playerTimeX[j]);
            PlayerPrefs.SetFloat(("GhostLevely" + _level + "_" + j), _playerTimeY[j]);
            PlayerPrefs.SetFloat(("GhostLevelRot" + _level + "_" + j), _playerRot[j]);
        }
    }

    private void FixedUpdate()
    {
        if (_Pmanager._inMenu) return; 
        _playerTimeX.Add(-_xRot);
        _playerTimeY.Add(-_yRot);
        _playerRot.Add(_shipRot);
        if (_activeGhost)
        {            
            _ghostGO.Rotate(_ghostTimeY[_ghostIndex], _ghostTimeX[_ghostIndex], 0, Space.Self);
            _ghostShip.localRotation = Quaternion.Euler(0, 0, _ghostRot[_ghostIndex]);
            _ghostIndex++;
            _activeGhost = (_Pmanager._timer <= _playerManager._times[_level]);
        }
        else
        {
            _ghostGO.gameObject.SetActive(false);
        }
    }
}
