using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region GameManager Singleton
    private GameManager() { }
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                    _instance = new GameManager();
            }
            return _instance;
        }
    }
    #endregion

    [field: SerializeField] public Map_Information map_infomation { get; set; }
    [field: SerializeField] public Func_Spawn func_Spawn { get; set; }
    [field: SerializeField] public UiManager uiManager { get; set; }

    [SerializeField] private List<int[]> matchedList = new List<int[]>();
    [SerializeField] private int score;

    private void Awake()
    {
        map_infomation = FindObjectOfType<Map_Information>();
        func_Spawn = FindObjectOfType<Func_Spawn>();
        uiManager = FindObjectOfType<UiManager>();
    }

    private void Start()
    {
        Debug.Log("hello world");
    }
    //스왑후 정보들을 받음
    public void SendSwapInfo(Block[,] blocks, List<int[]> matchedList)
    {
        map_infomation.SetBlccksInfo(blocks);
        this.matchedList = matchedList;

        //스포너에게 블록 생성 및 삭제, 점수증가 처리 시킴
        func_Spawn.SetMapBlocksInfo(blocks);
        func_Spawn.SetMatchedList(this.matchedList);

        for (int i = 0; i < matchedList.Count; i++)
        {
            //가로 세로 모두 있을을 경우 2번 찍히는 좌표가 있음
            Debug.Log(matchedList[i][0] + " " + matchedList[i][1]);
        }
    }
    public void SetScore(int point)
    {
        score += point;
        uiManager.SetScoreOnUI(score);
    }
}
