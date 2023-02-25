using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockPos
{
    public int x;
    public int y;   
}

public class Block : MonoBehaviour
{

    [SerializeField] private BlockColor blockColor; //블럭 색깔 지정
    [SerializeField] private BlockMode blockMode; //일반인지 먼치킨인지 구분
    [SerializeField] private SpriteRenderer mySpriteRenderer = null;
    private BlockPos blockPos = new BlockPos(); //블럭의 위치 값
    private void Awake()
    {
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    public void SetBlockColor(BlockColor blockColor)
    {
        switch (blockColor)
        {
            case BlockColor.Red:
                mySpriteRenderer.color = Color.red;
                blockColor = BlockColor.Red;
                break;
            case BlockColor.Green:
                mySpriteRenderer.color = Color.green;
                blockColor = BlockColor.Green;
                break;
            case BlockColor.Purple:
                mySpriteRenderer.color = Color.magenta;
                blockColor = BlockColor.Purple;
                break;
            case BlockColor.Yellow:
                mySpriteRenderer.color = Color.yellow;
                blockColor = BlockColor.Yellow;
                break;
        }
    }
    public void SetBlockMode(BlockMode blockmode)
    {
        this.blockMode = blockmode;
    }
    public void SetBlockPos(int x, int y)
    {
        blockPos.x = x;
        blockPos.y = y;
    }
    public BlockPos GetBlockPos()
    {
        return blockPos;
    }
}
