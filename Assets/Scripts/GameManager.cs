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
    [field: SerializeField] public Func_Swap func_swap { get; set; }

    [SerializeField] private List<int[]> matchedList = new List<int[]>();
    [SerializeField] private int score = 0;
    [SerializeField] private int munchkinNum = 3;

    private void Awake()
    {
        func_swap = FindObjectOfType<Func_Swap>();
        map_infomation = FindObjectOfType<Map_Information>();
        func_Spawn = FindObjectOfType<Func_Spawn>();
        uiManager = FindObjectOfType<UiManager>();
    }

    private void Start()
    {
        Debug.Log("hello world");
    }
    //������ �������� ����
    public void SendSwapInfo(Block[,] blocks, List<int[]> matchedList, List<int[]> munPos)
    {
        map_infomation.SetBlccksInfo(blocks);
        this.matchedList = matchedList;

        //�����ʿ��� ��� ���� �� ����, �������� ó�� ��Ŵ
        func_Spawn.SetMapBlocksInfo(blocks);
        func_Spawn.SetMatchedList(this.matchedList, munPos);

    }
    public void SetScore(int point)
    {
        score += point;
        uiManager.SetScoreOnUI(score);
    }
    public void SetMunchkinNum()
    {
        munchkinNum--;
        uiManager.SetMunchkinOnUI(munchkinNum);
        if (munchkinNum == 0)
        {
            uiManager.GameOverWin();
            func_swap.gameOver = true;
        }
    }
}
