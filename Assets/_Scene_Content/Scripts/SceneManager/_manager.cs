using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

public class _manager : MonoBehaviour {

    public bool _developmentMode;

    public float _timer;
    public float _best;
    public float _countdown = 0.5f;

    public int _cratesRemaining;
    public int levelIndex;
    public string build;
    
    public bool _inMenu;   
    public bool _gameOver;

    public GameObject _winScreen;
    public GameObject _pauseScreen;
    public GameObject _rotCam;
    public GameObject _playerCol;
    public GameObject _playerManagerPrefab;
    public GameObject _rankFX;

    public Text _timeTakenText;
    public Text _recordTxt;
    public Text _bestTxt;
    public Text _timerTxt;
    public Text _cratesRemainingTxt;
    
    public Image _rankImage;
    public Sprite _rankSsprite, _rankAsprite, _rankBsprite, _rankCsprite;
    public LevelTimeContainer _timeContainer;
    public Ghosts _ghosts;

    public Animator _UIanim;
    public Animator _playerAnim;
    public Animator _gameOverPrompt;
    
    public bool _paused;
    public bool _saving;
    public bool _ending;

    public Color _newRecordColour;    

    private void Awake()
    {
        if (GameObject.Find("_playerManager") == null && !_developmentMode) SceneManager.LoadScene(0);
        if (_developmentMode)
        {
            GameObject _pmPrefab = Instantiate(_playerManagerPrefab) as GameObject;
            _pmPrefab.name = "_playerManager";
        }
        else
        {
            SpawnLevel();
        }

    }

