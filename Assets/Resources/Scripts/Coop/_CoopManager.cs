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
    public GameObject _pauseScreen;
    public GameObject _rotCam;
    public GameObject _player1Col;
    public GameObject _player2Col;
    public GameObject _playerManagerPrefab;

    public Text _timeTakenText;
    public Text _recordTxt;
    public Text _bestTxt;
    public Text _timerTxt;
    public Text _P1cratesRemainingTxt;
    public Text _P2cratesRemainingTxt;
    public Text _rankText;  
    
    public Image _rankImage;
    public Sprite _rankSsprite, _rankAsprite, _rankBsprite, _rankCsprite;
    public LevelTimeContainer _timeContainer;

    public Animator _UIanim;
    public Animator _player1Anim;
    public Animator _player2Anim;
    public Animator _gameOverPrompt;
    
    public bool _paused;
    public bool _tooltipActive;
    public bool _ending;
    
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
        _cratesRemaining_P1 = GameObject.FindGameObjectsWithTag("Crate0").Length;
        _cratesRemaining_P2 = GameObject.FindGameObjectsWithTag("Crate1").Length;
        _P1cratesRemainingTxt.text = _cratesRemaining_P1.ToString();
        _P2cratesRemainingTxt.text = _cratesRemaining_P2.ToString();

        _pauseScreen.SetActive(false);
        _rotCam.SetActive(false);
        _winScreen.SetActive(false);
        _gameOverPrompt.gameObject.SetActive(false);

        _countdown = 1.5f;
        _timer = 0.0f;
        _inMenu = true;

        build = (_playerManager._times[levelIndex] == 0.0f) ? "--:--" : _playerManager._times[levelIndex].ToString("F2");
        _winScreen.SetActive(false);
        _timeTakenText.enabled = false;
        
        _timerTxt.text = _timer.ToString("F2");
    }

    void SpawnLevel()
    {
        levelIndex = Random.Range(8, 17);
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
        _timeTakenText = GameObject.Find("TimeTakenTxt").GetComponent<Text>();
        _winScreen = GameObject.Find("GameOver_win");
        _timerTxt = GameObject.Find("TimerTxt").GetComponent<Text>();
        _pauseScreen = GameObject.Find("PauseMenu");
        _rotCam = GameObject.Find("RotationCam");
        _UIanim = gameObject.GetComponent<Animator>();
        _resume = GameObject.Find("Resume").GetComponent<Button>();
        _gameOverPrompt = GameObject.Find("GameOverPrompt").GetComponent<Animator>();
    }

	void FixedUpdate () {
        if (_inMenu && !_gameOver && !_paused)
        {
            Countdown();
            return;
        }
        if (!_gameOver && !_paused) UpdateTimer();
        if (Input.GetKeyDown(KeyCode.W)) { EndLevel(0);}
        if (_gameOver)
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
        if (Input.GetButtonDown("Pause") && !_tooltipActive && !_gameOver) PauseGame(!_paused);

        if (Input.GetButton("Pause"))
            print("joystick 1 detected");
        else if (Input.GetButton("Pause2"))
            print("joystick 2 detected");
        else if (Input.GetButton("Pause3"))
            print("joystick 3 detected");
        else if (Input.GetButton("Pause4"))
            print("joystick 4 detected");
        else if (Input.GetButton("Pause5"))
            print("joystick 5 detected");
        else if (Input.GetButton("Pause6"))
            print("joystick 6 detected");
        else if (Input.GetButton("Pause7"))
            print("joystick 7 detected");
        else if (Input.GetButton("Pause8"))
            print("joystick 8 detected");
        else if (Input.GetButton("Pause8"))
            print("joystick 8 detected");
        else if (Input.GetButton("Pause9"))
            print("joystick 8 detected");
        else if (Input.GetButton("Pause10"))
            print("joystick 8 detected");
        else if (Input.GetButton("Pause11"))
            print("joystick 8 detected");
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
        if (playerIndex == 0)
        {
            _cratesRemaining_P1--;
            _P1cratesRemainingTxt.text = _cratesRemaining_P1.ToString();
        }
        if (playerIndex == 1)
        {
            _P2cratesRemainingTxt.text = _cratesRemaining_P1.ToString();
            _cratesRemaining_P2--;
        }
        
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
        _player1Anim.SetBool("Outro", true);
        _player2Anim.SetBool("Outro", true);
        _UIanim.SetBool("Complete", true);
        _UIanim.SetBool("Victory", true);
        _inMenu = true;
        _gameOver = true;
        var timeString = _timer.ToString("F2");
        _timeTakenText.text = "Completed in: " + timeString + "s";
        _timeTakenText.enabled = true;
        StartCoroutine(WaitForEnding());
    }

    public IEnumerator WaitForEnding()
    {
        yield return new WaitForSeconds(1.5f);
        _UIanim.SetBool("Complete", true);
        _UIanim.SetBool("Victory", true);
        _gameOver = true;
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
        _playerManager._skipscreen = true;
    }

    public void NextLevel()
    {
        /*
        if (levelIndex < _playerManager._totalLevels)
        {
            _playerManager._levelIndex++;
            SceneManager.LoadScene(3);
        }
        else {
            ChangeScene();
        }   
        */
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
