using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _musicManager : MonoBehaviour {

    public List<AudioSource> _music = new List<AudioSource>();
    public AudioSource _activeSong;

    public float _volumeHigh = 1.0f;
    public float _volumeLow = 0.0f;

    public float _timePlayed;

    public int _index;

    public bool _fadingIn;
    public bool _fadingOut;

	// Use this for initialization
	void Start () {
        Switch(0);
	}

    void Update()
    {
        if (_fadingOut)
        {
            FadeOut();
        }
        else if (_fadingIn)
        {
            FadeIn();
        }
        else
        {
            TimeChecker();
        }

        _timePlayed += Time.deltaTime;
    }

    void Switch(int index)
    {
        if (index >= _music.Count) _index = 1;
        for (int i = 0; i < _music.Count; i++)
        {
            _music[i].Stop();
        }
        _activeSong = _music[index];
        _activeSong.Play();
        _activeSong.volume = _volumeLow;
        _index = index;
        _fadingIn = true;
    }

    void FadeIn()
    {
        if (_activeSong.volume < _volumeHigh) _activeSong.volume += 0.3f * Time.deltaTime;
        else _fadingIn = false;
    }

    void FadeOut()
    {
        if (_activeSong.volume > _volumeLow) _activeSong.volume -= 0.3f * Time.deltaTime;
        else
        {
            _index++;
            Switch(_index);
            _fadingOut = false;
            _timePlayed = 0.0f;
        }
    }

    void TimeChecker()
    {
        if (_timePlayed >= _activeSong.clip.length - 1.0f) _fadingOut = true;
    }
}
