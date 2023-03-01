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
    [SerializeField] private int nowX;
    [SerializeField] private int nowY;
    private BlockPos blockPos = new BlockPos(); //블럭의 위치 값
    [field: SerializeField] public bool ismoving { get; set; } //코루틴 중복 막기
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
        //현재 블럭의 위치값표현하기 위함 
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
        //시작위치와 도착위치 지정
        Vector3 startPos = blockObject.transform.position;
        Vector3 endPos = new Vector3(x-4,4-y,0);
        //지정 시간안에 도달하게 하기위한 변수 선언
        float lerpTime = 0.5f;
        float curTime = 0;

        //lerp로 시작점과 끝점 움직인 선형보간
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
        //시작위치와 도착위치 지정
        Vector3 startPos = blockObject.transform.position;
        Vector3 endPos = new Vector3(x , y, 0);
        //지정 시간안에 도달하게 하기위한 변수 선언
        float lerpTime = 0.5f;
        float curTime = 0;

        //lerp로 시작점과 끝점 움직인 선형보간
        while (lerpTime >= curTime)
        {
            curTime += Time.deltaTime;
            blockObject.transform.position = Vector3.Lerp(startPos, endPos, curTime / lerpTime);
            yield return null;
        }
        //오브젝트 풀에 넣기 위해 블럭 모드 전환
        SetBlockMode(BlockMode.Normal);
        //게임 종료 로직을 위한 먼치킨 스코어값 올려주기
        GameManager.Instance.SetMunchkinNum();
        gameObject.transform.localScale = Vector3.one;
        blockObject.transform.position = endPos;
        yield return null;
        ismoving = false;
        
    }
}
