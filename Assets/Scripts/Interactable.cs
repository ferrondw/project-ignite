using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
    public UnityEvent OnTriggered;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
