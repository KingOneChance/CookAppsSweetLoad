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
    //������ �������� ����
    public void SendSwapInfo(Block[,] blocks, List<int[]> matchedList)
    {
        map_infomation.SetBlccksInfo(blocks);
        this.matchedList = matchedList;

        //�����ʿ��� ��� ���� �� ����, �������� ó�� ��Ŵ
        func_Spawn.SetMapBlocksInfo(blocks);
        func_Spawn.SetMatchedList(this.matchedList);

        for (int i = 0; i < matchedList.Count; i++)
        {
            //���� ���� ��� ������ ��� 2�� ������ ��ǥ�� ����
            Debug.Log(matchedList[i][0] + " " + matchedList[i][1]);
        }
    }
    public void SetScore(int point)
    {
        score += point;
        uiManager.SetScoreOnUI(score);
    }
}
