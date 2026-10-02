using System;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Moshing : MonoBehaviour
{
    public Texture2D tex;
    public RenderTexture renderTex;
    public RawImage rawImage;
    public Camera cam;
    public float maxDiff = 0.3f;
    public float lerpSpeed = 0.3f;
    public int chunkSize = 20;

    private void Start()
    {
        Application.targetFrameRate = 24;
        tex = ScreenCapture.CaptureScreenshotAsTexture();
        rawImage.texture = tex;
    }

    private void Update()
    {
        var texPixels = tex.GetPixels();
        var renderTexPixels = ToTexture2D(renderTex).GetPixels();
        var length = texPixels.Length;
        var modTime = Time.time % 1;

        for (var i = 0; i < length; i++)
        {
            var chunkId = i / Screen.width / chunkSize * Mathf.CeilToInt((float)Screen.width / chunkSize) + i % Screen.width / chunkSize;

            if (new Random(chunkId).Next(20) * 0.06f < modTime) continue;
            tex.SetPixel(i % Screen.width, i / Screen.width, renderTexPixels[i]);
        }

        tex.Apply();
    }

    // thanks stackoverflows
    private static Texture2D ToTexture2D(RenderTexture rTex)
    {
        var toTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        toTex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        toTex.Apply();
        return toTex;
    }
}