using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private bool _isGameOver;

    private void Update()
    {
        //if the r key was pressed
        //restart the scene
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(1); //Game Scene
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape Key was pressed");
            Application.Quit();
        }
        
        //if escape key is pressed
        //quit application
    }
    public void GameOver()
    {
        _isGameOver = true;
    }


}
