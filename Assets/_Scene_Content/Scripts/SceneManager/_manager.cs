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
    public int _sector;
    public string build;
    
    public bool _inMenu;   
    public bool _gameOver;

    public GameObject _winScreen;
    public GameObject _pauseScreen;
    public GameObject _rotCam;
    public GameObject _playerCol;
    public GameObject _playerManagerPrefab;
    public GameObject _rankFX;
    public GameObject _fadeOut;

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
    public Animator _rankSwitcher;

    public List<int> _tooltip1Index = new List<int>();
    public List<int> _tooltip2Index = new List<int>();
    public List<int> _tooltip3Index = new List<int>();

    public List<Color> _BGColours = new List<Color>();

    public bool _paused;
    public bool _saving;
    public bool _ending;
    public bool _endActive;
    public bool _loss;

    public Color _newRecordColour;    

    private void Awake()
    {
        //If not in development mode, load game from start
        if (GameObject.Find("_playerManager") == null && !_developmentMode) SceneManager.LoadScene(0);
        //Development mode for level testing
        if (_developmentMode)
        {
            //Create blank _playerManager object
            GameObject _pmPrefab = Instantiate(_playerManagerPrefab) as GameObject;
            _pmPrefab.name = "_playerManager";
        }
        else
        {
            SpawnLevel();
        }
    }
    
    void Start()
    {                
        GameObjectFinder();
        SpawnCrates();

        _cratesRemaining = GameObject.FindGameObjectsWithTag("Crate").Length;
        _cratesRemainingTxt.text = _cratesRemaining.ToString();

        //Set initial states
        _countdown = 0.9f;
        _timer = 0.0f;
        _inMenu = true;
        build = (_playerManager._times[levelIndex] == 0.0f) ? "--:--" : _playerManager._times[levelIndex].ToString("F2");
        _fadeOut.GetComponent<Image>().color = _BGColours[_sector - 1];

        //Diable GO's not in use yet
        _pauseScreen.SetActive(false);        
        _rotCam.SetActive(false);
        _winScreen.SetActive(false);
        _gameOverPrompt.gameObject.SetActive(false);
        _rankFX.SetActive(false);
        _fadeOut.SetActive(false);
        _rankSwitcher.gameObject.SetActive(false);

        //Initiate end times and disable
        _bestTxt.text = build + " s";
        _recordTxt.text = build + " s";
        _timerTxt.text = _timer.ToString("F2");
        _bestTxt.enabled = false;
        _timeTakenText.enabled = false;
    }

    void SpawnLevel()
    {
        //Calculate Sector and level, and subsequent level diectory
        levelIndex = _playerManager._levelIndex;
        _sector = Mathf.FloorToInt(levelIndex / 9) + 1;
        var level = levelIndex - ((_sector - 1) * 9);
        var path = "Prefabs/Worlds/Sc0" + _sector + "/pfbWorldSc0" + _sector + "_0" + (level + 1);
        //Instantitate level and clear values
        GameObject World = Instantiate(Resources.Load(path, typeof (GameObject))) as GameObject;
        World.transform.parent = GameObject.Find("World001Container").GetComponent<Transform>();
        World.transform.localPosition = new Vector3(0,0,0);
        World.transform.localScale = new Vector3(1,1,1);
        //Cleanup
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Background"))
        {
            obj.GetComponent<MeshRenderer>().material.color = _BGColours[Mathf.FloorToInt(_sector) - 1];
        }
    }

    void SpawnCrates()
    {
        //Spawn Crates
        var collectables = GameObject.FindGameObjectsWithTag("Collectable");
        foreach (GameObject collectable in collectables)
        {
            Instantiate(Resources.Load("Prefabs/Collectables/Collectable_base01") as GameObject, collectable.transform);
        }
    }

    void GameObjectFinder()
    {
        //Get all objects referenced
        _playerAnim = GameObject.Find("Player").GetComponent<Animator>();
        _playerCol = _playerAnim.transform.Find("Collider").gameObject;
        _timeContainer = GameObject.Find("_playerManager").GetComponent<LevelTimeContainer>();
        _rankImage = GameObject.Find("Rank").GetComponent<Image>();
        _bestTxt = GameObject.Find("BestTimeTxt").GetComponent<Text>();
        _recordTxt = GameObject.Find("RecordTxt").GetComponent<Text>();
        _fadeOut = GameObject.Find("FadeOut");        
        _winScreen = GameObject.Find("GameOver_win");
        _cratesRemainingTxt = GameObject.Find("CratesRemainingTxt").GetComponent<Text>();        
        _timeTakenText = GameObject.Find("TimeTakenTxt").GetComponent<Text>();
        _timerTxt = GameObject.Find("TimerTxt").GetComponent<Text>();
        _pauseScreen = GameObject.Find("PauseMenu");
        _rotCam = GameObject.Find("RotationCam");
        _UIanim = gameObject.GetComponent<Animator>();
        _ghosts = gameObject.GetComponent<Ghosts>();
        _gameOverPrompt = GameObject.Find("GameOverPrompt").GetComponent<Animator>();
        _rankSwitcher = GameObject.Find("Rank").GetComponent<Animator>();
        _rankFX = GameObject.Find("RankFX");
        GameObject.Find("Prompt1").SetActive(_tooltip1Index.Contains(levelIndex));
        GameObject.Find("Prompt2").SetActive(_tooltip2Index.Contains(levelIndex));
        GameObject.Find("Prompt3").SetActive(_tooltip3Index.Contains(levelIndex));
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
        if (_ending && !_endActive)
        {
            EndGameInput();
        }
    }

    void InputManager()
    {
        if (Input.GetButtonDown("Pause") && !_gameOver && !_ending) PauseGame(!_paused);
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
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
        _loss = !victory;
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
                if (_timer <= 300.0f)
                {
                    _saving = true;
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
            if (_playerManager._playerLevel > _playerManager._totalLevels) _playerManager._playerLevel = _playerManager._totalLevels;
            PlayerPrefs.SetInt("PlayerLevel", _playerManager._playerLevel);
            var timeString = _timer.ToString("F2");
            _timeTakenText.text = timeString + "s";
            _timeTakenText.enabled = true;
            _timeContainer.CheckTimes();
            StartCoroutine(WaitForEnding());
            
        }
        else
        {
            if ((_playerManager._times[levelIndex] == 0.0f))
            {
                _saving = true;
                _ghosts.SaveGhost();                
            }
            StartCoroutine(LevelReset());
        }
    }

    public IEnumerator WaitForEnding()
    {
        yield return new WaitForSeconds(0.5f);
        _UIanim.SetBool("Complete", true);
        _UIanim.SetBool("Victory", true);
    }

    public IEnumerator LevelReset()
    {
        yield return new WaitForSeconds(1.0f);
        Restart();
    }

    void RankSwitcher(int index, float time)
    {
        _rankSwitcher.gameObject.SetActive(true);
        if (time <= _timeContainer._levelTimes[index]._S_time)
        {
            _rankSwitcher.SetInteger("Rank", 3);
        }
        else if (time <= _timeContainer._levelTimes[index]._A_time)
        {
            _rankSwitcher.SetInteger("Rank", 2);
        }
        else if (time <= _timeContainer._levelTimes[index]._B_time)
        {
            _rankSwitcher.SetInteger("Rank", 1);
        }
        else
        {
            _rankSwitcher.SetInteger("Rank", 0);
        }        
    }

    public void ChangeScene()
    {
        _playerManager._skipscreen = true;
        StartCoroutine(Switcher(1));
    }

    public void Restart()
    {
        if (!_inMenu) AnalyticsData(false);
        StartCoroutine(Switcher(2));

    }

    public void NextLevel()
    {
        if (levelIndex < _playerManager._totalLevels)
        {
            _playerManager._levelIndex++;
            StartCoroutine(Switcher(2));
        }
        else {
            ChangeScene();
        }        
    }

    public IEnumerator Switcher(int sceneIndex)
    {
        _fadeOut.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(sceneIndex);
    }

    public void EndGameInput()
    {
        if (_saving || _loss) return;
        if (Input.GetButtonDown("Submit"))
        {
            NextLevel();
            _endActive = true;
        }
        if (Input.GetButtonDown("Restart"))
        {
            Restart();
            _endActive = true;
        }
        if (Input.GetButtonDown("MainMenu"))
        {
            ChangeScene();
            _endActive = true;
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
