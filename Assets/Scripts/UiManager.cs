using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scorePanel=null;

    private void Start()
    {
        scorePanel.text = "Score : 0";
    }
    public void SetScoreOnUI(int score)
    {
        scorePanel.text = "Scord : " + score;
    }
}
