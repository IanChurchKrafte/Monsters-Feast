using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string LevelName;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            LoadLevel();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public void LoadLevel()
    {
        Cow.cowInjured = false;
        SceneManager.LoadScene(LevelName);
    }
}
