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

    public bool _chasing;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SpawnChaser();
            _col.enabled = false;
        }
    }

    void SpawnChaser()
    {
        _playerPosIndex = _ghosts._playerTimeX.Count;
        _chaser.gameObject.SetActive(true);
        StartCoroutine(WaitForSpawn());
    }

    public IEnumerator WaitForSpawn()
    {
        yield return new WaitForSeconds(0.6f);
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
                _chaser.Rotate(_ghosts._playerTimeY[_playerPosIndex], _ghosts._playerTimeX[_playerPosIndex], 0, Space.Self);
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
