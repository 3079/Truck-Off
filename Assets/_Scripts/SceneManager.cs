using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private string currentScene;

    public int finalScore { get; private set; }
    public int highScore { get; private set; }

    public static SceneManager instance;
    
    // private void OnEnable()
    // {
        // InputHandler.Instance.OnRestart += RestartScene;
    // }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        currentScene = "Start Screen";
    }

    public void LoadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game Screen");
        currentScene = "Game Screen";
    }

    public void Menu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start Screen");
        currentScene = "Start Screen";
    }
    
    public void EndScreen(int score)
    {
        finalScore = score;
        highScore = score > highScore ? score : highScore;
        UnityEngine.SceneManagement.SceneManager.LoadScene("End Screen");
        currentScene = "End Screen";
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // private void RestartScene()
    // {
    //     if (currentScene.Equals("Start Screen") || currentScene.Equals("End Screen")) return;
    //     
    //     UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene);
    // }
    //
    // private void OnDisable()
    // {
    //     InputHandler.Instance.OnRestart -= RestartScene;
    // }
}
