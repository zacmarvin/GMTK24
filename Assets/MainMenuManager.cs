using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        CursorLockMode cursorMode = CursorLockMode.None;
        Cursor.lockState = cursorMode;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Foodtruck");
    }
}
