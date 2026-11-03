using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0) return;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnterGame()
    {
        SceneManager.LoadScene("Game");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
