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

        blockColor = new BlockColor();
        blockMode = new BlockMode();
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    public void SetBlockColor(BlockColor blockColor)
    {
        switch (blockColor)
        {
            case BlockColor.Red:
                mySpriteRenderer.color = Color.red;
                this.blockColor = BlockColor.Red;
                break;
            case BlockColor.Green:
                mySpriteRenderer.color = Color.green;
                this.blockColor = BlockColor.Green;
                break;
            case BlockColor.Purple:
                mySpriteRenderer.color = Color.magenta;
                this.blockColor = BlockColor.Purple;
                break;
            case BlockColor.Yellow:
                mySpriteRenderer.color = Color.yellow;
                this.blockColor = BlockColor.Yellow;
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
    public BlockColor GetBlockColor()
    {
        return blockColor;
    }
    public BlockMode GetBlockMode()
    {
        return blockMode;
    }
    public BlockPos GetBlockPos()
    {
        return blockPos;
    }
}
