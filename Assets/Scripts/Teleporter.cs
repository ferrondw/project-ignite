using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yakanashe.Yautl;

public class Teleporter : MonoBehaviour
{
    public Datamosh mosh;
    public Transform player;
    public List<Transform> teleportPoints;

    public float teleportInterval = 8f;
    public Slider teleportIntervalSlider;
    
    private int _currentTeleportPoint = -1;
    private float _baseEntropy;
    private float _lastTeleportTime;

    private void Start()
    {
        _baseEntropy = mosh.Entropy;
    }

    private void Update()
    {

        teleportIntervalSlider.value = (teleportInterval - (Time.time - _lastTeleportTime)) / teleportInterval;
        if (!(Time.time - _lastTeleportTime > teleportInterval)) return;
        
        _lastTeleportTime = Time.time;
        mosh.Sequence = 1;
        Teleport();
        EntropyTo(mosh, -0.5f, 1.5f).OnComplete(() =>
        {
            mosh.Sequence = 0;
            mosh.Entropy = _baseEntropy;
        });
    }

    private void Teleport()
    {
        _currentTeleportPoint = (_currentTeleportPoint + 1) % (teleportPoints.Count);
        player.transform.position = teleportPoints[_currentTeleportPoint].position;
        Debug.Log($"Teleported to {teleportPoints[_currentTeleportPoint].position}");
    }
    
    public static ITween EntropyTo(Datamosh mosh, float to, float duration, EaseType ease = EaseType.InOutSine)
    {
        var tween = new Tween<float>(mosh.transform, () => mosh.Entropy, v => mosh.Entropy = v, to, duration, ease, Mathf.Lerp);
        TweenRunner.Instance.Run(tween);
        return tween;
    }
}