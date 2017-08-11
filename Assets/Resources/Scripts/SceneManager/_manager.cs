using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

public class _manager : MonoBehaviour {

    public bool _developmentMode;
    public GameObject _playerManagerPrefab;

    public float _timer;
    public float _maxTime = 30.0f;
    public float _best;
    public float _countdown = 1.2f;

    public int _cratesRemaining;
    public int levelIndex;
    public string build;
    
    public bool _inMenu;   
    public bool _gameOver;

    public GameObject _winScreen;    

    public Text _timeTakenText;
    public Text _recordTxt;
    public Text _levelTxt;
    public Text _bestTxt;
    public Text _timerTxt;
    public Text _cratesRemainingTxt;
    public Text _rankText;    

    public Ghosts _ghosts;    
    public Image _rankImage;
    public Sprite _rankSsprite, _rankAsprite, _rankBsprite, _rankCsprite;
    public LevelTimeContainer _timeContainer;

    public Animator _UIanim;
    public Animator _playerAnim;

    //Skip function variables
    public GameObject _pauseScreen;
    public GameObject _rotCam;
    public bool _paused;
    public bool _tooltipActive;

    //Initial selection buttons
    public Button _nextLevel;
    public Button _mainMenu;
    public Button _restart;
    public Button _resume;

    private void Awake()
    {
        if (GameObject.Find("_playerManager") == null && !_developmentMode) SceneManager.LoadScene(0);
        if (_developmentMode)
        {
            GameObject _pmPrefab = Instantiate(_playerManagerPrefab) as GameObject;
            _pmPrefab.name = "_playerManager";
        }
        SpawnLevel();

    }

    // Use this for initialization
    void Start()
    {                
        GameObjectFinder();
        SpawnCrates();
        _nextLevel.interactable = false;
        _mainMenu.interactable = false;
        _restart.interactable = false;
        _pauseScreen.SetActive(false);
        _rotCam.SetActive(false);
        _countdown = 2.0f;
        _timer = 0.0f;
        _inMenu = true;
        build = (_playerManager._times[levelIndex] == 0.0f) ? "--:--" : _playerManager._times[levelIndex].ToString("F2");
        _bestTxt.text = "Record: " + build + " s";
        _recordTxt.text = build + " s";
        _levelTxt.text = "Level " + (levelIndex + 1);
        _levelTxt.enabled = false;
        _winScreen.SetActive(false);
        _timeTakenText.enabled = false;
        _cratesRemainingTxt.text = _cratesRemaining.ToString();
        _timerTxt.text = _timer.ToString("F2");
    }

    void SpawnLevel()
    {
        levelIndex = _playerManager._levelIndex;
        var sector = Mathf.Floor(levelIndex / 9) + 1;
        var level = levelIndex - ((sector - 1) * 9);
        var path = "Prefabs/Worlds/Sc0" + sector + "/pfbWorldSc0" + sector + "_0" + (level + 1);
        GameObject World = Instantiate(Resources.Load(path, typeof (GameObject))) as GameObject;
        World.transform.parent = GameObject.Find("World001Container").GetComponent<Transform>();
        World.transform.localPosition = new Vector3(0,0,0);
        World.transform.localScale = new Vector3(1,1,1);
    }

    void SpawnCrates()
    {
        var collectables = GameObject.FindGameObjectsWithTag("Collectable");
        foreach (GameObject collectable in collectables)
        {
            Instantiate(Resources.Load("Prefabs/Collectables/Collectable_base01") as GameObject, collectable.transform);
        }
    }

    void GameObjectFinder()
    {
        _playerAnim = GameObject.Find("Player").GetComponent<Animator>();
        _timeContainer = GameObject.Find("_playerManager").GetComponent<LevelTimeContainer>();
        _rankImage = GameObject.Find("Rank").GetComponent<Image>();
        _rankText = GameObject.Find("RankText").GetComponent<Text>();
        _bestTxt = GameObject.Find("BestTimeTxt").GetComponent<Text>();
        _recordTxt = GameObject.Find("RecordTxt").GetComponent<Text>();
        _winScreen = GameObject.Find("GameOver_win");
        _cratesRemainingTxt = GameObject.Find("CratesRemainingTxt").GetComponent<Text>();
        _cratesRemaining = GameObject.FindGameObjectsWithTag("Crate").Length;
        _timeTakenText = GameObject.Find("TimeTakenTxt").GetComponent<Text>();
        _winScreen = GameObject.Find("GameOver_win");
        _timerTxt = GameObject.Find("TimerTxt").GetComponent<Text>();
        _levelTxt = GameObject.Find("LevelTxt").GetComponent<Text>();
        _pauseScreen = GameObject.Find("PauseMenu");
        _rotCam = GameObject.Find("RotationCam");
        _UIanim = gameObject.GetComponent<Animator>();
        _ghosts = gameObject.GetComponent<Ghosts>();
        _nextLevel = GameObject.Find("NextLevel").GetComponent<Button>();
        _mainMenu = GameObject.Find("MainMenu").GetComponent<Button>();
        _restart = GameObject.Find("Restart").GetComponent<Button>();
        _resume = GameObject.Find("Resume").GetComponent<Button>();
    }

