using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 4;
    [SerializeField] private Vector2 camSensitivity = new(0.8f, 0.5f);
    [SerializeField] private float bobAmount = 0.2f;
    [SerializeField] private float bobSpeed = 4;
    [SerializeField] private new Transform camera;
    [SerializeField] private AudioSource footStepSource;
    
    private float _yRot = 0f;
    private Vector3 _defaultCameraPosition;

    private void Start()
    {
        _defaultCameraPosition = camera.localPosition;
    }

    private void Update()
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal") * (movementSpeed * 0.1f), 0, Input.GetAxis("Vertical")) * (movementSpeed * 0.01f), Space.Self);
        transform.Rotate(new Vector3(0, Input.mousePositionDelta.x * camSensitivity.x, 0), Space.Self);
        
        _yRot += -Input.mousePositionDelta.y * camSensitivity.y;
        _yRot = Mathf.Clamp(_yRot, -80f, 80f);
        camera.localEulerAngles = new Vector3(_yRot, camera.localEulerAngles.y, camera.localEulerAngles.z);
        camera.localPosition = _defaultCameraPosition + new Vector3(0, Mathf.Abs(Mathf.Sin(Time.time * bobSpeed)) * bobAmount * Input.GetAxis("Vertical"));
        footStepSource.volume = Mathf.Clamp01(Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal")));
    }

    public void TeleportTo(Transform target)
    {
        transform.position = target.position;
    }
}