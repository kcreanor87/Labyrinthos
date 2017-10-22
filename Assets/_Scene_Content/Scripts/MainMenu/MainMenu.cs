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
    public List<GameObject> _levelLines = new List<GameObject>();

    public LevelTimeContainer _timeContainer;

    public bool _waiting;

    public Button _startGame;
    
    public GameObject _intro;
    public GameObject _levelSelect;
    public GameObject _sectorSelect;
    public GameObject _worldcontainer;  

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
    public Color _regColor;
    public Color _lockedColour;
    public Animator _uiAnims;

    public List<GameObject> _planets = new List<GameObject>();

    public Animator _worldContainerAnim;
    public Animator _buttonPrompt;

    public int _screenIndex;
    public int _playerNumber;

    public Animator _introAnim;
    public Animator _sectorAnim;
    public Animator _levelAnim;
    public Animator _planetFader;
    public Animator _introSpheres;

    // Use this for initialization

    private void Awake()
    {
        if (GameObject.Find("_playerManager") == null) SceneManager.LoadScene(0);        
    }

    void Start()
    {
        FindGameObjects();
        _startGame.Select();
        _levelSelected = -1;
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttonImages.Add(_buttons[i].GetComponentInChildren<Image>());
        }
        for (int i = 0; i < _sectorButtons.Count; i++)
        {
            _sectorButtons[i].interactable = (i <= (_playerManager._playerLevel / 9));
        }
        
        if (_playerManager._skipscreen)
        {
            _intro.SetActive(false);
            var sector = Mathf.FloorToInt((float)_playerManager._levelIndex / 9);
            var level = _playerManager._levelIndex - (sector * 9);
            OpenLevel(sector);
            SelectLevel(level);
            _buttons[level].Select();
            _playerNumber = 1;
        }
        else
        {
            SelectLevel(0);
            _levelSelect.SetActive(false);
            _worldcontainer.SetActive(false);
            _sectorSelect.SetActive(false);
        }
    }

    private void Update()
    {
        InputManager();
    }

    void InputManager()
    {
        if (_waiting)
        {
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }
        if (Input.GetButtonDown("Cancel") && !_intro.activeInHierarchy)
        {
            Back(_screenIndex);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    void FindGameObjects()
    {
        _timeContainer = GameObject.Find("_playerManager").GetComponent<LevelTimeContainer>();
        _worldContainerAnim = _worldcontainer.GetComponent<Animator>();
        _intro = transform.Find("Intro").gameObject;
        _introAnim = _intro.GetComponent<Animator>();
        _sectorSelect = transform.Find("SectorSelect").gameObject;
        _sectorAnim = _sectorSelect.GetComponent<Animator>();
        _levelSelect = transform.Find("LevelSelect").gameObject;
        _levelAnim = _levelSelect.GetComponent<Animator>();
        _planetFader = GameObject.Find("PlanetFader").GetComponent<Animator>();
        _startGame = GameObject.Find("StartGame").GetComponent<Button>();
        _introSpheres = GameObject.Find("Circles").GetComponent<Animator>();
        _buttonPrompt = GameObject.Find("ButtonPrompt").GetComponent<Animator>();
    }

    public void SelectLevel(int i)
    {
        if (i == _levelSelected) return;
        _levelSelected = i;
        var level = i + (_activeSector * 9);
        _playerManager._levelIndex = level;
        foreach (Transform child in _worldcontainer.transform)
        {
            if (child.name != "StartPad") GameObject.Destroy(child.gameObject);
        }
        Instantiate (_planets[level] as GameObject, _worldcontainer.transform);
        _worldContainerAnim.SetBool("Reset", !_worldContainerAnim.GetBool("Reset"));
        for (int j = 0; j < _buttons.Count; j++) {
            SpriteSelector(j);
            _buttonImages[j].color = (i == j) ? _highlightColor : _regColor;
            if (_playerManager._playerLevel >= (j + (_activeSector * 9)))
            {
                _buttonImages[j].gameObject.SetActive(true);
                _buttons[j].interactable = true;
                _buttons[j].GetComponentInChildren<Animator>().enabled = (i == j);                
            }
            else
            {
                _buttonImages[j].gameObject.SetActive(false);
                _buttons[j].interactable = false;
            }
        }     
    }

    void SpriteSelector(int index)
    {
        var level = index + (_activeSector * 9);
        if (level >= _playerManager._totalLevels + 1)
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
        for (int i = 0; i < _levelLines.Count; i++)
        {
            _levelLines[i].SetActive(maxLevel > i);
        }
        _buttons[maxLevel].Select();
        _sectorSelect.SetActive(false);
        _screenIndex = 1;
        _planetFader.SetBool("Active", true);
        _buttonPrompt.SetBool("Pressed", false);
    }

    public void OpenSectorSelect()
    {
        if (_screenIndex < 1)
        {
            _introSpheres.SetBool("Outro", true);
            StartCoroutine(FadeOutToSector(_introAnim));
        }
        else
        {
            _planetFader.SetBool("Active", false);
            _buttonPrompt.SetBool("Pressed", true);
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
        _waiting = true;
        anim.SetBool("Outro", true);
        yield return new WaitForSeconds(1.4f);
        _waiting = false;
        OpenSector();
    }

    public IEnumerator FadeOutToLevel(int sector)
    {
        _waiting = true;
        _sectorAnim.SetBool("Outro", true);
        yield return new WaitForSeconds(1.4f);
        _waiting = false;
        OpenLevel(sector);
    }

    public IEnumerator FadeOutToIntro()
    {
        _waiting = true;        
        if (_introSpheres.gameObject.activeInHierarchy) _introSpheres.SetBool("Outro", false);
        else { _sectorAnim.SetBool("Outro", true); }
        yield return new WaitForSeconds(1.4f);
        _waiting = false;
        ToggleIntro();
    }

    public IEnumerator FadeOutToMultiplayer()
    {
        _waiting = true;
        _introAnim.SetBool("Outro", true);
        _introSpheres.SetBool("Outro", true);
        yield return new WaitForSeconds(1.4f);
        _waiting = false;
        OpenMultiplayer();
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

    public void OpenMultiplayer()
    {
        _playerManager._P1Score = 0;
        _playerManager._P2Score = 0;
        _playerNumber = 2;
        _introAnim.SetBool("Outro", true);
        StartCoroutine(SceneChange());
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
    }

    public void Play()
    {
        _levelAnim.SetBool("Outro", true);
        _buttonPrompt.SetBool("Pressed", true);
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
    }

    public IEnumerator SceneChange()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(_playerNumber + 1);
    }
}
