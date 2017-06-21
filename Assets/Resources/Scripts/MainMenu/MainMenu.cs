using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public List<Button> _buttons = new List<Button>();
    public List<Button> _sectorButtons = new List<Button>();
    public List<GameObject> _worlds = new List<GameObject>();
    public LevelTimeContainer _timeContainer;
    public int _activeSector;
    public GameObject _intro;
    public GameObject _introContainer;
    public GameObject _levelSelect;
    public GameObject _sectorSelect;
    public GameObject _worldcontainer;
    public List<GameObject> _levelSelections = new List<GameObject>();
    public Sprite _mute;
    public Sprite _unmute;
    public Image _muteBtn;

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
        _intro = transform.FindChild("Intro").gameObject;
        _sectorSelect = transform.FindChild("SectorSelect").gameObject;
        _timeContainer = GameObject.Find("_playerManager").GetComponent<LevelTimeContainer>();
        _levelSelect = transform.FindChild("LevelSelect").gameObject;
        _introContainer = GameObject.Find("IntroContainer");
        //_muteBtn = GameObject.Find("Mute").GetComponent<Image>();
        //_muteBtn.sprite = (AudioListener.pause == true) ? _unmute : _mute;
        _bestTimeTxt = GameObject.Find("BestTimeTxt").GetComponent<Text>();
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].interactable = (i <= _playerManager._playerLevel);
            _buttons[i].GetComponentInChildren<Text>().enabled = (i <= _playerManager._playerLevel);
        }
        for (int i = 0; i < _sectorButtons.Count; i++)
        {
            _sectorButtons[i].interactable = (i <= (_playerManager._playerLevel / 9));
        }
        SelectLevel(0);
        for (int i = 0; i < _levelSelections.Count; i++)
        {
            _levelSelections[i].SetActive(false);
        }
        _levelSelect.SetActive(false);
        _worldcontainer.SetActive(false);
        _sectorSelect.SetActive(false);
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
        _bestTimeTxt.text = (_playerManager._times[_levelSelected] > 0.0f) ? "Record: " + _playerManager._times[_levelSelected].ToString("F2") + "s" : "Record: - - : - -";
        
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

    public void ToggleSector(int sector)
    {
        _activeSector = sector;
        _levelSelect.SetActive(true);
        for (int i = 0; i < _levelSelections.Count; i++)
        {
            _levelSelections[i].SetActive(i == _activeSector);
        }
        _worldcontainer.SetActive(true);
        SelectLevel(0);
        _sectorSelect.SetActive(false);        
        
    }

    public void OpenSectorSelect()
    {
        _intro.SetActive(false);
        _introContainer.SetActive(false);
        _worldcontainer.SetActive(false);
        _levelSelect.SetActive(false);
        _sectorSelect.SetActive(true);
        
    }

    public void ToggleIntro()
    {
        _intro.SetActive(true);
        _introContainer.SetActive(true);
        _sectorSelect.SetActive(false);
        
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

    public void Back(int index)
    {
        if (index == 0)
        {
            ToggleIntro();
        }
        else
        {
            OpenSectorSelect();
        }
    }

    public void WipeData()
    {
        PlayerPrefs.DeleteAll();
        Destroy(GameObject.Find("_playerManager"));
        SceneManager.LoadScene(0);
    }

    public void UnlockAll()
    {
        _playerManager._playerLevel = _playerManager._totalLevels - 1;
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].interactable = (i <= _playerManager._playerLevel);
            _buttons[i].GetComponentInChildren<Text>().enabled = (i <= _playerManager._playerLevel);
        }
        for (int i = 0; i < _sectorButtons.Count; i++)
        {
            _sectorButtons[i].interactable = (i <= (_playerManager._playerLevel / 9));
        }
        SelectLevel(0);
        for (int i = 0; i < _levelSelections.Count; i++)
        {
            _levelSelections[i].SetActive(false);
        }
        OpenSectorSelect();
    }
}
