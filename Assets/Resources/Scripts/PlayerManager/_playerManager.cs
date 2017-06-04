using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class _playerManager : MonoBehaviour {

    public static int _playerLevel;

    public bool _newGame;
    public int _totalLevels;

    public static List<float> _times = new List<float>();

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
            _playerLevel = PlayerPrefs.GetInt("PlayerLevel");
            for (int i = 0; i < (_playerLevel + 1); i++)
            {
                _times.Add(0.0f);
            }
            LoadTimes();
        }
        for (int i = 0; i < (_times.Count); i++)
        {
            print("Level " + i + ": " + _times[i]);
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
