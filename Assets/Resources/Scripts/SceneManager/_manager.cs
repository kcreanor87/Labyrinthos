using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class _manager : MonoBehaviour {

    public bool _developmentMode;
    public GameObject _playerManagerPrefab;

    public float _timer;
    public float _maxTime = 30.0f;
    public float _best;
    public float _countdown = 1.2f;

    public int _cratesRemaining;
    public int buildIndex;
    public string build;
    
    public bool _inMenu;   
    public bool _gameOver;

    public GameObject _winScreen;
    public GameObject _loseScreen;    

    public Transform _joystick;

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
    public Animator _joystickAnim;
    public Animator _playerAnim;

    //Skip function variables
    public GameObject _skipScreen;
    public GameObject _shopScreen;
    public Button _skipButton;
    public Text _skipText;
    public bool _paused;
    public bool _tooltipActive;

    private void Awake()
    {
        if (GameObject.Find("_playerManager") == null && !_developmentMode) SceneManager.LoadScene(0);
        if (_developmentMode)
        {
            GameObject _pmPrefab = Instantiate(_playerManagerPrefab) as GameObject;
            _pmPrefab.name = "_playerManager";
        }
        
    }

    // Use this for initialization
    void Start () {
        _skipScreen = GameObject.Find("SkipScreen");
        _shopScreen = GameObject.Find("ShopScreen");
        _skipButton = GameObject.Find("SkipBtn").GetComponent<Button>();
        _skipText = GameObject.Find("SkipsRemaining").GetComponent<Text>();
        _skipScreen.SetActive(false);
        _shopScreen.SetActive(false);
        SpawnCrates();
        _playerAnim = GameObject.Find("Player").GetComponent<Animator>();
        _UIanim = gameObject.GetComponent<Animator>();
        _joystickAnim = GameObject.Find("VirtualJoystick").GetComponent<Animator>();
        _joystick = _joystickAnim.transform.Find("MobileJoystick");
        _timeContainer = GameObject.Find("_playerManager").GetComponent<LevelTimeContainer>();
        _rankImage = GameObject.Find("Rank").GetComponent<Image>();
        _rankText = GameObject.Find("RankText").GetComponent<Text>();
        _countdown = 2.0f;
        _timer = 0.0f;
        _inMenu = true;
        _bestTxt = GameObject.Find("BestTimeTxt").GetComponent<Text>();
        _recordTxt = GameObject.Find("RecordTxt").GetComponent<Text>();
        buildIndex = (SceneManager.GetActiveScene().buildIndex - 2);
        build = (_playerManager._times[buildIndex] == 0.0f) ? "--:--" : _playerManager._times[buildIndex].ToString("F2");
        _bestTxt.text = "Record: " + build + " s";
        _recordTxt.text = build + " s";
        _levelTxt = GameObject.Find("LevelTxt").GetComponent<Text>();
        _levelTxt.text = "Level " + (SceneManager.GetActiveScene().buildIndex - 1);
        _levelTxt.enabled = false;
        _winScreen = GameObject.Find("GameOver_win");
        _winScreen.SetActive(false);
        _timeTakenText = GameObject.Find("TimeTakenTxt").GetComponent<Text>();
        _timeTakenText.enabled = false;
        _timerTxt = GameObject.Find("TimerTxt").GetComponent<Text>();
        _cratesRemainingTxt = GameObject.Find("CratesRemainingTxt").GetComponent<Text>();
        _cratesRemaining = GameObject.FindGameObjectsWithTag("Crate").Length;
        _cratesRemainingTxt.text = _cratesRemaining.ToString();
        _timerTxt.text = _timer.ToString("F2");
        _ghosts = gameObject.GetComponent<Ghosts>();  
}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (_inMenu && !_gameOver)
        {
            Countdown();
            return;
        }
        if (!_gameOver && !_paused) UpdateTimer();
        if (Input.GetKeyDown(KeyCode.W)){ EndLevel(true);}
    }

    void UpdateTimer()
    {
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

    public void SpawnCrates()
    {
        var collectables = GameObject.FindGameObjectsWithTag("Collectable");
        foreach (GameObject collectable in collectables)
        {
            Instantiate(Resources.Load("Prefabs/Collectables/Collectable_base01") as GameObject, collectable.transform);
        }        
    }

    public void UpdateCrates()
    {
        _cratesRemaining = GameObject.FindGameObjectsWithTag("Crate").Length-1;
        _cratesRemainingTxt.text = _cratesRemaining.ToString();
        if (_cratesRemaining == 0) EndLevel(true);
    }

    public IEnumerator CloseCountdown()
    {
        yield return new WaitForSeconds(1.2f);
        //_countdownTxt.enabled = false;
    }

    public void EndLevel(bool victory)
    {
        _playerAnim.SetBool("Outro", true);
        _joystick.GetComponent<Image>().enabled = false;
        _UIanim.SetBool("Complete", true);
        _UIanim.SetBool("Victory", victory);
        _joystickAnim.SetBool("Complete", true);        
        _levelTxt.enabled = true;
        _inMenu = true;
        _gameOver = true;
        AnalyticsData(victory);
        if (victory)
        {
            RankSwitcher(buildIndex, _timer);
            if (_timer < _playerManager._times[buildIndex] || (_playerManager._times[buildIndex] == 0.0f))
            {
                _playerManager._times[buildIndex] = _timer;
                _bestTxt.text = "New Record: " + _timer.ToString("F2") + "s";
                _bestTxt.color = Color.green;
                _playerManager.SaveTimes();
                _ghosts.SaveGhost();
            }
            _bestTxt.enabled = true;
            if (_playerManager._playerLevel < SceneManager.GetActiveScene().buildIndex - 1) _playerManager._playerLevel = (SceneManager.GetActiveScene().buildIndex - 1);
            if (_playerManager._playerLevel >= _playerManager._totalLevels) _playerManager._playerLevel = _playerManager._totalLevels - 1;
            PlayerPrefs.SetInt("PlayerLevel", _playerManager._playerLevel);
            var timeString = _timer.ToString("F2");
            _timeTakenText.text = "Completed in: " + timeString + "s";
            _timeTakenText.enabled = true;
            _timeContainer.CheckTimes();
        }
        else
        {
            StartCoroutine(LevelReset());
        }

    }

    public void SkipLevel()
    {
        if (_playerManager._playerLevel >= SceneManager.GetActiveScene().buildIndex - 1)
        {
            NextLevel();
        }
        else
        {
            _joystickAnim.SetBool("Complete", true);
            _UIanim.SetBool("Complete", true);
            _skipScreen.SetActive(true);
            _skipButton.interactable = _playerManager._skips > 0;
            _skipText.text = _playerManager._skips.ToString();
            _paused = true;
        }
        //if (_playerManager._playerLevel < SceneManager.GetActiveScene().buildIndex - 1) _playerManager._playerLevel = (SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void Back(bool shop)
    {
        if (shop)
        {
            _shopScreen.SetActive(false);
            _skipScreen.SetActive(true);   
            _skipText.text = _playerManager._skips.ToString();
            _skipButton.interactable = _playerManager._skips > 0;
        }
        else
        {
            _joystickAnim.SetBool("Complete", false);
            _UIanim.SetBool("Complete", false);
            _skipScreen.SetActive(false);
            _paused = false;
        }        
    }

    public void AddSkips(int i)
    {
        _playerManager._skips += i;
        Back(true);
    }

    public void Skip()
    {
        //IAP MANAGER
        _playerManager._playerLevel = (SceneManager.GetActiveScene().buildIndex - 1);
        _playerManager._skips--;
        _playerManager.SaveTimes();
        PlayerPrefs.SetInt("PlayerLevel", _playerManager._playerLevel);
        Analytics.CustomEvent("levelSkipped", new Dictionary<string, object>
            {
                {"version", _playerManager._version},
                {"level", buildIndex}
            });
        NextLevel();
    }

    public void OpenShop()
    {
        _skipScreen.SetActive(false);
        _shopScreen.SetActive(true);
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

    public void ChangeScene(int i)
    {
        _playerManager._skipscreen = true;
        SceneManager.LoadScene(i + 1);
    }

    public void Restart()
    {
        if (!_inMenu) AnalyticsData(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        if ((SceneManager.GetActiveScene().buildIndex - 2) <= _playerManager._totalLevels)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else {
            ChangeScene(0);
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
            var best = Mathf.Min(_timer, _playerManager._times[buildIndex]);
            Analytics.CustomEvent("levelComplete", new Dictionary<string, object>
            {
                {"version", _playerManager._version},
                {"level", buildIndex},
                {"time", _timer},
                {"record", best}
            });
        }
        else
        {
            Analytics.CustomEvent("levelFailed", new Dictionary<string, object>
            {
                {"version", _playerManager._version},
                {"level", buildIndex},
                {"position", _ghosts._sphere.eulerAngles},
                {"time", _timer}
            });
        }        
    }
}
