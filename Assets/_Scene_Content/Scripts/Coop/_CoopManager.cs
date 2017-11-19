using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

public class _CoopManager : MonoBehaviour {

    public bool _developmentMode;    

    public float _timer;
    public float _best;
    public float _countdown = 0.5f;

    public int _cratesRemaining_P1;
    public int _cratesRemaining_P2;
    public int levelIndex;
    public int _sector;
    public string build;
    
    public bool _inMenu;   
    public bool _gameOver;

    public GameObject _winScreen;
    public GameObject _pauseScreen;
    public GameObject _P1rotCam;
    public GameObject _P2rotCam;
    public GameObject _player1Col;
    public GameObject _player2Col;
    public GameObject _playerManagerPrefab;
    public GameObject _gameOverPanel;
    
    public Text _P1timerTxt;
    public Text _P2timerTxt;
    public Text _P1cratesRemainingTxt;
    public Text _P2cratesRemainingTxt;
    public Text _P1Score;
    public Text _P2Score;
    public Text _rankText;  
    
    public Image _rankImage;
    public Sprite _rankSsprite, _rankAsprite, _rankBsprite, _rankCsprite;
    public LevelTimeContainer _timeContainer;

    public Animator _UIanim;
    public Animator _player1Anim;
    public Animator _player2Anim;
    public Animator _gameOverPrompt;
    
    public bool _paused;
    public bool _ending;
    public bool _switching;

    public List<Color> _BGColours = new List<Color>();

    public List<int> _exclusions = new List<int>();

    private void Awake()
    {
        if (GameObject.Find("_playerManager") == null && !_developmentMode) SceneManager.LoadScene(0);
        else if (_developmentMode && GameObject.Find("_playerManager") == null)
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
        _cratesRemaining_P1 = GameObject.FindGameObjectsWithTag("Crate0").Length;
        _cratesRemaining_P2 = GameObject.FindGameObjectsWithTag("Crate1").Length;

        _pauseScreen.SetActive(false);
        _P1rotCam.SetActive(false);
        _P2rotCam.SetActive(false);
        _winScreen.SetActive(false);
        _gameOverPrompt.gameObject.SetActive(false);
        _gameOverPanel.SetActive(false);

        _countdown = 1.5f;
        _timer = 0.0f;
        _inMenu = true;

        build = (_playerManager._times[levelIndex] == 0.0f) ? "--:--" : _playerManager._times[levelIndex].ToString("F2");
        _winScreen.SetActive(false);
        
        _P1timerTxt.text = _timer.ToString("F2");
        _P2timerTxt.text = _timer.ToString("F2");
    }

    void SpawnLevel()
    {
        //Calculate Sector and level, and subsequent level diectory
        levelIndex = _playerManager._levelIndex;
        _sector = Mathf.FloorToInt(levelIndex / 9) + 1;
        var level = levelIndex - ((_sector - 1) * 9);
        var path = "Prefabs/Worlds/Sc0" + _sector + "/pfbWorldSc0" + _sector + "_0" + (level + 1);
        //Instantitate level and clear values
        GameObject World = Instantiate(Resources.Load(path, typeof(GameObject))) as GameObject;
        World.transform.parent = GameObject.Find("World001Container").GetComponent<Transform>();
        World.transform.localPosition = new Vector3(0, 0, 0);
        World.transform.localScale = new Vector3(1, 1, 1);
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
        var collectables = GameObject.FindGameObjectsWithTag("Collectable");
        foreach (GameObject collectable in collectables)
        {
            Instantiate(Resources.Load("Prefabs/Collectables/Collectable_Coop01") as GameObject, collectable.transform);
            Instantiate(Resources.Load("Prefabs/Collectables/Collectable_Coop02") as GameObject, collectable.transform);
        }
    }

    void GameObjectFinder()
    {
        _player1Anim = GameObject.Find("Player1").GetComponent<Animator>();
        _player2Anim = GameObject.Find("Player2").GetComponent<Animator>();
        _player1Col = _player1Anim.transform.Find("Collider").gameObject;
        _player2Col = _player2Anim.transform.Find("Collider").gameObject;
        _timeContainer = GameObject.Find("_playerManager").GetComponent<LevelTimeContainer>();
        _winScreen = GameObject.Find("GameOver_win");
        _P1cratesRemainingTxt = GameObject.Find("P1CratesRemainingTxt").GetComponent<Text>();
        _P2cratesRemainingTxt = GameObject.Find("P2CratesRemainingTxt").GetComponent<Text>();
        _P1cratesRemainingTxt.text = _playerManager._P1Score.ToString();
        _P2cratesRemainingTxt.text = _playerManager._P2Score.ToString();
        _P1Score = GameObject.Find("P1Score").GetComponent<Text>();
        _P2Score = GameObject.Find("P2Score").GetComponent<Text>();
        _winScreen = GameObject.Find("GameOver_win");
        _P1timerTxt = GameObject.Find("P1TimerTxt").GetComponent<Text>();
        _P2timerTxt = GameObject.Find("P2TimerTxt").GetComponent<Text>();
        _pauseScreen = GameObject.Find("PauseMenu");
        _P1rotCam = GameObject.Find("P1RotationCam");
        _P2rotCam = GameObject.Find("P2RotationCam");
        _UIanim = gameObject.GetComponent<Animator>();
        _gameOverPrompt = GameObject.Find("GameOverPrompt").GetComponent<Animator>();
        _gameOverPanel = GameObject.Find("GameOverPanel");
    }

