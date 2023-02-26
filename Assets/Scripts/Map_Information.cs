using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map_Information : MonoBehaviour
{
    [Header("=== intput all tiles in map ===")]
    [SerializeField] private Tile_Information[] tile = null;
    [Header("=== input your block's prefab ===")]
    [SerializeField] private GameObject blockPrefab = null;

    private Tile_Information[,] tileInfo = null;
    private Block[,] blocks = null;
    private void Awake()
    {
        //�� Ÿ�Ͽ� SweetLoad Lv6 Ÿ�� ���� ä���
        tileInfo = new Tile_Information[9, 9];
        blocks = new Block[9, 9];
        GameObject blockFolder = new GameObject("BlockFolder");
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                tileInfo[i, j] = tile[i * 9 + j];
                if (tileInfo[i, j].tileState == TileState.Closed)
                    tileInfo[i, j].gameObject.SetActive(false);

                //�� Ÿ���� Normal���¸� �ʱ� ���� ����
                if (tileInfo[i, j].tileState == TileState.Normal)
                {
                    GameObject newBlock = Instantiate(blockPrefab, tileInfo[i, j].transform.position, Quaternion.identity);
                    newBlock.TryGetComponent<Block>(out blocks[i, j]);

                    //���̾��Ű �� �����뵵
                    newBlock.gameObject.transform.SetParent(blockFolder.transform);

                    //���� ���� ��ġ�� �־��ֱ� 
                    blocks[i, j].SetBlockMode(BlockMode.Normal);
                    blocks[i, j].SetBlockPos(j, i);
                }
            }
        }
        #region �� ��� �ʱ� �� ����
        blocks[1, 1].SetBlockColor(BlockColor.Purple);
        blocks[1, 2].SetBlockColor(BlockColor.Red);
        blocks[1, 3].SetBlockColor(BlockColor.Purple);
        blocks[1, 4].SetBlockColor(BlockColor.Red);
        blocks[1, 5].SetBlockColor(BlockColor.Red);
        blocks[1, 6].SetBlockColor(BlockColor.Yellow);
        blocks[1, 7].SetBlockColor(BlockColor.Yellow);

        blocks[2, 1].SetBlockColor(BlockColor.Yellow);
        blocks[2, 2].SetBlockColor(BlockColor.Purple);
        blocks[2, 3].SetBlockColor(BlockColor.Green);
        blocks[2, 4].SetBlockColor(BlockColor.Yellow);
        blocks[2, 5].SetBlockColor(BlockColor.Green);
        blocks[2, 6].SetBlockColor(BlockColor.Red);
        blocks[2, 7].SetBlockColor(BlockColor.Purple);

        blocks[3, 1].SetBlockColor(BlockColor.Purple);
        blocks[3, 2].SetBlockColor(BlockColor.Green);
        blocks[3, 3].SetBlockColor(BlockColor.Red);
        blocks[3, 4].SetBlockColor(BlockColor.Red);
        blocks[3, 5].SetBlockColor(BlockColor.Yellow);
        blocks[3, 6].SetBlockColor(BlockColor.Purple);
        blocks[3, 7].SetBlockColor(BlockColor.Yellow);

        blocks[4, 1].SetBlockColor(BlockColor.Yellow);
        blocks[4, 2].SetBlockColor(BlockColor.Yellow);
        blocks[4, 3].SetBlockColor(BlockColor.Red);
        blocks[4, 4].SetBlockColor(BlockColor.Green);
        blocks[4, 5].SetBlockColor(BlockColor.Red);
        blocks[4, 6].SetBlockColor(BlockColor.Yellow);
        blocks[4, 7].SetBlockColor(BlockColor.Green);

        blocks[5, 1].SetBlockColor(BlockColor.Purple);
        blocks[5, 2].SetBlockColor(BlockColor.Green);
        blocks[5, 3].SetBlockColor(BlockColor.Green);
        blocks[5, 4].SetBlockColor(BlockColor.Yellow);
        blocks[5, 5].SetBlockColor(BlockColor.Green);
        blocks[5, 6].SetBlockColor(BlockColor.Green);
        blocks[5, 7].SetBlockColor(BlockColor.Purple);

        blocks[6, 1].SetBlockColor(BlockColor.Green);
        blocks[6, 2].SetBlockColor(BlockColor.Yellow);
        blocks[6, 3].SetBlockColor(BlockColor.Purple);
        blocks[6, 4].SetBlockColor(BlockColor.Green);
        blocks[6, 5].SetBlockColor(BlockColor.Yellow);
        blocks[6, 6].SetBlockColor(BlockColor.Green);
        blocks[6, 7].SetBlockColor(BlockColor.Yellow);

        blocks[7, 1].SetBlockColor(BlockColor.Green);
        blocks[7, 2].SetBlockColor(BlockColor.Green);
        blocks[7, 3].SetBlockColor(BlockColor.Purple);
        blocks[7, 4].SetBlockColor(BlockColor.Red);
        blocks[7, 5].SetBlockColor(BlockColor.Purple);
        blocks[7, 6].SetBlockColor(BlockColor.Yellow);
        blocks[7, 7].SetBlockColor(BlockColor.Yellow);

        #endregion
    }

    private void Start()
    {
        GameManager.Instance.func_Spawn.SetInitBlockPool(blocks);
    }
    public Block[,] GetBlocksInfo()
    {
        return blocks;
    }
    public void SetBlccksInfo(Block[,] blocks)
    {
        this.blocks = blocks;
    }
    public void PrintBlocks()
    {
       
    }
}
