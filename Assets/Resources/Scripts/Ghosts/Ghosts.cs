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

    private void Awake()
    {
        _level = SceneManager.GetActiveScene().buildIndex - 2;
        _sphere = GameObject.Find("World001Container").GetComponent<Transform>();
        _ghostGO = GameObject.Find("Ghost").GetComponent<Transform>();
        _ghostShip = _ghostGO.Find("GhostGO").GetComponentInChildren<Transform>();
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
       if (PlayerPrefs.HasKey("GhostLevelx" + _level + "0"))
        {
            _activeGhost = true;
            for (int j = 0; j < Mathf.CeilToInt(_playerManager._times[_level] * 60.0f); j++)
            {
                _ghostTimeX.Add(PlayerPrefs.GetFloat("GhostLevelx" + _level + j));
                _ghostTimeY.Add(PlayerPrefs.GetFloat("GhostLevely" + _level + j));
                _ghostRot.Add(PlayerPrefs.GetFloat("GhostLevelRot" + _level + j));
            }
            _totalFrames = _ghostTimeX.Count;
        }
        else
        {
            _ghostGO.gameObject.SetActive(false);
        }
    }

    public void SaveGhost(int i)
    {
        print("level " + i + " Ghost saved");
        for (int j = 0; j < _ghostIndex + 6; j++)
        {
            PlayerPrefs.DeleteKey(("GhostLevelx" + i + j));
            PlayerPrefs.DeleteKey(("GhostLevely" + i + j));
            PlayerPrefs.DeleteKey(("GhostLevelRot" + i + j));
        }
        for (int j = 0; j < _playerTimeX.Count; j++)
        {
            PlayerPrefs.SetFloat(("GhostLevelx" + i + j), _playerTimeX[j]);
            PlayerPrefs.SetFloat(("GhostLevely" + i + j), _playerTimeY[j]);
            PlayerPrefs.SetFloat(("GhostLevelRot" + i + j), _playerRot[j]);
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
            _activeGhost = (_Pmanager._timer >= _playerManager._times[_level]);
        }
        else
        {
            _ghostGO.gameObject.SetActive(false);
        }
    }
}
