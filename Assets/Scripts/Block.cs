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

    [SerializeField] private BlockColor blockColor; //�� ���� ����
    [SerializeField] private BlockMode blockMode; //�Ϲ����� ��ġŲ���� ����
    [SerializeField] private SpriteRenderer mySpriteRenderer = null;
    private BlockPos blockPos = new BlockPos(); //���� ��ġ ��
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
