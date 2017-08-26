using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltips : MonoBehaviour {

    public int _index;
    public GameObject _tooltip;
    public bool _active;
    public List <GameObject> _tipGOs = new List<GameObject>();
    public _manager _sceneManager;

    private void Awake()
    {
        _sceneManager = gameObject.GetComponent<_manager>();
        _active = (!_playerManager._tooltips);
        if (_active) StartCoroutine(Countdown());
    }

    public void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            print("Submit Pressed");
            ClickAnywhereToContinue();
        }
    }

    public void ClickAnywhereToContinue()
    {
        if (_index < _tipGOs.Count)
        {
            for (int i = 0; i < _tipGOs.Count; i++)
            {
                _tipGOs[i].SetActive(i == _index);
            }
            _index++;
        }
        else
        {
            _tooltip.SetActive(false);
            _sceneManager._tooltipActive = false;
            PlayerPrefs.SetInt("Tooltip" + _index, 1);
        }        
    }

    public IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1.0f);
        _tooltip.SetActive(_active);
        _sceneManager._tooltipActive = true;
    }
}
