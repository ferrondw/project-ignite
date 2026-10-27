using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yakanashe.Yautl;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float downwardsTeleport = -5f;

    private void Start()
    {
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
    }

    private void Teleport()
    {
        player.transform.position += new Vector3(0, downwardsTeleport, 0);
    }
}