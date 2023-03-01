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
    [SerializeField] private int nowX;
    [SerializeField] private int nowY;
    private BlockPos blockPos = new BlockPos(); //���� ��ġ ��
    [field: SerializeField] public bool ismoving { get; set; } //�ڷ�ƾ �ߺ� ����
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
            case BlockColor.MunChKin:
                mySpriteRenderer.color =Color.white;
                this.blockColor = BlockColor.MunChKin;
                break;
            case BlockColor.Clear:
                mySpriteRenderer.color = Color.clear;
                this.blockColor = BlockColor.Clear;
                break;

        }
    }
    public void SetBlockRandomColor()
    {
        int ran = Random.Range(0, 4);
        switch (ran)
        {
            case 0:
                mySpriteRenderer.color = Color.red;
                this.blockColor = BlockColor.Red;
                break;
            case 1:
                mySpriteRenderer.color = Color.green;
                this.blockColor = BlockColor.Green;
                break;
            case 2:
                mySpriteRenderer.color = Color.magenta;
                this.blockColor = BlockColor.Purple;
                break;
            case 3:
                mySpriteRenderer.color = Color.yellow;
                this.blockColor = BlockColor.Yellow;
                break;
       
        }
    }
    public void SetBlockMode(BlockMode blockmode)
    {
        this.blockMode = blockmode;
        if (blockmode == BlockMode.Empty)
        {
            mySpriteRenderer.color = Color.clear;
            blockColor = BlockColor.Clear;
        }
    }
    public void SetBlockPos(int x, int y)
    {
        blockPos.x = x;
        blockPos.y = y;

        //gameObject.transform.position = new Vector2(x-4, 4-y);
        //���� ���� ��ġ��ǥ���ϱ� ���� 
        nowX = blockPos.x;
        nowY = blockPos.y;
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
    public int GetBlockPosX()
    {
        return blockPos.x;
    }
    public int GetBlockPosY()
    {
        return blockPos.y;
    }
    public void MoveBlock()
    {
        StartCoroutine(Co_MoveBlock(gameObject, nowX, nowY));
    }
    IEnumerator Co_MoveBlock(GameObject blockObject, int x, int y)
    {
        ismoving = true;
        //������ġ�� ������ġ ����
        Vector3 startPos = blockObject.transform.position;
        Vector3 endPos = new Vector3(x-4,4-y,0);
        //���� �ð��ȿ� �����ϰ� �ϱ����� ���� ����
        float lerpTime = 0.5f;
        float curTime = 0;

        //lerp�� �������� ���� ������ ��������
        while (lerpTime >= curTime)
        {
            curTime += Time.deltaTime;
            blockObject.transform.position = Vector3.Lerp(startPos, endPos, curTime / lerpTime);
            yield return null;
        }

        blockObject.transform.position = endPos;
        yield return null;
        ismoving = false;
    }

    public void MoveMuchkin(int x, int y)
    {
        StartCoroutine(Co_MoveMunchkin(gameObject, x, y));
    }
    IEnumerator Co_MoveMunchkin(GameObject blockObject, int x, int y)
    {
        //������ġ�� ������ġ ����
        Vector3 startPos = blockObject.transform.position;
        Vector3 endPos = new Vector3(x , y, 0);
        //���� �ð��ȿ� �����ϰ� �ϱ����� ���� ����
        float lerpTime = 0.5f;
        float curTime = 0;

        //lerp�� �������� ���� ������ ��������
        while (lerpTime >= curTime)
        {
            curTime += Time.deltaTime;
            blockObject.transform.position = Vector3.Lerp(startPos, endPos, curTime / lerpTime);
            yield return null;
        }
        //������Ʈ Ǯ�� �ֱ� ���� �� ��� ��ȯ
        SetBlockMode(BlockMode.Normal);
        //���� ���� ������ ���� ��ġŲ ���ھ �÷��ֱ�
        GameManager.Instance.SetMunchkinNum();
        gameObject.transform.localScale = Vector3.one;
        blockObject.transform.position = endPos;
        yield return null;
        ismoving = false;
        
    }
}
