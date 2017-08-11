using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

public class _CoopManager : MonoBehaviour {

    public bool _developmentMode;
    public GameObject _playerManagerPrefab;

    public float _timer;
    public float _maxTime = 30.0f;
    public float _best;
    public float _countdown = 1.2f;

    public int _cratesRemaining_P1;
    public int _cratesRemaining_P2;
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
        SpawnCrates();
        GameObjectFinder();
        _pauseScreen.SetActive(false);
        _rotCam.SetActive(false);
        _countdown = 2.0f;
        _timer = 0.0f;
        _inMenu = true;
        build = (_playerManager._times[levelIndex] == 0.0f) ? "--:--" : _playerManager._times[levelIndex].ToString("F2");
        _winScreen.SetActive(false);
        _timeTakenText.enabled = false;
        _cratesRemainingTxt.text = _cratesRemaining_P1.ToString();
        _timerTxt.text = _timer.ToString("F2");
        _nextLevel.Select();
    }

    void SpawnLevel()
    {
        levelIndex = _playerManager._levelIndex;
        var sector = Mathf.Floor(levelIndex / 9) + 1;
        var level = levelIndex - ((sector - 1) * 9);
        var path = "Prefabs/2P_Worlds/Sc0" + sector + "/pfbWorldSc0" + sector + "_0" + (level + 1) + "_2P";
        print(path);
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
            Instantiate(Resources.Load("Prefabs/Collectables/Collectable_Coop01") as GameObject, collectable.transform);
            Instantiate(Resources.Load("Prefabs/Collectables/Collectable_Coop02") as GameObject, collectable.transform);
        }
    }

    void GameObjectFinder()
    {
        //_playerAnim = GameObject.Find("Player").GetComponent<Animator>();
        _timeContainer = GameObject.Find("_playerManager").GetComponent<LevelTimeContainer>();
        _winScreen = GameObject.Find("GameOver_win");
        _cratesRemainingTxt = GameObject.Find("CratesRemainingTxt").GetComponent<Text>();
        _cratesRemaining_P1 = GameObject.FindGameObjectsWithTag("Crate0").Length;
        _cratesRemaining_P2 = GameObject.FindGameObjectsWithTag("Crate1").Length;
        _timeTakenText = GameObject.Find("TimeTakenTxt").GetComponent<Text>();
        _winScreen = GameObject.Find("GameOver_win");
        _timerTxt = GameObject.Find("TimerTxt").GetComponent<Text>();
        _pauseScreen = GameObject.Find("PauseMenu");
        _rotCam = GameObject.Find("RotationCam");
        _UIanim = gameObject.GetComponent<Animator>();
        _nextLevel = GameObject.Find("NextLevel").GetComponent<Button>();
        _resume = GameObject.Find("Resume").GetComponent<Button>();
    }

	void FixedUpdate () {
        if (_inMenu && !_gameOver && !_paused)
        {
            Countdown();
            return;
        }
        if (!_gameOver && !_paused) UpdateTimer();
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
            _inMenu = false;
        }
    }   

    public void UpdateCrates(int playerIndex)
    {
        if (playerIndex == 0) _cratesRemaining_P1--;
        if (playerIndex == 1) _cratesRemaining_P2--;
        _cratesRemainingTxt.text = _cratesRemaining_P1.ToString();
        if (_cratesRemaining_P1 == 0) EndLevel(0);
        if (_cratesRemaining_P2 == 0) EndLevel(1);
    }

    public void PauseGame(bool active)
    {
        _paused = active;
        _pauseScreen.SetActive(_paused);
        _rotCam.SetActive(_paused);
        _inMenu = _paused;
        _resume.Select();
    }

    public void EndLevel(int victor)
    {
        //_playerAnim.SetBool("Outro", true);
        _UIanim.SetBool("Complete", true);
        _UIanim.SetBool("Victory", true);
        _inMenu = true;
        _gameOver = true;
        var timeString = _timer.ToString("F2");
        _timeTakenText.text = "Completed in: " + timeString + "s";
        _timeTakenText.enabled = true;
        _nextLevel.Select();
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
        _playerManager._skipscreen = true;
    }

    public void NextLevel()
    {
        if (levelIndex < _playerManager._totalLevels)
        {
            _playerManager._levelIndex++;
            SceneManager.LoadScene(3);
        }
        else {
            ChangeScene();
        }        
    }
}
