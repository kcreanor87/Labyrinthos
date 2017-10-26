using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ghosts : MonoBehaviour {

    public int _level;

    public Transform _sphere;
    public bool _activeGhost;
    public int _totalFrames;
    public int _endTimer;

    public Transform _ghostGO;
    public Transform _ghostShip;
    public GameObject _ghostExplosion;
    public int _ghostIndex;

    public int _saveIndexi;

    public _manager _Pmanager;

    public bool _saving;

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
        _ghostExplosion = _ghostGO.Find("Explosion").gameObject;
        _ghostExplosion.SetActive(false);
        _Pmanager = gameObject.GetComponent<_manager>();
        LoadGhosts();
    }

    public void StartGhost()
    {
        if (_activeGhost)
        {
            _ghostGO.gameObject.SetActive(true);
        }
    }

    public void LoadGhosts()
    {
        GhostData.inst.LoadGhostData(_level);
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

    public void SaveGhost()
    {
        if (!_saving) StartCoroutine(SaveData());
    }

    public IEnumerator SaveData()
    {
        _saving = true;
        GhostData.inst._ghostLevels[_level]._framePos.Clear();
        GhostData.inst._ghostLevels[_level]._frameRot.Clear();
        for (int i = 0; i < _playerTimeX.Count; i++)
        {
            GhostData.inst._ghostLevels[_level]._framePos.Add(new Vector3(_playerTimeX[i], _playerTimeY[i], _playerTimeZ[i]));
            GhostData.inst._ghostLevels[_level]._frameRot.Add(new Vector3(_playerRotX[i], _playerRotY[i], _playerRotZ[i]));
        }
        GhostData.inst.SaveGhostData(_level);
        _Pmanager._saving = false; 
        yield return null;
    }

    private void FixedUpdate()
    {
        if (_Pmanager._inMenu) return;
        if (_activeGhost && (_ghostIndex < _totalFrames))
        {
            _ghostGO.eulerAngles = new Vector3(_ghostTimeX[_ghostIndex], _ghostTimeY[_ghostIndex], _ghostTimeZ[_ghostIndex]);
            _ghostShip.localRotation = (_ghostRotX[_ghostIndex] == 0) ? Quaternion.Euler(-90, 180, 0) : Quaternion.Euler(_ghostRotX[_ghostIndex], _ghostRotY[_ghostIndex], _ghostRotZ[_ghostIndex]);
            _ghostIndex++;
            _activeGhost = (_ghostIndex < _totalFrames);
            if (_totalFrames - _ghostIndex <= 0)
            {
                _ghostShip.GetComponentInChildren<MeshRenderer>().enabled = false;
                _ghostExplosion.SetActive(true);
            }
        }        
        if (_Pmanager._ending && _endTimer < 60)
        {
            _endTimer++;
        }
        else if (_endTimer >= 60) return;
        _playerTimeX.Add(_xRot);
        _playerTimeY.Add(_yRot);
        _playerTimeZ.Add(_zRot);
        _playerRotX.Add(_shipRot.x);
        _playerRotY.Add(_shipRot.y);
        _playerRotZ.Add(_shipRot.z);        
    }
}
