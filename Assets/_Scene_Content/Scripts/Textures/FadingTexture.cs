using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingTexture : MonoBehaviour {

    public float _changeSpeed = 0.12f;
    public float scrollSpeed = 0.1f;
    public float _maxAlpha = 100;
    public float _minAlpha = 30;
    private float value;
    private bool increase;
    public Renderer rend;
    public Color _colour;
    public int _materialIndex = 1;

    void Start()
    {

        _minAlpha = _minAlpha / 255;
        _maxAlpha = _maxAlpha/ 255;
        _changeSpeed = _changeSpeed / 255;
        value = _minAlpha;
        rend = GetComponent<Renderer>();
        for (int i = 0; i < rend.materials.Length; i++)
        {
            if (rend.materials[i].name.Contains("Glow"))
            {
                _materialIndex = i;
            }
        }
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
        rend.materials[_materialIndex].SetFloat("_MKGlowTexStrength", value * 3.0f);
        float offset = Time.time * scrollSpeed;
        rend.materials[_materialIndex].SetTextureOffset("_MKGlowTex", new Vector2(0, offset));
    }
}