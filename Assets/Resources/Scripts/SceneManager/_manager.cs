using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class _manager : MonoBehaviour {

    public float _timer = 30.0f;
    public float _maxTime = 30.0f;
    public int _cratesRemaining;
    public Text _timerTxt;
    public Text _cratesRemainingTxt;
    public bool _inMenu;
    public GameObject _winScreen;
    public GameObject _loseScreen;
    public Text _countdownTxt;
    public bool _gameOver;
    public float _countdown = 1.5f;
    public Text _timeTakenText;
    public bool _paused;
    public GameObject _pauseMenu;
    public Text _levelTxt;
    public Text _bestTxt;
    public float _best;
    public Sprite _mute;
    public Sprite _unmute;
    public Image _muteBtn;
    public Ghosts _ghosts;

    public AudioSource _music;

	// Use this for initialization
	void Start () {
        _countdown = 1.5f;
        Time.timeScale = 1.0f;
        _timer = _maxTime;
        _inMenu = true;
        _music = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        _music.volume = 0;
        _muteBtn = GameObject.Find("Mute").GetComponent<Image>();
        _muteBtn.sprite = (AudioListener.pause == true) ? _unmute : _mute;
        _bestTxt = GameObject.Find("BestTimeTxt").GetComponent<Text>();
        var buildIndex = (SceneManager.GetActiveScene().buildIndex - 2);
        var build = (_playerManager._times[buildIndex] == 0.0f) ? _maxTime.ToString("F2") : _playerManager._times[buildIndex].ToString("F2");
        if (_playerManager._times[buildIndex] == 0.0f) _playerManager._times[buildIndex] = _maxTime;
        _bestTxt.text = "Record: " + build + "s";
        _levelTxt = GameObject.Find("LevelTxt").GetComponent<Text>();
        _levelTxt.text = "Level " + (SceneManager.GetActiveScene().buildIndex - 2);
        _levelTxt.enabled = false;
        _pauseMenu = GameObject.Find("PauseMenu");
        _pauseMenu.SetActive(false);
        _winScreen = GameObject.Find("GameOver_win");
        _winScreen.SetActive(false);
        _loseScreen = GameObject.Find("GameOver_lose");
        _loseScreen.SetActive(false);
        _timeTakenText = GameObject.Find("TimeTakenTxt").GetComponent<Text>();
        _timeTakenText.enabled = false;
        _countdownTxt = GameObject.Find("CountdownTxt").GetComponent<Text>();
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
        if (Input.GetKeyDown(KeyCode.Escape) && !_gameOver) Pause(!_paused);
        if (_inMenu && !_gameOver && !_paused)
        {
            Countdown();
            return;
        }
        if (!_gameOver) UpdateTimer(); 
        if (_gameOver && _music.volume > 0.0f) _music.volume -= 0.0005f;
    }

    void UpdateTimer()
    {
        _timer -= Time.deltaTime;
        _timerTxt.text = _timer.ToString("F2");
        if (_timer <= 0.0f)
        {
            EndLevel(false);
        }
        else if (_timer <= 5.0f)
        {
            _timerTxt.color = Color.red;
        }
    }

    void Countdown()
    {
        _countdown -= Time.deltaTime;
        _music.volume += 0.0005f;
        if (_countdown <= 0.01f)
        {
            _ghosts.StartGhost();
            _countdownTxt.text = "GO!";
            _countdownTxt.fontSize = 150;
            _countdownTxt.color = Color.green;
            _bestTxt.enabled = false;
            _inMenu = false;
            StartCoroutine(CloseCountdown());
        }
        else 
        {
            _countdownTxt.text = "Ready?";
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
        _countdownTxt.enabled = false;
    }

    public void EndLevel(bool victory)
    {
        _levelTxt.enabled = true;
        _inMenu = true;
        _gameOver = true;
        if (victory)
        {
            var time = (_maxTime - _timer);
            if (time < _playerManager._times[SceneManager.GetActiveScene().buildIndex - 2])
            {
                _playerManager._times[SceneManager.GetActiveScene().buildIndex - 2] = time;
                _bestTxt.text = "New Record: " + time.ToString("F2") + "s";
                _bestTxt.color = Color.green;
                _playerManager.SaveTimes();
                _ghosts.SaveGhost(SceneManager.GetActiveScene().buildIndex - 2);
            }
            _bestTxt.enabled = true;
            _winScreen.SetActive(true);
            if (_playerManager._playerLevel < SceneManager.GetActiveScene().buildIndex - 1) _playerManager._playerLevel = (SceneManager.GetActiveScene().buildIndex - 1);
            if (_playerManager._playerLevel == 12) _playerManager._playerLevel = 11;
            PlayerPrefs.SetInt("PlayerLevel", _playerManager._playerLevel);
            var timeString = time.ToString("F2");
            _timeTakenText.text = "Completed in: " + timeString + "s";
            _timeTakenText.enabled = true;
        }
        else
        {
            _loseScreen.SetActive(true);
        }
    }

    public void ChangeScene(int i)
    {
        SceneManager.LoadScene(i + 1);
        Time.timeScale = 1.0f;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
    }

    public void Pause(bool paused)
    {
        Time.timeScale = (!paused) ? 1.0f : 0.0f;
        _inMenu = paused;
        _pauseMenu.SetActive(paused);
        _paused = paused;
        _levelTxt.enabled = paused;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }
    public void Mute() {
        AudioListener.pause = !AudioListener.pause;
        _muteBtn.sprite = (AudioListener.pause == true) ? _unmute : _mute;
    }
}
