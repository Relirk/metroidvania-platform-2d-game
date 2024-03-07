using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public int score;

    void Awake() {
        // instance = this;
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if( instance != this){
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {
        Time.timeScale = 1;

        if(PlayerPrefs.GetInt("score") > 0) {
            score = PlayerPrefs.GetInt("score");
            Player.instance.scoreText.text = "x " + score.ToString();
        }
    }

    public void GetCoin() {
        score++;
        Player.instance.scoreText.text = "x " + score.ToString();

        // tipo um local storage
        PlayerPrefs.SetInt("score", score);
    }
    
    public void ShowGameOver() {
        Player.instance.gameOver.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
