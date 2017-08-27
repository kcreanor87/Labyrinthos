﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class _playerManager : MonoBehaviour {

    public static int _playerLevel;
    public static float _version = 0.1f;

    public bool _newGame;
    public bool _unlockAll;
    public static bool _skipscreen;
    public static int _totalLevels = 19;

    public static List<float> _times = new List<float>();

    public static bool _tooltips;
    public static int _levelIndex;

    public static int _P1Score;
    public static int _P2Score;

    void Awake()
    {        
        if (_newGame) PlayerPrefs.DeleteAll();
        DontDestroyOnLoad(gameObject);
        if (!PlayerPrefs.HasKey("PlayerLevel")){
            PlayerPrefs.SetInt("PlayerLevel", 0);
            for (int i = 0; i < _totalLevels; i++)
            {
                PlayerPrefs.SetFloat("Time" + i, 0.0f);
                _times.Add(0.0f);
            }
        }
        else {
            _playerLevel = (_unlockAll) ? _totalLevels - 1 : PlayerPrefs.GetInt("PlayerLevel");
            for (int i = 0; i < (_playerLevel + 1); i++)
            {
                _times.Add(0.0f);
            }
            LoadTimes();
        }
        _skipscreen = false;
        _tooltips = (PlayerPrefs.GetInt("Tooltip") > 0);
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            print(Input.GetJoystickNames()[i]);
        }
    }

    public static void SaveTimes()
    {
        for (int i = 0; i < _times.Count; i++)
        {
            PlayerPrefs.SetFloat("Time" + i, _times[i]);
        }
    }

    void LoadTimes()
    {
        for (int i = 0; i < _times.Count; i++)
        {
            _times[i] = PlayerPrefs.GetFloat("Time" + i);
        }
        int diff = _totalLevels - _times.Count;
        for (int i = 0; i < diff; i++)
        {
            _times.Add(0.0f);
        }
    }    
}