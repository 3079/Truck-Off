using System;
using UnityEngine;
using TMPro;

public class EndScreenUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _highscoreText;
    private void Start()
    {
        _scoreText.text = SceneManager.instance.finalScore.ToString();
        _highscoreText.text = SceneManager.instance.highScore.ToString();
    }
}
