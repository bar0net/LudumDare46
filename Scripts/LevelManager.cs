using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public bool main_menu_available = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }
}