	void FixedUpdate () {
        if (_inMenu && !_gameOver && !_paused)
        {
            Countdown();
            return;
        }
        if (!_gameOver && !_paused) UpdateTimer();
        if (Input.GetKeyDown(KeyCode.W)){ EndLevel(true);}
        if (_gameOver && EventSystem.current.GetComponent<EventSystem>().currentSelectedGameObject == null) _nextLevel.Select();
    }

    private void Update()
    {
        InputManager();
    }

    void InputManager()
    {
        if (Input.GetButtonDown("Pause") && !_tooltipActive && !_gameOver) PauseGame(!_paused);
    }

    void UpdateTimer()
    {
        if (_paused) return;
        _timer += Time.deltaTime;
        _timerTxt.text = _timer.ToString("F2");
    }

    void Countdown()
    {
        if (_tooltipActive) return;
        _countdown -= Time.deltaTime;
        if (_countdown <= 0.01f)
        {
            if (!_developmentMode) _ghosts.StartGhost();
            _bestTxt.enabled = false;
            _inMenu = false;
        }
    }   

    public void UpdateCrates()
    {
        _cratesRemaining--;
        _cratesRemainingTxt.text = _cratesRemaining.ToString();        
        if (_cratesRemaining == 0) EndLevel(true);
    }

    public void PauseGame(bool active)
    {
        _paused = active;
        _pauseScreen.SetActive(_paused);
        _rotCam.SetActive(_paused);
        _inMenu = _paused;
        _resume.Select();
    }

    public void EndLevel(bool victory)
    {
        _playerAnim.SetBool("Outro", true);
        _UIanim.SetBool("Complete", true);
        _UIanim.SetBool("Victory", victory);     
        _levelTxt.enabled = true;
        _inMenu = true;
        _gameOver = true;
        AnalyticsData(victory);        
        if (victory)
        {
            RankSwitcher(levelIndex, _timer);
            if (_timer < _playerManager._times[levelIndex] || (_playerManager._times[levelIndex] == 0.0f))
            {
                _playerManager._times[levelIndex] = _timer;
                _bestTxt.text = "New Record: " + _timer.ToString("F2") + "s";
                _bestTxt.color = Color.green;
                _playerManager.SaveTimes();
                _ghosts.SaveGhost();
            }
            else
            {
                EnableButtons();
            }
            _bestTxt.enabled = true;
            if (_playerManager._playerLevel < levelIndex + 1) _playerManager._playerLevel = levelIndex + 1;
            if (_playerManager._playerLevel >= _playerManager._totalLevels) _playerManager._playerLevel = _playerManager._totalLevels - 1;
            PlayerPrefs.SetInt("PlayerLevel", _playerManager._playerLevel);
            var timeString = _timer.ToString("F2");
            _timeTakenText.text = "Completed in: " + timeString + "s";
            _timeTakenText.enabled = true;
            _timeContainer.CheckTimes();
            _nextLevel.Select();
        }
        else
        {
            StartCoroutine(LevelReset());
        }
    }

    public IEnumerator LevelReset()
    {
        yield return new WaitForSeconds(2.0f);
        Restart();
    }

    void RankSwitcher(int index, float time)
    {
        if (time <= _timeContainer._levelTimes[index]._S_time)
        {
            _rankImage.sprite = _rankSsprite;
            _rankText.text = "S";
            _UIanim.SetInteger("Class", 3);
        }
        else if (time <= _timeContainer._levelTimes[index]._A_time)
        {
            _rankImage.sprite = _rankAsprite;
            _rankText.text = "A";
            _UIanim.SetInteger("Class", 2);
        }
        else if (time <= _timeContainer._levelTimes[index]._B_time)
        {
            _rankImage.sprite = _rankBsprite;
            _rankText.text = "B";
            _UIanim.SetInteger("Class", 1);
        }
        else
        {
            _rankImage.sprite = _rankCsprite;
            _rankText.text = "C";
            _UIanim.SetInteger("Class", 0);
        }        
    }

    public void ChangeScene()
    {
        _playerManager._skipscreen = true;
        SceneManager.LoadScene(1);        
    }

    public void Restart()
    {
        if (!_inMenu) AnalyticsData(false);
        SceneManager.LoadScene(2);
    }

    public void EnableButtons()
    {
        _restart.interactable = true;
        _mainMenu.interactable = true;
        _nextLevel.interactable = true;
    }

    public void NextLevel()
    {
        if (_timer < _playerManager._times[levelIndex] || (_playerManager._times[levelIndex] == 0.0f))
        {
            _ghosts.SaveGhost();
        }
        if (levelIndex < _playerManager._totalLevels)
        {
            _playerManager._levelIndex++;
            SceneManager.LoadScene(2);
        }
        else {
            ChangeScene();
        }        
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void AnalyticsData(bool win)
    {
        if (win)
        {
            var best = Mathf.Min(_timer, _playerManager._times[levelIndex]);
            Analytics.CustomEvent("levelComplete", new Dictionary<string, object>
            {
                {"version", _playerManager._version},
                {"level", levelIndex},
                {"time", _timer},
                {"record", best}
            });
        }
        else
        {
            Analytics.CustomEvent("levelFailed", new Dictionary<string, object>
            {
                {"version", _playerManager._version},
                {"level", levelIndex},
                {"position", _playerAnim.transform.eulerAngles},
                {"time", _timer}
            });
        }        
    }
}
