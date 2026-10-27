using TMPro;
using UnityEngine;

public class InteractionChecker : MonoBehaviour
{
    [SerializeField] private float checkDistance = 5;
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private LayerMask interactionLayer;

    private void LateUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hitInfo2, checkDistance))
        {
            Debug.Log(hitInfo2.transform.name);
        }

        if (!Physics.Raycast(transform.position, transform.forward, out var hitInfo, checkDistance, interactionLayer))
        {
            interactText.gameObject.SetActive(false);
            return;
        }

        interactText.gameObject.SetActive(true);

        if (!Input.GetKeyDown(KeyCode.E)) return;
        if (!hitInfo.collider.TryGetComponent(out Interactable interactable)) return;

        interactable.OnTriggered.Invoke();
    }
}