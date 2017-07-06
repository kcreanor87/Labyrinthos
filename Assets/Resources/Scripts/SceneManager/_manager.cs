﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class _manager : MonoBehaviour {

    public float _timer;
    public float _maxTime = 30.0f;
    public float _best;
    public float _countdown = 1.2f;

    public int _cratesRemaining;
    
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
    //public Text _countdownTxt;
    public Text _rankText;

    public Ghosts _ghosts;    
    public Image _rankImage;
    public Sprite _rankSsprite, _rankAsprite, _rankBsprite, _rankCsprite;
    public LevelTimeContainer _timeContainer;

    public Animator _UIanim;
    public Animator _joystickAnim;
    public Animator _playerAnim;
    

    private void Awake()
    {
        if (GameObject.Find("_playerManager") == null) SceneManager.LoadScene(0);
    }

    // Use this for initialization
    void Start () {
        SpawnCrates();
        _playerAnim = GameObject.Find("Player").GetComponent<Animator>();
        _UIanim = gameObject.GetComponent<Animator>();
        _joystickAnim = GameObject.Find("VirtualJoystick").GetComponent<Animator>();
        _joystick = _joystickAnim.transform.Find("MobileJoystick");
        _timeContainer = GameObject.Find("_playerManager").GetComponent<LevelTimeContainer>();
        _rankImage = GameObject.Find("Rank").GetComponent<Image>();
        _rankText = GameObject.Find("RankText").GetComponent<Text>();
        _countdown = 2.0f;
        Time.timeScale = 1.0f;
        _timer = 0.0f;
        _inMenu = true;
        _bestTxt = GameObject.Find("BestTimeTxt").GetComponent<Text>();
        _recordTxt = GameObject.Find("RecordTxt").GetComponent<Text>();
        var buildIndex = (SceneManager.GetActiveScene().buildIndex - 2);
        var build = (_playerManager._times[buildIndex] == 0.0f) ? "--:--" : _playerManager._times[buildIndex].ToString("F2");
        _bestTxt.text = "Record: " + build + " s";
        _recordTxt.text = build + " s";
        _levelTxt = GameObject.Find("LevelTxt").GetComponent<Text>();
        _levelTxt.text = "Level " + (SceneManager.GetActiveScene().buildIndex - 1);
        _levelTxt.enabled = false;
        _winScreen = GameObject.Find("GameOver_win");
        _winScreen.SetActive(false);
        _loseScreen = GameObject.Find("GameOver_lose");
        _loseScreen.SetActive(false);
        _timeTakenText = GameObject.Find("TimeTakenTxt").GetComponent<Text>();
        _timeTakenText.enabled = false;
        //_countdownTxt = GameObject.Find("CountdownTxt").GetComponent<Text>();
        _timerTxt = GameObject.Find("TimerTxt").GetComponent<Text>();
        _cratesRemainingTxt = GameObject.Find("CratesRemainingTxt").GetComponent<Text>();
        _cratesRemaining = GameObject.FindGameObjectsWithTag("Crate").Length;
        _cratesRemainingTxt.text = _cratesRemaining.ToString();
        _timerTxt.text = _timer.ToString("F2");
        _gameOver = false;
        _ghosts = gameObject.GetComponent<Ghosts>();  
}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (_inMenu && !_gameOver)
        {
            Countdown();
            return;
        }
        if (!_gameOver) UpdateTimer();
        if (Input.GetKeyDown(KeyCode.W)){ EndLevel(true);}
    }

    void UpdateTimer()
    {
        _timer += Time.deltaTime;
        _timerTxt.text = _timer.ToString("F2");
    }

    void Countdown()
    {
        _countdown -= Time.deltaTime;
        if (_countdown <= 0.01f)
        {
            _ghosts.StartGhost();
            //_countdownTxt.text = "GO!";
            //_countdownTxt.fontSize = 150;
            _bestTxt.enabled = false;
            _inMenu = false;
            //StartCoroutine(CloseCountdown());
        }
        else 
        {
            //_countdownTxt.text = "Ready?";
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
        int index = SceneManager.GetActiveScene().buildIndex - 2;
        if (victory)
        {
            RankSwitcher(index, _timer);
            if (_timer < _playerManager._times[index] || (_playerManager._times[SceneManager.GetActiveScene().buildIndex - 2] == 0.0f))
            {
                _playerManager._times[index] = _timer;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        if ((SceneManager.GetActiveScene().buildIndex - 2) < _playerManager._totalLevels)
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
}
