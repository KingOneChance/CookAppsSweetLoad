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
            if(_instance==null)
            {
                _instance = FindObjectOfType<GameManager>();
                if( _instance == null )
                    _instance = new GameManager();
            }
            return _instance;
        }
    }
    #endregion

    [field:SerializeField] public int score { get; set; } 
    [field:SerializeField] public Map_Information map_infomation { get; set; }

    [SerializeField] private List<int[]> matchedList = new List<int[]>();

    private void Awake()
    {
        map_infomation = FindObjectOfType<Map_Information>();
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
        
        for(int i= 0; i < matchedList.Count; i++)
        {
            Debug.Log(matchedList[i][0] + " " + matchedList[i][1]);
        }
    }
}
