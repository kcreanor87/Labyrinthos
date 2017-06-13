using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public List<Button> _buttons = new List<Button>();
    public List<GameObject> _worlds = new List<GameObject>();
    public LevelTimeContainer _timeContainer;
    public int _activeSector;
    public GameObject _intro;
    public GameObject _introContainer;
    public GameObject _levelSelect;
    public GameObject _worldcontainer;
    public List<GameObject> _levelSelections = new List<GameObject>();
    public Sprite _mute;
    public Sprite _unmute;
    public Image _muteBtn;

    public Button _prevSectorBtn;
    public Button _nextSectorBtn;

    public Text _bestTimeTxt;
    //Increase this as sectors are added
    public int _maxSector = 2;

    public int _levelSelected;

    public Sprite _regular;
    public Sprite _rankCsprite;
    public Sprite _rankBsprite;
    public Sprite _rankAsprite;
    public Sprite _rankSsprite;
    public Sprite _selected;

    public Color _highlightColor;

	// Use this for initialization
	void Start () {
        _timeContainer = GameObject.Find("_playerManager").GetComponent<LevelTimeContainer>();
        _levelSelect = GameObject.Find("LevelSelect");
        _introContainer = GameObject.Find("IntroContainer");
        //_muteBtn = GameObject.Find("Mute").GetComponent<Image>();
        //_muteBtn.sprite = (AudioListener.pause == true) ? _unmute : _mute;
        _bestTimeTxt = GameObject.Find("BestTimeTxt").GetComponent<Text>();
        _bestTimeTxt.text = "Record: " + _playerManager._times[_playerManager._playerLevel].ToString("F2") + "s";
        _prevSectorBtn = GameObject.Find("PrevSectorBtn").GetComponent<Button>();
        _nextSectorBtn = GameObject.Find("NextSectorBtn").GetComponent<Button>();
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].interactable = (i <= _playerManager._playerLevel);
            _buttons[i].GetComponentInChildren<Text>().enabled = (i <= _playerManager._playerLevel);
        }
        SelectLevel(0);
        for (int i = 0; i < _levelSelections.Count; i++)
        {
            _levelSelections[i].SetActive(false);
        }
        _levelSelect.SetActive(false);
        _worldcontainer.SetActive(false);
    }

    public void SelectLevel(int i)
    {
        _levelSelected = i + (_activeSector * 9);
        for (int j = 0; j < _worlds.Count; j++) {
            _worlds[j].SetActive(_levelSelected == j);
            SpriteSelector(j); 
            _buttons[j].image.color = (_levelSelected == j) ? Color.white : _highlightColor;
            _buttons[j].interactable = (j <= _playerManager._playerLevel);

        }
        _bestTimeTxt.text = "Record: " + _playerManager._times[_levelSelected].ToString("F2") + "s";
        
    }

    void SpriteSelector(int index)
    {
        switch (_timeContainer._levelTimes[index]._rank)
        {
            case "-":
                _buttons[index].image.sprite = _regular;
                break;
            case "S":
                _buttons[index].image.sprite = _rankSsprite;
                break;
            case "A":
                _buttons[index].image.sprite = _rankAsprite;
                break;
            case "B":
                _buttons[index].image.sprite = _rankBsprite;
                break;
            case "C":
                _buttons[index].image.sprite = _rankCsprite;
                break;

        }
    }

    public void ToggleSector(bool positive)
    {
        _activeSector = (positive) ? (_activeSector + 1) : (_activeSector - 1);
        _prevSectorBtn.enabled = (_activeSector > 0);
        _nextSectorBtn.enabled = (_activeSector < _maxSector - 1);
        for (int i = 0; i < _levelSelections.Count; i++)
        {
            _levelSelections[i].SetActive(i == _activeSector);
        }
        SelectLevel(0);
    }

    public void OpenLevelSelect()
    {
        _intro.SetActive(false);
        _introContainer.SetActive(false);
        _levelSelect.SetActive(true);
        _levelSelections[0].SetActive(true);
        _worldcontainer.SetActive(true);
        SelectLevel(0);
    }

    public void Play()
    {
        SceneManager.LoadScene(_levelSelected + 2);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Mute()
    {
        AudioListener.pause = !AudioListener.pause;
        //_muteBtn.sprite = (AudioListener.pause == true) ? _unmute : _mute;
    }
}
