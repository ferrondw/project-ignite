using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MovementSpeed = 4;
    public Vector2 CamSensitivity = new Vector2(0.8f, 0.5f);
    public Transform Camera;
    
    private float _yRot = 0f;
    
    private void Update()
    {
        transform.Translate(new Vector3(0, 0, Input.GetAxis("Vertical")) * (MovementSpeed * 0.01f), Space.Self);
        transform.Rotate(new Vector3(0, Input.mousePositionDelta.x * CamSensitivity.x, 0), Space.Self);
        
        _yRot += -Input.mousePositionDelta.y * CamSensitivity.y;
        _yRot = Mathf.Clamp(_yRot, -80f, 80f);
        Camera.localEulerAngles = new Vector3(_yRot, Camera.localEulerAngles.y, Camera.localEulerAngles.z);

    }
}