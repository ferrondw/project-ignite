using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yakanashe.Yautl;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Datamosh mosh;
    [SerializeField] private Transform player;
    [SerializeField] private float downwardsTeleport = -5f;
    
    private float _baseEntropy;

    private void Start()
    {
        _baseEntropy = mosh.Entropy;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Teleport();
        
        /*mosh.Sequence = 1;
        EntropyTo(mosh, -0.5f, 1.5f).OnComplete(() =>
        {
            mosh.Sequence = 0;
            mosh.Entropy = _baseEntropy;
        });*/
    }

    private void Teleport()
    {
        player.transform.position += new Vector3(0, downwardsTeleport, 0);
    }
    
    public static ITween EntropyTo(Datamosh mosh, float to, float duration, EaseType ease = EaseType.InOutSine)
    {
        var tween = new Tween<float>(mosh.transform, () => mosh.Entropy, v => mosh.Entropy = v, to, duration, ease, Mathf.Lerp);
        TweenRunner.Instance.Run(tween);
        return tween;
    }
}