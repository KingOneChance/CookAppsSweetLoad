using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Func_Swap : MonoBehaviour
{
    //ù ���� ���� �ݶ��̴��� �� ��° ���� �ݶ��̴��� ��ġ�� �ٲ۴�. 
    //���� ��ġ�� �������� X ��翡�� ��� ������ �̵��ϴ��� üũ�ϴ� ���� �ۼ� �ʿ�

    [SerializeField] private bool canTouch = true; //�ܺο��� Ž�� �˰���� ���� �� ����ı��� �ٳ����� ȣ��Ǿ� Ʈ�簡 �ȴ�.
    [SerializeField] private bool isDrag =false;
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
            // ���콺 Ŭ�� ��
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider != null)
            {
                myBlock = hit.collider.gameObject;
                initialPosition = hit.collider.gameObject.transform.position;

                Block temp;
                myBlock.TryGetComponent<Block>(out temp); //���� �� Ž��
                initPos = temp.GetBlockPos(); //���� ���� �ʿ����� ��ġ 
                isDrag = true;
            }
        }
        //�巡�� ���� �� 
        if (isDrag == true)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float posX = mousePosition.x - initialPosition.x;
            float posY = mousePosition.y - initialPosition.y;
            float tan = posY / posX; //��ġ�� ���� tan��

            if (Vector3.Distance(initialPosition, mousePosition) > 0.3f)
            {
                canTouch = false;

                Debug.Log("if �ʱ���ġ : " + initialPosition + "\n������ġ : " + posX + ", " + posY);
                if (posY > 0 && (tan > 1 || tan < -1)) // 1��и�� 2��и� ���� 
                {
                    // y-1 ��ġ ���� ���� �� ��ġ ��ü
                    //������ ����
                    //������Ʈ �̵�
                    //�ʿ� �������� �˸� 
                    SwapBlock(initPos, Direction.Up);
                    Debug.Log("up");

                }
                else if (posY < 0 && (tan > 1 || tan < -1)) //3��и�� 4��и� ����
                {
                    // y+1 ��ġ ���� ���� �� ��ġ ��ü
                    SwapBlock(initPos, Direction.Down);
                    Debug.Log("down");

                }
                else if (posX < 0 && (-1 < tan && tan < 1)) //2��и�� 3��и� ����
                {
                    // x+1 ��ġ ���� ���� �� ��ġ ��ü
                    SwapBlock(initPos, Direction.Left);
                    Debug.Log("left");

                }
                else if (posX > 0 && (-1 < tan && tan < 1)) //1��и�� 4��и� ����
                {
                    // x-1 ��ġ ���� ���� �� ��ġ ��ü 
                    SwapBlock(initPos, Direction.Right);
                    Debug.Log("right");

                }
                //�巡�� ���°� �ƴ��� �˸� => sweet load������ �������� �巡�װ� �ƴϰ� ������ ������ ���� �巡�״� ������
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
                if (blocks[pos.y - 1, pos.x] != null) //������ ���� �ƴ϶�� ������ ����
                {
                    //match.FindAllMatchBlock(blocks, pos.x, pos.y); //BFSŽ���� ���, pos�� newPos��� 

                    //�ʿ��� ������Ʈ �̵�
                    StartCoroutine(SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y - 1, pos.x].gameObject));

                    //�ڽ��� ����� �����ǰ� �־��ֱ� 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x, pos.y - 1);
                    blocks[pos.y - 1, pos.x].SetBlockPos(pos.x, pos.y);

                    //�ʿ����� ���� ��ȯ
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y - 1, pos.x];
                    blocks[pos.y - 1, pos.x] = temp;

                    matchList = match.FindDirectMatchBlock(blocks, newPos.x, newPos.y);
                    matchList2 = match.FindDirectMatchBlock(blocks, pos.x, pos.y);
                    //�Űܼ� ��Ʈ���� ���ٸ� �ٽ� ���ƿ�
                    //���� ���� ���� ���� ����� 3���� ���� ���
                    if ((matchList.sameRawBlockPosition.Count < 3 && matchList.sameColumnBlockPosition.Count < 3) &&
                        (matchList2.sameRawBlockPosition.Count < 3 && matchList2.sameColumnBlockPosition.Count < 3))
                    {
                        //��ġŲ ���� ����� ���� ����Ʈ�� ���� ���� ���
                        if ((matchList.tempColumnBlockPosition != matchList.tempRawBlockPosition) &&
                            (matchList2.tempColumnBlockPosition != matchList2.tempRawBlockPosition))
                        {
                            //�� ���ڸ� ����
                            StartCoroutine(Co_WaitReturnToOriginPos(newPos, pos));

                            //�ڽ��� ����� �����ǰ� �־��ֱ� 
                            blocks[pos.y, pos.x].SetBlockPos(pos.x, pos.y - 1);
                            blocks[pos.y - 1, pos.x].SetBlockPos(pos.x, pos.y);

                            //�ʿ����� ���� ��ȯ
                            temp = blocks[pos.y, pos.x];
                            blocks[pos.y, pos.x] = blocks[pos.y - 1, pos.x];
                            blocks[pos.y - 1, pos.x] = temp;
                        }
                    }
                }
                #region ��ġ����Ʈ Ȯ�ο� �����
                /*      Debug.Log("������ ī��Ʈ : " + matchList.sameRawBlockPosition.Count);
                      for (int i = 0; i < matchList.sameRawBlockPosition.Count; i++)
                      {
                          Debug.Log("������ x : " + matchList.sameRawBlockPosition[i][0] + ",  ������ y : " + matchList.sameRawBlockPosition[i][1]);
                      }
                      Debug.Log("������ ī��Ʈ : " + matchList.sameColumnBlockPosition.Count);
                      for (int i = 0; i < matchList.sameColumnBlockPosition.Count; i++)
                      {
                          Debug.Log("������ x : " + matchList.sameColumnBlockPosition[i][0] + ",  ������ y : " + matchList.sameColumnBlockPosition[i][1]);
                      }
                      Debug.Log("������ �ӽ� ī��Ʈ : " + matchList.TempRawBlockPosition.Count);
                      for (int i = 0; i < matchList.TempRawBlockPosition.Count; i++)
                      {
                          Debug.Log("������ �ӽ� x : " + matchList.TempRawBlockPosition[i][0] + ",  ������ y : " + matchList.TempRawBlockPosition[i][1]);
                      }
                      Debug.Log("������ �ӽ� ī��Ʈ : " + matchList.TempColumnBlockPosition.Count);
                      for (int i = 0; i < matchList.TempColumnBlockPosition.Count; i++)
                      {
                          Debug.Log("������ �ӽ� x : " + matchList.TempColumnBlockPosition[i][0] + ",  ������ y : " + matchList.TempColumnBlockPosition[i][1]);
                      }*/
                #endregion
                else
                    canTouch = true;
                break;
            case Direction.Down:
                newPos.y = pos.y + 1;
                if (blocks[pos.y + 1, pos.x] != null) //������ ���� �ƴ϶�� ������ ����
                {
                    //�ʿ��� ������Ʈ �̵�
                    StartCoroutine(SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y + 1, pos.x].gameObject));

                    //�ڽ��� ����� �����ǰ� �־��ֱ� 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x, pos.y + 1);
                    blocks[pos.y + 1, pos.x].SetBlockPos(pos.x, pos.y);
                    //�ʿ����� ���� ��ȯ
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y + 1, pos.x];
                    blocks[pos.y + 1, pos.x] = temp;

                    matchList = match.FindDirectMatchBlock(blocks, newPos.x, newPos.y);
                    matchList2 = match.FindDirectMatchBlock(blocks, pos.x, pos.y);
                    //�Űܼ� ��Ʈ���� ���ٸ� �ٽ� ���ƿ�
                    //���� ���� ���� ���� ����� 3���� ���� ���
                    if ((matchList.sameRawBlockPosition.Count < 3 && matchList.sameColumnBlockPosition.Count < 3) &&
                        (matchList2.sameRawBlockPosition.Count < 3 && matchList2.sameColumnBlockPosition.Count < 3))
                    {
                        //��ġŲ ���� ����� ���� ����Ʈ�� ���� ���� ���
                        if ((matchList.tempColumnBlockPosition != matchList.tempRawBlockPosition) &&
                            (matchList2.tempColumnBlockPosition != matchList2.tempRawBlockPosition))
                        {
                            //�ʿ��� ������Ʈ �̵�
                            StartCoroutine(Co_WaitReturnToOriginPos(newPos, pos));


                            //�ڽ��� ����� �����ǰ� �־��ֱ� 
                            blocks[pos.y, pos.x].SetBlockPos(pos.x, pos.y + 1);
                            blocks[pos.y + 1, pos.x].SetBlockPos(pos.x, pos.y);
                            //�ʿ����� ���� ��ȯ
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
                if (blocks[pos.y, pos.x - 1] != null) //������ ���� �ƴ϶�� ������ ����
                {

                    //�ʿ��� ������Ʈ �̵�
                    StartCoroutine(SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y, pos.x - 1].gameObject));

                    //�ڽ��� ����� �����ǰ� �־��ֱ� 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x - 1, pos.y);
                    blocks[pos.y, pos.x - 1].SetBlockPos(pos.x, pos.y);
                    //�ʿ����� ���� ��ȯ
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y, pos.x - 1];
                    blocks[pos.y, pos.x - 1] = temp;

                    matchList = match.FindDirectMatchBlock(blocks, newPos.x, newPos.y);
                    matchList2 = match.FindDirectMatchBlock(blocks, pos.x, pos.y);
                    //�Űܼ� ��Ʈ���� ���ٸ� �ٽ� ���ƿ�
                    //���� ���� ���� ���� ����� 3���� ���� ���
                    if ((matchList.sameRawBlockPosition.Count < 3 && matchList.sameColumnBlockPosition.Count < 3) &&
                        (matchList2.sameRawBlockPosition.Count < 3 && matchList2.sameColumnBlockPosition.Count < 3))
                    {
                        //��ġŲ ���� ����� ���� ����Ʈ�� ���� ���� ���
                        if ((matchList.tempColumnBlockPosition != matchList.tempRawBlockPosition) &&
                            (matchList2.tempColumnBlockPosition != matchList2.tempRawBlockPosition))
                        {
                            StartCoroutine(Co_WaitReturnToOriginPos(newPos, pos));
                            Debug.Log("����");


                            //�ڽ��� ����� �����ǰ� �־��ֱ� 
                            blocks[pos.y, pos.x].SetBlockPos(pos.x - 1, pos.y);
                            blocks[pos.y, pos.x - 1].SetBlockPos(pos.x, pos.y);
                            //�ʿ����� ���� ��ȯ
                            temp = blocks[pos.y, pos.x];
                            blocks[pos.y, pos.x] = blocks[pos.y, pos.x - 1];
                            blocks[pos.y, pos.x - 1] = temp;
                        }
                        Debug.Log(matchList.tempColumnBlockPosition == matchList.tempRawBlockPosition);

                    }
                }
                else
                    canTouch = true;
                break;
            case Direction.Right:
                newPos.x = pos.x + 1;
                if (blocks[pos.y, pos.x + 1] != null) //������ ���� �ƴ϶�� ������ ����
                {
                    //�ʿ��� ������Ʈ �̵�
                    StartCoroutine(SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y, pos.x + 1].gameObject));
                    //�ڽ��� ����� �����ǰ� �־��ֱ� 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x + 1, pos.y);
                    blocks[pos.y, pos.x + 1].SetBlockPos(pos.x, pos.y);
                    //�ʿ����� ���� ��ȯ
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y, pos.x + 1];
                    blocks[pos.y, pos.x + 1] = temp;

                    matchList = match.FindDirectMatchBlock(blocks, newPos.x, newPos.y);
                    matchList2 = match.FindDirectMatchBlock(blocks, pos.x, pos.y);
                    //�Űܼ� ��Ʈ���� ���ٸ� �ٽ� ���ƿ�
                    //���� ���� ���� ���� ����� 3���� ���� ���
                    if ((matchList.sameRawBlockPosition.Count < 3 && matchList.sameColumnBlockPosition.Count < 3) &&
                        (matchList2.sameRawBlockPosition.Count < 3 && matchList2.sameColumnBlockPosition.Count < 3))
                    {
                        //��ġŲ ���� ����� ���� ����Ʈ�� ���� ���� ���
                        if ((matchList.tempColumnBlockPosition != matchList.tempRawBlockPosition) &&
                            (matchList2.tempColumnBlockPosition != matchList2.tempRawBlockPosition))
                        {
                            //�ʿ��� ������Ʈ �̵�
                            StartCoroutine(Co_WaitReturnToOriginPos(newPos, pos));

                            //�ڽ��� ����� �����ǰ� �־��ֱ� 
                            blocks[pos.y, pos.x].SetBlockPos(pos.x + 1, pos.y);
                            blocks[pos.y, pos.x + 1].SetBlockPos(pos.x, pos.y);
                            //�ʿ����� ���� ��ȯ
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


        //��ġ�� ����Ʈ �ϳ��� ��ġ��
        //

        //�����ϴ� ����
        //StartCoroutine(Co_WaitGetScore(matchList, matchList2));
        //������ ���� ���� �Ŵ����� �ѱ��
        List<int[]> newList = SumMatchedList(matchList.matchedBlockPostion, matchList2.matchedBlockPostion);
        GameManager.Instance.SendSwapInfo(blocks, newList);
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

    IEnumerator SwapBlockPos(GameObject block1, GameObject block2)
    {
        //������ġ�� ������ġ ����
        Vector3 startPos1 = block1.transform.position, startPos2 = block2.transform.position;
        Vector3 endPos1 = startPos2, endPos2 = startPos1;
        //���� �ð��ȿ� �����ϰ� �ϱ����� ���� ����
        float lerpTime = 0.5f;
        float curTime = 0;

        //lerp�� �������� ���� ������ ��������
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
        //��Ʈ���� ���� �� �����ϴ� ����
        moving = true;
        StartCoroutine(SwapBlockPos(blocks[starPos.y, starPos.x].gameObject, blocks[endPos.y, endPos.x].gameObject));
    }
    IEnumerator Co_WaitGetScore(MatchList matchList, MatchList matchList2)
    {
        yield return new WaitUntil(() => moving == false);

        for (int i = 0; i < matchList.matchedBlockPostion.Count; i++)
        {
            blocks[matchList.matchedBlockPostion[i][1], matchList.matchedBlockPostion[i][0]].gameObject.SetActive(false);
        }
        for (int i = 0; i < matchList2.matchedBlockPostion.Count; i++)
        {
            blocks[matchList2.matchedBlockPostion[i][1], matchList2.matchedBlockPostion[i][0]].gameObject.SetActive(false);
        }
    }
}