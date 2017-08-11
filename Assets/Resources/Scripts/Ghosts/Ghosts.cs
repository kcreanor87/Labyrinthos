﻿using System.Collections;
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

    public int _saveIndexi;
    public int _saveIndexj;

    public _manager _Pmanager;

    public List<float> _playerTimeX = new List<float>();
    public List<float> _playerTimeY = new List<float>();
    public List<float> _playerRotX = new List<float>();
    public List<float> _playerRotY = new List<float>();
    public List<float> _playerRotZ = new List<float>();
    public List<float> _ghostTimeX = new List<float>();
    public List<float> _ghostTimeY = new List<float>();
    public List<float> _ghostRotX = new List<float>();
    public List<float> _ghostRotY = new List<float>();
    public List<float> _ghostRotZ = new List<float>();

    public float _xRot;
    public float _yRot;

    public Vector3 _shipRot;

    public GameObject _ghostParticleReference;

    private void Awake()
    {
        _level = _playerManager._levelIndex;
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
        if (PlayerPrefs.HasKey("GhostLevelx" + _level + "_0"))
        {
            _activeGhost = true;
            for (int j = 0; j < Mathf.FloorToInt(_playerManager._times[_level] * 60.0f); j++)
            {
                _ghostTimeX.Add(PlayerPrefs.GetFloat("GhostLevelx" + _level + "_" + j));
                _ghostTimeY.Add(PlayerPrefs.GetFloat("GhostLevely" + _level + "_" + j));
                _ghostRotX.Add(PlayerPrefs.GetFloat("GhostLevelRotx" + _level + "_" + j));
                _ghostRotY.Add(PlayerPrefs.GetFloat("GhostLevelRoty" + _level + "_" + j));
                _ghostRotZ.Add(PlayerPrefs.GetFloat("GhostLevelRotz" + _level + "_" + j));
            }
            _totalFrames = _ghostTimeX.Count;
            StartGhost();
        }
        else
        {
            _ghostGO.gameObject.SetActive(false);
        }
    }

    public void SaveGhost()
    {
        StartCoroutine(SaveData());
    }

    public IEnumerator SaveData()
    {
        print("level " + _level + " Ghost saved");        
        while (_saveIndexi < _ghostIndex)
        {
            if (_saveIndexi < _playerTimeX.Count) {
                PlayerPrefs.SetFloat(("GhostLevelx" + _level + "_" + _saveIndexj), _playerTimeX[_saveIndexi]);
                PlayerPrefs.SetFloat(("GhostLevely" + _level + "_" + _saveIndexj), _playerTimeY[_saveIndexi]);
                PlayerPrefs.SetFloat(("GhostLevelRotx" + _level + "_" + _saveIndexj), _playerRotX[_saveIndexi]);
                PlayerPrefs.SetFloat(("GhostLevelRoty" + _level + "_" + _saveIndexj), _playerRotY[_saveIndexi]);
                PlayerPrefs.SetFloat(("GhostLevelRotz" + _level + "_" + _saveIndexj), _playerRotZ[_saveIndexi]);

            }
            else
            {
                PlayerPrefs.DeleteKey(("GhostLevelx" + _level + "_" + _saveIndexi));
                PlayerPrefs.DeleteKey(("GhostLevely" + _level + "_" + _saveIndexi));
                PlayerPrefs.DeleteKey(("GhostLevelRotx" + _level + "_" + _saveIndexi));
                PlayerPrefs.DeleteKey(("GhostLevelRoty" + _level + "_" + _saveIndexi));
                PlayerPrefs.DeleteKey(("GhostLevelRotz" + _level + "_" + _saveIndexi));
            }
            _saveIndexi++;
            yield return null;
        }
        _Pmanager.EnableButtons();
    }

    private void FixedUpdate()
    {
        if (_Pmanager._inMenu) return; 
        _playerTimeX.Add(-_xRot);
        _playerTimeY.Add(_yRot);
        _playerRotX.Add(_shipRot.x);
        _playerRotY.Add(_shipRot.y);
        _playerRotZ.Add(_shipRot.z);
        if (_activeGhost)
        {            
            _ghostGO.Rotate(_ghostTimeY[_ghostIndex], _ghostTimeX[_ghostIndex], 0, Space.Self);
            _ghostShip.localRotation = Quaternion.Euler(_ghostRotX[_ghostIndex], _ghostRotY[_ghostIndex], _ghostRotZ[_ghostIndex]);
            _ghostIndex++;
            _activeGhost = (_Pmanager._timer <= _playerManager._times[_level]);
        }
        else
        {
            _ghostGO.gameObject.SetActive(false);
        }
    }
}
