using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map_Information : MonoBehaviour
{
    [Header("===intput Mapsize (size X size) ===")]
    [SerializeField] private Tile_Information[] tile = null;
    [SerializeField] private Tile_Information[,] tileState = null;
    public void Awake()
    {
        //맵의 타일 정보 채우기
        tileState = new Tile_Information[9, 9];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                tileState[i, j] = tile[i * 9 + j];
                if (tileState[i, j].tileState == TileState.Closed) tileState[i, j].gameObject.SetActive(false);
            }
        }
    }

    public void Start()
    {
        Debug.Log(tileState[0, 0].tileState.ToString());
    }

}
