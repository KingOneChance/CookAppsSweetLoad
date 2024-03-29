using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Func_Swap : MonoBehaviour
{
    //ù ���� ���� �ݶ��̴��� �� ��° ���� �ݶ��̴��� ��ġ�� �ٲ۴�. 
    //���� ��ġ�� �������� X ��翡�� ��� ������ �̵��ϴ��� üũ�ϴ� ���� �ۼ� �ʿ�

    [SerializeField] private bool canTouch = true; //�ܺο��� Ž�� �˰������ ���� �� �����ı��� �ٳ����� ȣ��Ǿ� Ʈ�簡 �ȴ�.
    [SerializeField] private bool isDrag = false;
    [SerializeField] private bool moving = false;
    [SerializeField] public bool gameOver = false;
    [SerializeField] private GameObject myBlock = null;
    [SerializeField] private GameObject swapblock = null;
    [SerializeField] private Block blockTemp = null;

    [SerializeField] private Func_Match func_Match = null;
    [SerializeField] private Map_Information mapInfo = null;

    private Vector2 initialPosition;
    private BlockPos initPos;
    private Block[,] blocks;

    private void Awake()
    {
        mapInfo = FindObjectOfType<Map_Information>();
        func_Match = FindObjectOfType<Func_Match>();
    }
    private void Start()
    {
        blocks = mapInfo.GetBlocksInfo();
    }
    void Update()
    {
        if (gameOver == true) return;
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

                myBlock.TryGetComponent<Block>(out blockTemp); //���� ���� Ž��
                initPos = blockTemp.GetBlockPos(); //���� ������ �ʿ����� ��ġ 
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

                if (posY > 0 && (tan > 1 || tan < -1)) // 1��и�� 2��и� ���� 
                {
                    // y-1 ��ġ ������ ���� ���� ��ġ ��ü
                    if (blockTemp.GetBlockMode() == BlockMode.Normal)
                        SwapBlock(initPos, Direction.Up);
                    else if (blockTemp.GetBlockMode() == BlockMode.Munchkin)
                        SwapMunchkin(blockTemp, Direction.Up);
                }
                else if (posY < 0 && (tan > 1 || tan < -1)) //3��и�� 4��и� ����
                {
                    // y+1 ��ġ ������ ���� ���� ��ġ ��ü
                    if (blockTemp.GetBlockMode() == BlockMode.Normal)
                        SwapBlock(initPos, Direction.Down);
                    else if (blockTemp.GetBlockMode() == BlockMode.Munchkin)
                        SwapMunchkin(blockTemp, Direction.Down);

                }
                else if (posX < 0 && (-1 < tan && tan < 1)) //2��и�� 3��и� ����
                {
                    // x+1 ��ġ ������ ���� ���� ��ġ ��ü
                    if (blockTemp.GetBlockMode() == BlockMode.Normal)
                        SwapBlock(initPos, Direction.Left);
                    else if (blockTemp.GetBlockMode() == BlockMode.Munchkin)
                        SwapMunchkin(blockTemp, Direction.Left);

                }
                else if (posX > 0 && (-1 < tan && tan < 1)) //1��и�� 4��и� ����
                {
                    // x-1 ��ġ ������ ���� ���� ��ġ ��ü 
                    if (blockTemp.GetBlockMode() == BlockMode.Normal)
                        SwapBlock(initPos, Direction.Right);
                    else if (blockTemp.GetBlockMode() == BlockMode.Munchkin)
                        SwapMunchkin(blockTemp, Direction.Right);

                }
                //�巡�� ���°� �ƴ��� �˸� => sweet load������ �������� �巡�װ� �ƴϰ� ������ ������ ���� �巡�״� ����
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
                    StartCoroutine(Co_SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y - 1, pos.x].gameObject));

                    //�ڽ��� ����� �����ǰ� �־��ֱ� 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x, pos.y - 1);
                    blocks[pos.y - 1, pos.x].SetBlockPos(pos.x, pos.y);

                    //�ʿ����� ���� ��ȯ
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y - 1, pos.x];
                    blocks[pos.y - 1, pos.x] = temp;

                    matchList = func_Match.FindDirectMatchBlock(blocks, newPos.x, newPos.y);
                    matchList2 = func_Match.FindDirectMatchBlock(blocks, pos.x, pos.y);
                    //�Űܼ� ��Ʈ���� ���ٸ� �ٽ� ���ƿ�
                    //���� ���� ���� ���� ������ 3���� ���� ���
                    if (matchList.matchedBlockPostion.Count == 0 && matchList2.matchedBlockPostion.Count == 0)
                    {
                        //��ġŲ ������ ����� ���� ����Ʈ�� ���� ���� ���
                        if ((matchList.tempColumnBlockPosition != matchList.tempRawBlockPosition) &&
                            (matchList2.tempColumnBlockPosition != matchList2.tempRawBlockPosition))
                        {
                            //���� ���ڸ� ����
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
                else
                    canTouch = true;
                break;
            case Direction.Down:
                newPos.y = pos.y + 1;
                if (blocks[pos.y + 1, pos.x] != null) //������ ���� �ƴ϶�� ������ ����
                {
                    //�ʿ��� ������Ʈ �̵�
                    StartCoroutine(Co_SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y + 1, pos.x].gameObject));

                    //�ڽ��� ����� �����ǰ� �־��ֱ� 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x, pos.y + 1);
                    blocks[pos.y + 1, pos.x].SetBlockPos(pos.x, pos.y);
                    //�ʿ����� ���� ��ȯ
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y + 1, pos.x];
                    blocks[pos.y + 1, pos.x] = temp;

                    matchList = func_Match.FindDirectMatchBlock(blocks, newPos.x, newPos.y);
                    matchList2 = func_Match.FindDirectMatchBlock(blocks, pos.x, pos.y);
                    //�Űܼ� ��Ʈ���� ���ٸ� �ٽ� ���ƿ�
                    //���� ���� ���� ���� ������ 3���� ���� ���
                    if (matchList.matchedBlockPostion.Count == 0 && matchList2.matchedBlockPostion.Count == 0)
                    {
                        //��ġŲ ������ ����� ���� ����Ʈ�� ���� ���� ���
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
                    StartCoroutine(Co_SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y, pos.x - 1].gameObject));

                    //�ڽ��� ����� �����ǰ� �־��ֱ� 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x - 1, pos.y);
                    blocks[pos.y, pos.x - 1].SetBlockPos(pos.x, pos.y);
                    //�ʿ����� ���� ��ȯ
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y, pos.x - 1];
                    blocks[pos.y, pos.x - 1] = temp;

                    matchList = func_Match.FindDirectMatchBlock(blocks, newPos.x, newPos.y);
                    matchList2 = func_Match.FindDirectMatchBlock(blocks, pos.x, pos.y);
                    //�Űܼ� ��Ʈ���� ���ٸ� �ٽ� ���ƿ�
                    //���� ���� ���� ���� ������ 3���� ���� ���
                    if (matchList.matchedBlockPostion.Count == 0 && matchList2.matchedBlockPostion.Count == 0)
                    {
                        //��ġŲ ������ ����� ���� ����Ʈ�� ���� ���� ���
                        if ((matchList.tempColumnBlockPosition != matchList.tempRawBlockPosition) &&
                            (matchList2.tempColumnBlockPosition != matchList2.tempRawBlockPosition))
                        {
                            StartCoroutine(Co_WaitReturnToOriginPos(newPos, pos));

                            //�ڽ��� ����� �����ǰ� �־��ֱ� 
                            blocks[pos.y, pos.x].SetBlockPos(pos.x - 1, pos.y);
                            blocks[pos.y, pos.x - 1].SetBlockPos(pos.x, pos.y);
                            //�ʿ����� ���� ��ȯ
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
                if (blocks[pos.y, pos.x + 1] != null) //������ ���� �ƴ϶�� ������ ����
                {
                    //�ʿ��� ������Ʈ �̵�
                    StartCoroutine(Co_SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y, pos.x + 1].gameObject));
                    //�ڽ��� ����� �����ǰ� �־��ֱ� 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x + 1, pos.y);
                    blocks[pos.y, pos.x + 1].SetBlockPos(pos.x, pos.y);
                    //�ʿ����� ���� ��ȯ
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y, pos.x + 1];
                    blocks[pos.y, pos.x + 1] = temp;

                    matchList = func_Match.FindDirectMatchBlock(blocks, newPos.x, newPos.y);
                    matchList2 = func_Match.FindDirectMatchBlock(blocks, pos.x, pos.y);
                    //�Űܼ� ��Ʈ���� ���ٸ� �ٽ� ���ƿ�
                    //���� ���� ���� ���� ������ 3���� ���� ���
                    if (matchList.matchedBlockPostion.Count == 0 && matchList2.matchedBlockPostion.Count == 0)
                    {
                        //��ġŲ ������ ����� ���� ����Ʈ�� ���� ���� ���
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
        //�����ϴ� ����
        //StartCoroutine(Co_WaitGetScore(matchList, matchList2));
        //������ ���� ���� �Ŵ����� �ѱ��
        List<int[]> newList = SumMatchedList(matchList.matchedBlockPostion, matchList2.matchedBlockPostion);
        List<int[]> munPos = new List<int[]>();

        if (matchList.isMunchkin == true)
            munPos.Add(matchList.munchkinBlockPos);
        if (matchList2.isMunchkin == true)
            munPos.Add(matchList2.munchkinBlockPos);

        StartCoroutine(Co_SendToGameManage(newList, munPos));
    }
    public void SwapMunchkin(Block block, Direction dir)
    {
        StartCoroutine(Co_MoveMunchkin(block, dir));
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
    public void AutoSwapBlock(Block[,] blocks, int x, int y)
    {
        MatchList matchList = func_Match.FindDirectMatchBlock(blocks, x, y);
        List<int[]> newList = matchList.matchedBlockPostion;
        List<int[]> munPos = new List<int[]>();
        if (matchList.isMunchkin == true)
            munPos.Add(matchList.munchkinBlockPos);
        StartCoroutine(Co_SendToGameManage(newList, munPos));
    }
    IEnumerator Co_SwapBlockPos(GameObject block1, GameObject block2)
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
        StartCoroutine(Co_SwapBlockPos(blocks[starPos.y, starPos.x].gameObject, blocks[endPos.y, endPos.x].gameObject));
    }
    IEnumerator Co_SendToGameManage(List<int[]> newList, List<int[]> munPos)
    {
        yield return new WaitUntil(() => moving == false);
        GameManager.Instance.SendSwapInfo(blocks, newList, munPos);
    }
    IEnumerator Co_MoveMunchkin(Block block, Direction dir)
    {
        switch (dir)
        {
            case Direction.Left:
                block.ismoving = true;
                Debug.Log("��ġŲ �̵���");
                block.MoveMuchkin(-4, (int)block.gameObject.transform.position.y);
                yield return new WaitUntil(() => block.ismoving == false);
                Debug.Log("��ġŲ �̵��Ϸ�");
                break;
            case Direction.Right:
                block.ismoving = true;
                block.MoveMuchkin(4, (int)block.gameObject.transform.position.y);
                yield return new WaitUntil(() => block.ismoving == false);
                break;
            case Direction.Up:
                block.ismoving = true;
                block.MoveMuchkin((int)block.gameObject.transform.position.x, 4);
                yield return new WaitUntil(() => block.ismoving == false);
                break;
            case Direction.Down:
                block.ismoving = true;
                block.MoveMuchkin((int)block.gameObject.transform.position.x, -4);
                yield return new WaitUntil(() => block.ismoving == false);
                break;
        }
        List<int[]> temp = new List<int[]>();
        List<int[]> nullList = new List<int[]>();
        temp = func_Match.HitFromMunchkin(block, dir);
        GameManager.Instance.SendSwapInfo(blocks, temp, nullList);
        canTouch = true;

    }
}