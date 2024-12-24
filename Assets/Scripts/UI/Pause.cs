using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;

    public Button exitButton;
    
    void Awake()
    {
        pauseMenu.SetActive(false);
        exitButton.onClick.AddListener(ExitGame);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ExitGame()
    {
        print("Execution finished");
        Application.Quit();
    }
}