	void FixedUpdate () {
        if (_inMenu && !_gameOver && !_paused)
        {
            Countdown();
            return;
        }
        if (!_ending && !_paused) UpdateTimer();
        //if (Input.GetKeyDown(KeyCode.W)) { EndLevel(0);}
        if (_paused)
        {
            PausedInput();
        }
        if (_gameOver && !_switching)
        {
            EndGameInput();
        }
    }

    private void Update()
    {
        InputManager();
    }

    void InputManager()
    {
        if (Input.GetButtonDown("Pause") && !_ending && !_gameOver) PauseGame(!_paused);
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    void UpdateTimer()
    {
        if (_paused || _ending) return;
        _timer += Time.deltaTime;
        _P1timerTxt.text = _timer.ToString("F2");
        _P2timerTxt.text = _timer.ToString("F2");
    }

    void Countdown()
    {
        _countdown -= Time.deltaTime;
        if (_countdown <= 0.01f)
        {
            _inMenu = false;
        }
    }   

    public void UpdateCrates(int playerIndex)
    {
        if (playerIndex == 0)
        {
            _cratesRemaining_P1--;           
        }
        if (playerIndex == 1)
        {
            _cratesRemaining_P2--;
        }
        
        if (_cratesRemaining_P1 == 0 && !_ending) EndLevel(0);
        if (_cratesRemaining_P2 == 0 && !_ending) EndLevel(1);
    }

    public void PauseGame(bool active)
    {
        _paused = active;
        _pauseScreen.SetActive(_paused);
        _gameOverPanel.SetActive(_paused);
        _P1rotCam.SetActive(_paused);
        _P2rotCam.SetActive(_paused);
        _inMenu = _paused;
    }

    public void EndLevel(int victor)
    {
        if (victor == 0) _playerManager._P1Score++;
        else _playerManager._P2Score++;
        _ending = true;
        _gameOverPrompt.gameObject.SetActive(true);
        _gameOverPrompt.SetBool("Saving", false);
        _player1Anim.SetBool("Outro", true);
        _player2Anim.SetBool("Outro", true);
        _UIanim.SetBool("Complete", true);
        _UIanim.SetBool("Victory", true);        
        _playerManager._times[levelIndex] = _timer;
        _playerManager.SaveTimes();
        if (_playerManager._playerLevel < levelIndex + 1) _playerManager._playerLevel = levelIndex + 1;
        if (_playerManager._playerLevel > _playerManager._totalLevels) _playerManager._playerLevel = _playerManager._totalLevels;
        PlayerPrefs.SetInt("PlayerLevel", _playerManager._playerLevel);
        _timeContainer.CheckTimes();
        StartCoroutine(WaitForEnding());

    }

    public IEnumerator WaitForEnding()
    {
        yield return new WaitForSeconds(1.5f);
        _gameOverPanel.SetActive(true);
        _P1Score.text = _playerManager._P1Score.ToString();
        _P2Score.text = _playerManager._P2Score.ToString();
        _P1cratesRemainingTxt.text = _playerManager._P1Score.ToString();
        _P2cratesRemainingTxt.text = _playerManager._P2Score.ToString();
        _inMenu = true;
        _UIanim.SetBool("Complete", true);
        _UIanim.SetBool("Victory", true);
        _P1rotCam.SetActive(true);
        _P2rotCam.SetActive(true);
        _gameOver = true;
    }

    public void ChangeScene()
    {
        StartCoroutine(ChangeSceneTo(1));
    }

    public void NextLevel()
    {
        if (levelIndex < _playerManager._totalLevels)
        {
            _playerManager._levelIndex++;
            StartCoroutine(ChangeSceneTo(3));
        }
        else
        {
            ChangeScene();
        }        
    }

    public IEnumerator ChangeSceneTo(int index)
    {
        _switching = true;
        _playerManager._skipscreen = false;
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(index);
    }

    public void PausedInput()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseGame(false);
            _gameOverPrompt.SetBool("Pressed", true);
        }
        if (Input.GetButtonDown("MainMenu"))
        {
            ChangeScene();
            _gameOverPrompt.SetBool("Pressed", true);
        }
    }

    public void EndGameInput()
    {
        if (Input.GetButtonDown("Submit"))
        {
            NextLevel();
            _gameOverPrompt.SetBool("Pressed", true);
        }
        if (Input.GetButtonDown("MainMenu"))
        {
            ChangeScene();
            _gameOverPrompt.SetBool("Pressed", true);
        }
    }
}
