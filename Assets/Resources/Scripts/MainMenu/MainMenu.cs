using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {

    public List<Button> _buttons = new List<Button>();
    public List<Image> _buttonImages = new List<Image>();
    public List<Button> _sectorButtons = new List<Button>();
    public List<Image> _lines = new List<Image>();

    public LevelTimeContainer _timeContainer;  

    public Button _startGame;
    
    public GameObject _intro;
    public GameObject _levelSelect;
    public GameObject _sectorSelect;
    public GameObject _worldcontainer;   

    public Text _bestTimeTxt;

    //Increase this as sectors are added
    public int _maxSector = 2;
    public int _activeSector;
    public int _levelSelected;

    public Sprite _regular;
    public Sprite _rankCsprite;
    public Sprite _rankBsprite;
    public Sprite _rankAsprite;
    public Sprite _rankSsprite;
    public Sprite _selected;
    public Sprite _mute;
    public Sprite _unmute;
    public Sprite _lockedSprite;

    public Image _muteBtn;

    public Color _highlightColor;
    public Animator _uiAnims;
    public GameObject _unlockAllPrompt;
    public GameObject _settingsMenu;
    public GameObject _wipePrompt;
    public GameObject _wipeScreen;

    public List<GameObject> _planets = new List<GameObject>();

    public Animator _worldContainerAnim;
    public RectTransform _highlighter;

    public int _screenIndex;
    public int _playerNumber;

    public Animator _introAnim;
    public Animator _sectorAnim;
    public Animator _levelAnim;
    public Animator _planetFader;

    // Use this for initialization

    private void Awake()
    {
        if (GameObject.Find("_playerManager") == null) SceneManager.LoadScene(0);        
    }

    void Start()
    {
        FindGameObjects();
        _startGame.Select();     
        _settingsMenu.SetActive(false);        
        _wipePrompt.SetActive(false);        
        _wipeScreen.SetActive(false);        
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttonImages.Add(_buttons[i].GetComponentInChildren<Image>());
        }
        for (int i = 0; i < _sectorButtons.Count; i++)
        {
            _sectorButtons[i].interactable = (i <= (_playerManager._playerLevel / 9));
        }
        SelectLevel(0);        
        _levelSelect.SetActive(false);
        _worldcontainer.SetActive(false);
        _sectorSelect.SetActive(false);
        if (_playerManager._skipscreen)
        {
            OpenSectorSelect();
            ToggleSector(Mathf.FloorToInt((float)_playerManager._playerLevel / 9));
        }        
    }

    private void Update()
    {
        InputManager();
    }

    void InputManager()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Back(_screenIndex);
        }
        if (Input.GetButtonDown("Pause") && _screenIndex > 0)
        {
            Play();
        }
        if (_screenIndex > 0)
        {
            _highlighter.localPosition = EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject.transform.localPosition;
            if (Input.GetButtonDown("RightShoulder"))
            {
                SectorSwitcher(true);
            }
            if (Input.GetButtonDown("LeftShoulder"))
            {
                SectorSwitcher(false);
            }
        } 
    }

    void FindGameObjects()
    {
        _settingsMenu = GameObject.Find("SettingsMenu");
        _wipePrompt = GameObject.Find("WipePrompt");
        _wipeScreen = GameObject.Find("WipeScreen");
        _timeContainer = GameObject.Find("_playerManager").GetComponent<LevelTimeContainer>();
        _worldContainerAnim = _worldcontainer.GetComponent<Animator>();
        _intro = transform.Find("Intro").gameObject;
        _introAnim = _intro.GetComponent<Animator>();
        _sectorSelect = transform.Find("SectorSelect").gameObject;
        _sectorAnim = _sectorSelect.GetComponent<Animator>();
        _levelSelect = transform.Find("LevelSelect").gameObject;
        _levelAnim = _levelSelect.GetComponent<Animator>();
        _planetFader = GameObject.Find("PlanetFader").GetComponent<Animator>();
        _bestTimeTxt = _levelSelect.transform.Find("BestTimeTxt").GetComponent<Text>();
        _startGame = GameObject.Find("StartGame").GetComponent<Button>();
        _highlighter = GameObject.Find("Highlighter").GetComponent<RectTransform>();
    }

    public void SelectLevel(int i)
    {
        _levelSelected = i + (_activeSector * 9);
        _playerManager._levelIndex = _levelSelected;
        foreach (Transform child in _worldcontainer.transform)
        {
            if (child.name != "StartPad") GameObject.Destroy(child.gameObject);
        }
        Instantiate (_planets[_levelSelected] as GameObject, _worldcontainer.transform);
        _worldContainerAnim.SetBool("Reset", !_worldContainerAnim.GetBool("Reset"));
        for (int j = 0; j < _buttons.Count; j++) {
            SpriteSelector(j);
            _buttonImages[j].color = (i == j) ? Color.white : _highlightColor;
            _buttons[j].interactable = (_playerManager._playerLevel >= (j + (_activeSector * 9)));
            if (!_buttons[j].interactable)
            {
                _buttonImages[j].sprite = _lockedSprite;
                _buttonImages[j].color = Color.white;
            }
            _buttons[j].GetComponentInChildren<Animator>().enabled = (i == j);
        }
        _bestTimeTxt.text = (_playerManager._times[_levelSelected] > 0.0f) ? "Record: " + _playerManager._times[_levelSelected].ToString("F2") + "s" : "Record: - - : - -";        
    }

    void SpriteSelector(int index)
    {
        var level = index + (_activeSector * 9);
        if (level >= _playerManager._totalLevels)
        {
            _buttonImages[index].sprite = _lockedSprite;
            return;
        }
        switch (_timeContainer._levelTimes[level]._rank)
        {
            case "-":
                _buttonImages[index].sprite = _regular;
                break;
            case "S":
                _buttonImages[index].sprite = _rankSsprite;
                break;
            case "A":
                _buttonImages[index].sprite = _rankAsprite;
                break;
            case "B":
                _buttonImages[index].sprite = _rankBsprite;
                break;
            case "C":
                _buttonImages[index].sprite = _rankCsprite;
                break;
        }
    }

    public void ToggleSector(int sector)
    {
        StartCoroutine(FadeOutToLevel(sector));
    }

    public void OpenLevel(int sector)
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].transform.Find("ButtonImg").rotation = Quaternion.identity;
        }
        _activeSector = sector;
        _levelSelect.SetActive(true);
        _worldcontainer.SetActive(true);
        var maxLevel = (_playerManager._playerLevel >= (_activeSector + 1) * 9) ? 8 : (_playerManager._playerLevel - (_activeSector) * 9);
        SelectLevel(maxLevel);
        _buttons[maxLevel].Select();
        _sectorSelect.SetActive(false);
        _screenIndex = 1;
        _planetFader.SetBool("Active", true);
    }

    public void SectorSwitcher(bool increase)
    {
        if (increase)
        {
            if ((_activeSector + 1) <= (_playerManager._playerLevel/9))
            {
                ToggleSector(_activeSector + 1);
            }
            else
            {
                ToggleSector(0);
            }
        }
        else
        {
            if (_activeSector > 0)
            {
                ToggleSector(_activeSector - 1);
            }
            else
            {
                ToggleSector(Mathf.FloorToInt(_playerManager._playerLevel / 9));
            }
        }
    }

    public void OpenSectorSelect()
    {
        if (_screenIndex < 1)
        {
            StartCoroutine(FadeOutToSector(_introAnim));
        }
        else
        {
            _planetFader.SetBool("Active", false);
            StartCoroutine(FadeOutToSector(_levelAnim));
        }
        
    }

    public void OpenSector()
    {
        for (int i = 0; i < _lines.Count; i++)
        {
            if (!(i < Mathf.FloorToInt(_playerManager._playerLevel / 9))) _lines[i].gameObject.SetActive(false);
        }
        _intro.SetActive(false);
        _worldcontainer.SetActive(false);
        _levelSelect.SetActive(false);
        _sectorSelect.SetActive(true);
        _sectorButtons[Mathf.FloorToInt(_playerManager._playerLevel / 9)].Select();
    }

    public IEnumerator FadeOutToSector(Animator anim)
    {
        anim.SetBool("Outro", true);
        yield return new WaitForSeconds(1.8f);
        OpenSector();
    }

    public IEnumerator FadeOutToLevel(int sector)
    {
        _sectorAnim.SetBool("Outro", true);
        yield return new WaitForSeconds(1.4f);
        OpenLevel(sector);
    }

    public IEnumerator FadeOutToIntro()
    {
        _sectorAnim.SetBool("Outro", true);
        yield return new WaitForSeconds(1.4f);
        ToggleIntro();
    }

    public void ToggleIntro()
    {
        _intro.SetActive(true);
        _sectorSelect.SetActive(false);
        _startGame.Select();
    }
    public void PlayerNumbers(int i)
    {
        _playerNumber = i;
    }

    public void UnlockAllPrompt(bool active)
    {
        _unlockAllPrompt.SetActive(active);
    }

    public void Back(int index)
    {
        if (index == 0)
        {
            StartCoroutine(FadeOutToIntro());
        }
        else
        {
            OpenSectorSelect();
            _screenIndex = 0;
        }
    }

    public void ToggleSettings(bool active)
    {
        _settingsMenu.SetActive(active);
    }

    public void WipePrompt(bool active)
    {
        _wipePrompt.SetActive(active);
    }

    public void WipeData()
    {
        _wipeScreen.SetActive(true);
        PlayerPrefs.DeleteAll();
        Destroy(GameObject.Find("_playerManager"));
        Application.Quit();
    }

    public void UnlockAll()
    {
        _playerManager._playerLevel = _playerManager._totalLevels - 1;
        PlayerPrefs.SetInt("PlayerLevel", _playerManager._playerLevel);
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
        OpenSectorSelect();
        _unlockAllPrompt.SetActive(false);
    }

    public void Play()
    {
        _levelAnim.SetBool("Outro", true);
        _planetFader.SetBool("Active", false);
        StartCoroutine(SceneChange());
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

    public IEnumerator SceneChange()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(_playerNumber + 1);
    }
}
