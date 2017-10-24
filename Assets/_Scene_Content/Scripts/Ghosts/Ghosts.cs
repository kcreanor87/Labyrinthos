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

    public int _saveIndexi;

    public _manager _Pmanager;

    public List<float> _playerTimeX = new List<float>();
    public List<float> _playerTimeY = new List<float>();
    public List<float> _playerTimeZ = new List<float>();
    public List<float> _playerRotX = new List<float>();
    public List<float> _playerRotY = new List<float>();
    public List<float> _playerRotZ = new List<float>();
    public List<float> _ghostTimeX = new List<float>();
    public List<float> _ghostTimeY = new List<float>();
    public List<float> _ghostTimeZ = new List<float>();
    public List<float> _ghostRotX = new List<float>();
    public List<float> _ghostRotY = new List<float>();
    public List<float> _ghostRotZ = new List<float>();

    public float _xRot;
    public float _yRot;
    public float _zRot;

    public Vector3 _shipRot;

    private void Awake()
    {
        _saveIndexi = 0;
        _level = _playerManager._levelIndex;
        _ghostGO = GameObject.Find("Ghost").GetComponent<Transform>();
        _ghostShip = _ghostGO.Find("GhostGO").GetComponentInChildren<Transform>();        
        _Pmanager = gameObject.GetComponent<_manager>();
        LoadGhosts();
    }

    public void StartGhost()
    {
        if (_activeGhost) _ghostGO.gameObject.SetActive(true);
    }

    public void LoadGhosts()
    {
        if (_playerManager._times[_level] > 0)
        {
            GhostData.inst.LoadGhostData();
            if (GhostData.inst._ghostLevels[_level]._framePos.Count > 0)
            {
                _activeGhost = true;
                for (int j = 0; j < GhostData.inst._ghostLevels[_level]._framePos.Count; j++)
                {
                    _ghostTimeX.Add(GhostData.inst._ghostLevels[_level]._framePos[j].x);
                    _ghostTimeY.Add(GhostData.inst._ghostLevels[_level]._framePos[j].y);
                    _ghostTimeZ.Add(GhostData.inst._ghostLevels[_level]._framePos[j].z);
                    _ghostRotX.Add(GhostData.inst._ghostLevels[_level]._frameRot[j].x);
                    _ghostRotY.Add(GhostData.inst._ghostLevels[_level]._frameRot[j].y);
                    _ghostRotZ.Add(GhostData.inst._ghostLevels[_level]._frameRot[j].z);
                }
                _totalFrames = _ghostTimeX.Count;
            }
            else
            {
                _ghostGO.gameObject.SetActive(false);
            }
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
        GhostData.inst._ghostLevels[_level]._framePos.Clear();
        GhostData.inst._ghostLevels[_level]._frameRot.Clear();
        if (_ghostIndex == 0) _ghostIndex = _playerTimeX.Count;
        print("level " + _level + " Ghost saved");
        while (_saveIndexi < _ghostIndex)
        {
            if (_saveIndexi < _playerTimeX.Count) {
                GhostData.inst._ghostLevels[_level]._framePos.Add(new Vector3(_playerTimeX[_saveIndexi], _playerTimeY[_saveIndexi], _playerTimeZ[_saveIndexi]));
                GhostData.inst._ghostLevels[_level]._frameRot.Add(new Vector3(_playerRotX[_saveIndexi], _playerRotY[_saveIndexi], _playerRotZ[_saveIndexi]));
            }
            _saveIndexi++;
            yield return null;
        }
        //GhostData.inst.SaveGhostData(_level);
        _Pmanager._saving = false; 
        _Pmanager._gameOverPrompt.SetBool("Saving", false);
        yield return null;
    }

    private void FixedUpdate()
    {
        if (_Pmanager._inMenu) return;
        _playerTimeX.Add(-_xRot);
        _playerTimeY.Add(_yRot);
        _playerTimeZ.Add(_zRot);
        _playerRotX.Add(_shipRot.x);
        _playerRotY.Add(_shipRot.y);
        _playerRotZ.Add(_shipRot.z);
        if (_activeGhost && !_Pmanager._ending)
        {            
            _ghostGO.eulerAngles = new Vector3(-_ghostTimeX[_ghostIndex], _ghostTimeY[_ghostIndex], _ghostTimeZ[_ghostIndex]);
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
