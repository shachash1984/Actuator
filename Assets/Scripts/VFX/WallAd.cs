using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAd : MonoBehaviour
{
    Material mat;
    Vector2 offset;
    [Range(0, 1)]
    public float smoothing = 0.5f;

    private void Start()
    {
        mat = GetComponent<Renderer>().material;
        offset = Vector2.zero;
    }

    private void Update()
    {
        offset.x -= Time.deltaTime * smoothing;
        mat.SetTextureOffset("_MainTex", offset);
    }
}
