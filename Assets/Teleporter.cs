using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Teleporter : MonoBehaviour
{
    public Datamosh mosh;
    public CharacterController controller;
    public List<Transform> teleportPoints;

    private int _currentTeleportPoint = -1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            mosh.Sequence = 1;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            mosh.Sequence = 0;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            _currentTeleportPoint = (_currentTeleportPoint + 1) % (teleportPoints.Count);
            controller.transform.position = teleportPoints[_currentTeleportPoint].position;
            Debug.Log($"Teleported to {teleportPoints[_currentTeleportPoint].position}");
        }
    }
}