using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scorePanel=null;
    [SerializeField] private RawImage gameOver =null;
    [SerializeField] private Button restart =null;
    [SerializeField] private Button quit = null;

    private void Start()
    {
        scorePanel.text = "Score : 0";
        gameOver.gameObject.SetActive(false);
    }
    public void GameOverWin()
    {
        gameOver.gameObject.SetActive(true);
    }

    public void SetScoreOnUI(int score)
    {
        scorePanel.text = "Scord : " + score;
    }
    public void OnClick_Restart()
    {
        SceneManager.LoadScene("GameScene_Lv06");
    }
    public void OnClick_Quit()
    {
        Application.Quit();
    }
}
