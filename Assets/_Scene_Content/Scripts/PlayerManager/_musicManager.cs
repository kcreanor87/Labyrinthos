using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _musicManager : MonoBehaviour {

    public List<AudioSource> _music = new List<AudioSource>();

    public float _volumeHigh = 1.0f;
    public float _volumeLow = 0.0f;

    public bool _fadingIn;
    public bool _fadingOut;

    void Update()
    {
        if (_fadingIn) FadeIn();
        else if (_fadingOut) FadeOut();
    }

    void FadeIn()
    {
        if (_music[0].volume < _volumeHigh)
        {
            _music[0].volume += Time.deltaTime * 0.4f;
            _music[1].volume += Time.deltaTime * 0.4f;
        }
        else _fadingIn = false;
    }

    void FadeOut()
    {
        if (_music[0].volume > _volumeLow)
        {
            _music[0].volume -= Time.deltaTime * 1.0f;
            _music[1].volume -= Time.deltaTime * 1.0f;
        }
        else _fadingOut = false;
    }

    public void PlayAudio()
    {
        _music[0].Play();
        _music[1].Play();
    }
}
