using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Func_Swap : MonoBehaviour
{
    //첫 번쨰 닿은 콜라이더와 두 번째 닿은 콜라이더의 위치를 바꾼다. 
    //찍은 위치를 기준으로 X 모양에서 어느 범위로 이동하는지 체크하는 로직 작성 필요

    [SerializeField] private bool canTouch = true; //외부에서 탐색 알고리즘과 생성 및 블록파괴가 다끝난후 호출되어 트루가 된다.
    [SerializeField] private bool isDrag = false;
    [SerializeField] private bool moving = false;
    [SerializeField] private GameObject myBlock = null;
    [SerializeField] private GameObject swapblock = null;
    [SerializeField] private Func_Match match = null;
    [SerializeField] private Map_Information mapInfo = null;

    private Vector2 initialPosition;
    private BlockPos initPos;
    private Block[,] blocks;

    private void Awake()
    {
        mapInfo = FindObjectOfType<Map_Information>();
        match = FindObjectOfType<Func_Match>();
    }
    private void Start()
    {
        blocks = mapInfo.GetBlocksInfo();
    }
    void Update()
    {
        if (canTouch == false) return;
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 클릭 시
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider != null)
            {
                myBlock = hit.collider.gameObject;
                initialPosition = hit.collider.gameObject.transform.position;

                Block temp;
                myBlock.TryGetComponent<Block>(out temp); //현재 블럭 탐색
                initPos = temp.GetBlockPos(); //현재 블럭의 맵에서의 위치 
                isDrag = true;
            }
        }
        //드래그 중일 떄 
        if (isDrag == true)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float posX = mousePosition.x - initialPosition.x;
            float posY = mousePosition.y - initialPosition.y;
            float tan = posY / posX; //위치에 따른 tan값

            if (Vector3.Distance(initialPosition, mousePosition) > 0.3f)
            {
                canTouch = false;

                if (posY > 0 && (tan > 1 || tan < -1)) // 1사분면과 2사분면 사이 
                {
                    // y-1 위치 블럭과 현재 블럭 위치 교체
                    //맵정보 변경
                    //오브젝트 이동
                    //맵에 정보값을 알림 
                    SwapBlock(initPos, Direction.Up);
                }
                else if (posY < 0 && (tan > 1 || tan < -1)) //3사분면과 4사분면 사이
                {
                    // y+1 위치 블럭과 현재 블럭 위치 교체
                    SwapBlock(initPos, Direction.Down);
                }
                else if (posX < 0 && (-1 < tan && tan < 1)) //2사분면과 3사분면 사이
                {
                    // x+1 위치 블럭과 현재 블럭 위치 교체
                    SwapBlock(initPos, Direction.Left);
                }
                else if (posX > 0 && (-1 < tan && tan < 1)) //1사분면과 4사분면 사이
                {
                    // x-1 위치 블럭과 현재 블럭 위치 교체 
                    SwapBlock(initPos, Direction.Right);
                }
                //드래그 상태가 아님을 알림 => sweet load에서는 실질적인 드래그가 아니고 방향을 지정한 순간 드래그는 종료임
                isDrag = false;
            }
        }
    }
    public void SwapBlock(BlockPos pos, Direction dir)
    {
        Block temp;
        BlockPos newPos = pos;
        MatchList matchList = new MatchList();
        MatchList matchList2 = new MatchList();
        moving = true;
        switch (dir)
        {
            case Direction.Up:
                newPos.y = pos.y - 1;
                if (blocks[pos.y - 1, pos.x] != null) //맨위의 벽이 아니라면 맵정보 변경
                {
                    //match.FindAllMatchBlock(blocks, pos.x, pos.y); //BFS탐색시 사용, pos에 newPos사용 

                    //맵에서 오브젝트 이동
                    StartCoroutine(Co_SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y - 1, pos.x].gameObject));

                    //자신이 변경될 포지션값 넣어주기 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x, pos.y - 1);
                    blocks[pos.y - 1, pos.x].SetBlockPos(pos.x, pos.y);

                    //맵에서의 정보 교환
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y - 1, pos.x];
                    blocks[pos.y - 1, pos.x] = temp;

                    matchList = match.FindDirectMatchBlock(blocks, newPos.x, newPos.y);
                    matchList2 = match.FindDirectMatchBlock(blocks, pos.x, pos.y);
                    //옮겨서 터트릴게 없다면 다시 돌아옴
                    //가로 세로 같은 색의 블록이 3보다 작은 경우
                    if ((matchList.sameRawBlockPosition.Count < 3 && matchList.sameColumnBlockPosition.Count < 3) &&
                        (matchList2.sameRawBlockPosition.Count < 3 && matchList2.sameColumnBlockPosition.Count < 3))
                    {
                        //먼치킨 블럭을 만들기 위한 리스트가 같지 않을 경우
                        if ((matchList.tempColumnBlockPosition != matchList.tempRawBlockPosition) &&
                            (matchList2.tempColumnBlockPosition != matchList2.tempRawBlockPosition))
                        {
                            //블럭 제자리 복귀
                            StartCoroutine(Co_WaitReturnToOriginPos(newPos, pos));

                            //자신이 변경될 포지션값 넣어주기 
                            blocks[pos.y, pos.x].SetBlockPos(pos.x, pos.y - 1);
                            blocks[pos.y - 1, pos.x].SetBlockPos(pos.x, pos.y);

                            //맵에서의 정보 교환
                            temp = blocks[pos.y, pos.x];
                            blocks[pos.y, pos.x] = blocks[pos.y - 1, pos.x];
                            blocks[pos.y - 1, pos.x] = temp;
                        }
                    }
                }
                else
                    canTouch = true;
                break;
            case Direction.Down:
                newPos.y = pos.y + 1;
                if (blocks[pos.y + 1, pos.x] != null) //맨위의 벽이 아니라면 맵정보 변경
                {
                    //맵에서 오브젝트 이동
                    StartCoroutine(Co_SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y + 1, pos.x].gameObject));

                    //자신이 변경될 포지션값 넣어주기 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x, pos.y + 1);
                    blocks[pos.y + 1, pos.x].SetBlockPos(pos.x, pos.y);
                    //맵에서의 정보 교환
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y + 1, pos.x];
                    blocks[pos.y + 1, pos.x] = temp;

                    matchList = match.FindDirectMatchBlock(blocks, newPos.x, newPos.y);
                    matchList2 = match.FindDirectMatchBlock(blocks, pos.x, pos.y);
                    //옮겨서 터트릴게 없다면 다시 돌아옴
                    //가로 세로 같은 색의 블록이 3보다 작은 경우
                    if ((matchList.sameRawBlockPosition.Count < 3 && matchList.sameColumnBlockPosition.Count < 3) &&
                        (matchList2.sameRawBlockPosition.Count < 3 && matchList2.sameColumnBlockPosition.Count < 3))
                    {
                        //먼치킨 블럭을 만들기 위한 리스트가 같지 않을 경우
                        if ((matchList.tempColumnBlockPosition != matchList.tempRawBlockPosition) &&
                            (matchList2.tempColumnBlockPosition != matchList2.tempRawBlockPosition))
                        {
                            //맵에서 오브젝트 이동
                            StartCoroutine(Co_WaitReturnToOriginPos(newPos, pos));

                            //자신이 변경될 포지션값 넣어주기 
                            blocks[pos.y, pos.x].SetBlockPos(pos.x, pos.y + 1);
                            blocks[pos.y + 1, pos.x].SetBlockPos(pos.x, pos.y);
                            //맵에서의 정보 교환
                            temp = blocks[pos.y, pos.x];
                            blocks[pos.y, pos.x] = blocks[pos.y + 1, pos.x];
                            blocks[pos.y + 1, pos.x] = temp;
                        }
                    }
                }
                else
                    canTouch = true;
                break;
            case Direction.Left:
                newPos.x = pos.x - 1;
                if (blocks[pos.y, pos.x - 1] != null) //맨위의 벽이 아니라면 맵정보 변경
                {

                    //맵에서 오브젝트 이동
                    StartCoroutine(Co_SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y, pos.x - 1].gameObject));

                    //자신이 변경될 포지션값 넣어주기 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x - 1, pos.y);
                    blocks[pos.y, pos.x - 1].SetBlockPos(pos.x, pos.y);
                    //맵에서의 정보 교환
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y, pos.x - 1];
                    blocks[pos.y, pos.x - 1] = temp;

                    matchList = match.FindDirectMatchBlock(blocks, newPos.x, newPos.y);
                    matchList2 = match.FindDirectMatchBlock(blocks, pos.x, pos.y);
                    //옮겨서 터트릴게 없다면 다시 돌아옴
                    //가로 세로 같은 색의 블록이 3보다 작은 경우
                    if ((matchList.sameRawBlockPosition.Count < 3 && matchList.sameColumnBlockPosition.Count < 3) &&
                        (matchList2.sameRawBlockPosition.Count < 3 && matchList2.sameColumnBlockPosition.Count < 3))
                    {
                        //먼치킨 블럭을 만들기 위한 리스트가 같지 않을 경우
                        if ((matchList.tempColumnBlockPosition != matchList.tempRawBlockPosition) &&
                            (matchList2.tempColumnBlockPosition != matchList2.tempRawBlockPosition))
                        {
                            StartCoroutine(Co_WaitReturnToOriginPos(newPos, pos));

                            //자신이 변경될 포지션값 넣어주기 
                            blocks[pos.y, pos.x].SetBlockPos(pos.x - 1, pos.y);
                            blocks[pos.y, pos.x - 1].SetBlockPos(pos.x, pos.y);
                            //맵에서의 정보 교환
                            temp = blocks[pos.y, pos.x];
                            blocks[pos.y, pos.x] = blocks[pos.y, pos.x - 1];
                            blocks[pos.y, pos.x - 1] = temp;
                        }
                    }
                }
                else
                    canTouch = true;
                break;
            case Direction.Right:
                newPos.x = pos.x + 1;
                if (blocks[pos.y, pos.x + 1] != null) //맨위의 벽이 아니라면 맵정보 변경
                {
                    //맵에서 오브젝트 이동
                    StartCoroutine(Co_SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y, pos.x + 1].gameObject));
                    //자신이 변경될 포지션값 넣어주기 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x + 1, pos.y);
                    blocks[pos.y, pos.x + 1].SetBlockPos(pos.x, pos.y);
                    //맵에서의 정보 교환
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y, pos.x + 1];
                    blocks[pos.y, pos.x + 1] = temp;

                    matchList = match.FindDirectMatchBlock(blocks, newPos.x, newPos.y);
                    matchList2 = match.FindDirectMatchBlock(blocks, pos.x, pos.y);
                    //옮겨서 터트릴게 없다면 다시 돌아옴
                    //가로 세로 같은 색의 블록이 3보다 작은 경우
                    if ((matchList.sameRawBlockPosition.Count < 3 && matchList.sameColumnBlockPosition.Count < 3) &&
                        (matchList2.sameRawBlockPosition.Count < 3 && matchList2.sameColumnBlockPosition.Count < 3))
                    {
                        //먼치킨 블럭을 만들기 위한 리스트가 같지 않을 경우
                        if ((matchList.tempColumnBlockPosition != matchList.tempRawBlockPosition) &&
                            (matchList2.tempColumnBlockPosition != matchList2.tempRawBlockPosition))
                        {
                            //맵에서 오브젝트 이동
                            StartCoroutine(Co_WaitReturnToOriginPos(newPos, pos));

                            //자신이 변경될 포지션값 넣어주기 
                            blocks[pos.y, pos.x].SetBlockPos(pos.x + 1, pos.y);
                            blocks[pos.y, pos.x + 1].SetBlockPos(pos.x, pos.y);
                            //맵에서의 정보 교환
                            temp = blocks[pos.y, pos.x];
                            blocks[pos.y, pos.x] = blocks[pos.y, pos.x + 1];
                            blocks[pos.y, pos.x + 1] = temp;
                        }
                    }
                }
                else
                    canTouch = true;
                break;
            default:
                break;
        }
        //매치드 리스트 하나로 합치기
        //삭제하는 로직
        //StartCoroutine(Co_WaitGetScore(matchList, matchList2));
        //스왑후 정보 게임 매니저에 넘기기
        List<int[]> newList = SumMatchedList(matchList.matchedBlockPostion, matchList2.matchedBlockPostion);
        StartCoroutine(Co_SendToGameManage(newList));
    }
    private List<int[]> SumMatchedList(List<int[]> list1, List<int[]> list2)
    {
        List<int[]> sumList = new List<int[]>();
        foreach (int[] pos in list1)
        {
            sumList.Add(pos);
        }
        foreach (int[] pos in list2)
        {
            sumList.Add(pos);
        }

        return sumList;
    }

    public void AutoSwapBlock(Block[,] blocks ,int x, int y )
    {
        MatchList matchList = match.FindDirectMatchBlock(blocks,x, y);
        List<int[]> newList = matchList.matchedBlockPostion;
        StartCoroutine(Co_SendToGameManage(newList));
    }




    IEnumerator Co_SwapBlockPos(GameObject block1, GameObject block2)
    {
        //시작위치와 도착위치 지정
        Vector3 startPos1 = block1.transform.position, startPos2 = block2.transform.position;
        Vector3 endPos1 = startPos2, endPos2 = startPos1;
        //지정 시간안에 도달하게 하기위한 변수 선언
        float lerpTime = 0.5f;
        float curTime = 0;

        //lerp로 시작점과 끝점 움직인 선형보간
        while (lerpTime >= curTime)
        {
            curTime += Time.deltaTime;
            block1.transform.position = Vector3.Lerp(startPos1, endPos1, curTime / lerpTime);
            block2.transform.position = Vector3.Lerp(startPos2, endPos2, curTime / lerpTime);
            yield return null;
        }
        block1.transform.position = endPos1;
        block2.transform.position = endPos2;
        moving = false;
        canTouch = true;
    }
    IEnumerator Co_WaitReturnToOriginPos(BlockPos starPos, BlockPos endPos)
    {
        yield return new WaitUntil(() => moving == false);
        //터트릴게 없을 때 복귀하는 로직
        moving = true;
        StartCoroutine(Co_SwapBlockPos(blocks[starPos.y, starPos.x].gameObject, blocks[endPos.y, endPos.x].gameObject));
    }
    IEnumerator Co_SendToGameManage(List<int[]> newList)
    {
        yield return new WaitUntil(() => moving == false);
        GameManager.Instance.SendSwapInfo(blocks, newList);
    }
}