    // Use this for initialization
    void Start()
    {                
        GameObjectFinder();
        SpawnCrates();

        _cratesRemaining = GameObject.FindGameObjectsWithTag("Crate").Length;
        _cratesRemainingTxt.text = _cratesRemaining.ToString();

        _pauseScreen.SetActive(false);        
        _rotCam.SetActive(false);
        _winScreen.SetActive(false);
        _gameOverPrompt.gameObject.SetActive(false);
        _rankFX.SetActive(false);

        _countdown = 0.9f;
        _timer = 0.0f;
        _inMenu = true;

        build = (_playerManager._times[levelIndex] == 0.0f) ? "--:--" : _playerManager._times[levelIndex].ToString("F2");
        _bestTxt.text = build + " s";
        _recordTxt.text = build + " s";
        _timerTxt.text = _timer.ToString("F2");

        _bestTxt.enabled = false;
        _timeTakenText.enabled = false;
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
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    void SpawnCrates()
    {
        var collectables = GameObject.FindGameObjectsWithTag("Collectable");
        foreach (GameObject collectable in collectables)
        {
            Instantiate(Resources.Load("Prefabs/Collectables/Collectable_base01") as GameObject, collectable.transform);
            print(collectable.transform.parent.parent.name);
        }
    }

    void GameObjectFinder()
    {
        _playerAnim = GameObject.Find("Player").GetComponent<Animator>();
        _playerCol = _playerAnim.transform.Find("Collider").gameObject;
        _timeContainer = GameObject.Find("_playerManager").GetComponent<LevelTimeContainer>();
        _rankImage = GameObject.Find("Rank").GetComponent<Image>();
        _bestTxt = GameObject.Find("BestTimeTxt").GetComponent<Text>();
        _recordTxt = GameObject.Find("RecordTxt").GetComponent<Text>();
        _winScreen = GameObject.Find("GameOver_win");
        _cratesRemainingTxt = GameObject.Find("CratesRemainingTxt").GetComponent<Text>();        
        _timeTakenText = GameObject.Find("TimeTakenTxt").GetComponent<Text>();
        _winScreen = GameObject.Find("GameOver_win");
        _timerTxt = GameObject.Find("TimerTxt").GetComponent<Text>();
        _pauseScreen = GameObject.Find("PauseMenu");
        _rotCam = GameObject.Find("RotationCam");
        _UIanim = gameObject.GetComponent<Animator>();
        _ghosts = gameObject.GetComponent<Ghosts>();
        _gameOverPrompt = GameObject.Find("GameOverPrompt").GetComponent<Animator>();
        _rankFX = GameObject.Find("RankFX");
        GameObject.Find("Prompt1").SetActive(_playerManager._levelIndex == 0);
        GameObject.Find("Prompt2").SetActive(_playerManager._levelIndex == 1 || _playerManager._levelIndex == 4);
    }

	void FixedUpdate () {
        if (_inMenu && !_gameOver && !_paused)
        {
            Countdown();
            return;
        }
        if (!_ending && !_paused) UpdateTimer();
        if (Input.GetButtonDown("Restart")) Restart();              
    }

    private void Update()
    {
        InputManager();
        if (_paused)
        {
            PausedInput();
        }
        if (_ending && !_saving)
        {
            EndGameInput();
        }
    }

    void InputManager()
    {
        if (Input.GetButtonDown("Pause") && !_gameOver && !_ending) PauseGame(!_paused);
    }

    void UpdateTimer()
    {
        if (_paused) return;
        _timer += Time.deltaTime;
        _timerTxt.text = _timer.ToString("F2");
    }

    void Countdown()
    {
        _countdown -= Time.deltaTime;
        if (_countdown <= 0.01f)
        {
            if (!_developmentMode) _ghosts.StartGhost();            
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
    }

    public void EndLevel(bool victory)
    {
        _ending = true;
        _playerAnim.SetBool("Outro", true);
        AnalyticsData(victory);
        _playerCol.SetActive(false);        
        if (victory)
        {
            _gameOver = true;
            _gameOverPrompt.gameObject.SetActive(true);
            RankSwitcher(levelIndex, _timer);
            Camera.main.GetComponent<Animator>().enabled = true;
            if (_timer < _playerManager._times[levelIndex] || (_playerManager._times[levelIndex] == 0.0f))
            {
                if (_timer <= _timeContainer._levelTimes[levelIndex]._C_time)
                {
                    _saving = true;
                    _gameOverPrompt.SetBool("Saving", true);
                    _playerManager._times[levelIndex] = _timer;
                    _playerManager.SaveTimes();
                    _bestTxt.text = _timer.ToString("F2") + "s";
                    _bestTxt.color = _newRecordColour;
                    _rankFX.SetActive(true);
                    _ghosts.SaveGhost();
                }
            }
            else
            {
                _gameOverPrompt.SetBool("Saving", false);
            }
            _bestTxt.enabled = true;
            if (_playerManager._playerLevel < levelIndex + 1) _playerManager._playerLevel = levelIndex + 1;
            if (_playerManager._playerLevel >= _playerManager._totalLevels) _playerManager._playerLevel = _playerManager._totalLevels - 1;
            PlayerPrefs.SetInt("PlayerLevel", _playerManager._playerLevel);
            var timeString = _timer.ToString("F2");
            _timeTakenText.text = timeString + "s";
            _timeTakenText.enabled = true;
            _timeContainer.CheckTimes();
            StartCoroutine(WaitForEnding());
        }
        else
        {
            StartCoroutine(LevelReset());
        }
    }

    public IEnumerator WaitForEnding()
    {
        yield return new WaitForSeconds(1.5f);        
        _UIanim.SetBool("Complete", true);
        _UIanim.SetBool("Victory", true);
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
            _UIanim.SetInteger("Class", 3);
        }
        else if (time <= _timeContainer._levelTimes[index]._A_time)
        {
            _rankImage.sprite = _rankAsprite;
            _UIanim.SetInteger("Class", 2);
        }
        else if (time <= _timeContainer._levelTimes[index]._B_time)
        {
            _rankImage.sprite = _rankBsprite;
            _UIanim.SetInteger("Class", 1);
        }
        else
        {
            _rankImage.sprite = _rankCsprite;
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

    public void SaveTimes()
    {
        _ghosts.SaveGhost();
    }

    public void NextLevel()
    {
        if (levelIndex < _playerManager._totalLevels)
        {
            _playerManager._levelIndex++;
            SceneManager.LoadScene(2);
        }
        else {
            ChangeScene();
        }        
    }

    public void EndGameInput()
    {
        if (Input.GetButtonDown("Submit"))
        {
            NextLevel();
        }
        if (Input.GetButtonDown("Restart"))
        {
            Restart();
        }
        if (Input.GetButtonDown("MainMenu"))
        {
            ChangeScene();
        }
    }

    public void PausedInput()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseGame(false);
            _gameOverPrompt.SetBool("Pressed", true);
        }
        if (Input.GetButtonDown("Restart"))
        {
            Restart();
            _gameOverPrompt.SetBool("Pressed", true);
        }
        if (Input.GetButtonDown("MainMenu"))
        {
            ChangeScene();
            _gameOverPrompt.SetBool("Pressed", true);
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
