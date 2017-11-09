using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour {

    public float scrollSpeed = 0.5F;
    public Renderer rend;
    public int _materialIndex;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void Update()
    {
        float offset = Time.time * scrollSpeed;
        rend.materials[_materialIndex].SetTextureOffset("_MainTex", new Vector2(-offset, 0));
    }
}
