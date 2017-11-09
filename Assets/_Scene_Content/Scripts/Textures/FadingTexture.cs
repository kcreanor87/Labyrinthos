using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingTexture : MonoBehaviour {

    public float _changeSpeed = 0.25f;
    public float _maxAlpha = 150;
    public float _minAlpha = 60;
    private float value;
    private bool increase;
    public Renderer rend;
    public Color _colour;
    public int _materialIndex = 2;

    void Start()
    {
        _minAlpha = _minAlpha / 255;
        _maxAlpha = _maxAlpha/ 255;
        _changeSpeed = _changeSpeed / 255;
        value = _minAlpha;
        rend = GetComponent<Renderer>();
        _colour = rend.materials[_materialIndex].color;
    }
    void Update()
    {
        if (value >= _maxAlpha) increase = false;
        else if (value <= _minAlpha) increase = true;
        if (increase) value += _changeSpeed;
        else value -= _changeSpeed;
        Color color = new Color(_colour.r, _colour.g, _colour.b, value);
        rend.materials[_materialIndex].color = color;
        rend.materials[_materialIndex].SetFloat("_MKGlowTexStrength", value * 6.0f);
    }
}