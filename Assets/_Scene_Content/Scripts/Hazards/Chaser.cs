using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour {

    public Collider _col;
    public Collider _chaserCol;
    public Transform _chaser;
    public Transform _chaserBody;
    public int _playerPosIndex;
    public _manager _Pmanager;
    public Ghosts _ghosts;
    public Quaternion _startPos;
    public bool _chasing;
    public float _timeDiff = 0.5f;

    public void Start()
    {
        _col = gameObject.GetComponent<Collider>();
        _Pmanager = GameObject.Find("UI").GetComponent<_manager>();
        _ghosts = GameObject.Find("UI").GetComponent<Ghosts>();
        _chaser = transform.Find("Chaser").GetComponent<Transform>();
        _chaserBody = _chaser.Find("ChaserGO").GetComponentInChildren<Transform>();
        _chaserCol = _chaserBody.GetComponentInChildren<Collider>();
        _chaserCol.enabled = false;
        _chaser.gameObject.SetActive(false);
        _startPos = transform.rotation;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _col.enabled = false;
            SpawnChaser();
        }
    }

    void SpawnChaser()
    {        
        _playerPosIndex = _ghosts._playerTimeX.Count + 5;
        _chaser.gameObject.SetActive(true);
        StartCoroutine(WaitForSpawn());
    }

    public IEnumerator WaitForSpawn()
    {
        yield return new WaitForSeconds(_timeDiff);
        _chaserCol.enabled = true;
        _chasing = true;
    }

    public void FixedUpdate()
    {
        if (_chasing)
        {
            if (_Pmanager._inMenu) return;
            if (!_Pmanager._ending)
            {
                _chaser.eulerAngles = new Vector3(-_ghosts._playerTimeX[_playerPosIndex], _ghosts._playerTimeY[_playerPosIndex], _ghosts._playerTimeZ[_playerPosIndex]);
                _chaserBody.localRotation = Quaternion.Euler(_ghosts._playerRotX[_playerPosIndex], _ghosts._playerRotY[_playerPosIndex], _ghosts._playerRotZ[_playerPosIndex]);
                _playerPosIndex++;
            }
            else
            {
                _chaser.gameObject.SetActive(false);
            }
        }
    }
}
