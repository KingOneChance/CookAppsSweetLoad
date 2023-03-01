using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Func_Spawn : MonoBehaviour
{
    [SerializeField] private Block blockPrefab = null;
    [SerializeField] private Func_Swap func_swap = null;
    [SerializeField] private Func_Match func_match = null;
    [SerializeField] private List<Block> spawnListPool;
    [SerializeField] private List<Block> inactiveList;

    private List<int[]> matchList;
    private List<int[]> newMatchList = new List<int[]>();
    private Block[,] blocks;
    private void Awake()
    {
        func_match = FindObjectOfType<Func_Match>();
        func_swap = FindObjectOfType<Func_Swap>();
    }
    //�� �� ���� �ޱ�
    public void SetMapBlocksInfo(Block[,] blocks)
    {
        this.blocks = blocks;
    }
    //�ʱ� ���� ������Ʈ Ǯ�� �ֱ� 
    public void SetInitBlockPool(Block[,] blocks)
    {
        for (int i = 0; i < blocks.GetLength(1); i++)
        {
            for (int j = 0; j < blocks.GetLength(0); j++)
            {
                if (blocks[i, j] != null)
                    spawnListPool.Add(blocks[i, j]);
            }
        }
    }
    //��ġ�� �� ����Ʈ �ޱ�
    public void SetMatchedList(List<int[]> matchedList, List<int[]> munPos)
    {
        matchList = matchedList;
        DeleteBlocksScoreUP(matchList, munPos);
    }
    //��ġ�� ���� ����� ���� �ø��� �Լ�
    private void DeleteBlocksScoreUP(List<int[]> matchedList, List<int[]> munPos)
    {
        //��Ȱ��ȭ ������Ʈ Ǯ �ʱ�ȭ
        inactiveList.Clear();
        for (int i = 0; i < matchedList.Count; i++)
        {
            //matched list���� �޾ƿ� �ߺ�ó���� �Ǵ� ���, ���� ���� 
            if (blocks[matchedList[i][1], matchedList[i][0]].GetBlockMode() == BlockMode.Empty ||
                blocks[matchedList[i][1], matchedList[i][0]].GetBlockMode() == BlockMode.Munchkin)
                continue;
            //��ġŲ ���� �ִ� ��� 
            //��ġŲ ���� ��ġ�� ����Ʈ�� ��ġ�� �� 
            if (munPos.Count != 0)
            {
                for (int j = 0; j < munPos.Count; j++)
                {
                    if (munPos[j][0] == matchList[i][0] && munPos[j][1] == matchList[i][1])
                    {
                        //������ �ʰ� ������ Ű��°ɷ� Ȯ��
                        blocks[matchedList[i][1], matchedList[i][0]].SetBlockMode(BlockMode.Munchkin);
                        blocks[matchedList[i][1], matchedList[i][0]].SetBlockColor(BlockColor.MunChKin);
                        blocks[matchedList[i][1], matchedList[i][0]].gameObject.transform.localScale *= 1.2f;
                        GameManager.Instance.SetScore(40);
                        break;
                    }
                    else
                    {
                        //�� �����
                        blocks[matchedList[i][1], matchedList[i][0]].SetBlockMode(BlockMode.Empty);

                        //��Ȱ��ȭ ������Ʈ Ǯ�� ��Ƶα�
                        inactiveList.Add(blocks[matchedList[i][1], matchedList[i][0]]);
                        blocks[matchedList[i][1], matchedList[i][0]].gameObject.SetActive(false);
                        //���� �ø���
                        GameManager.Instance.SetScore(20);
                    }
                }
            }
            else
            {
                //�� �����
                blocks[matchedList[i][1], matchedList[i][0]].SetBlockMode(BlockMode.Empty);

                //��Ȱ��ȭ ������Ʈ Ǯ�� ��Ƶα�
                inactiveList.Add(blocks[matchedList[i][1], matchedList[i][0]]);
                blocks[matchedList[i][1], matchedList[i][0]].gameObject.SetActive(false);
                //���� �ø���
                GameManager.Instance.SetScore(20);
            }
            //�� ��ġ ����Ʈ ���� , 
            newMatchList.Add(new int[] { matchedList[i][0], matchedList[i][1] });

        }
        //���� �ִ� �ּ� y�� ã������ ����Ʈ
        List<int[]> minMax = new List<int[]>();
        //������ �̵� �����ؾ���
        for (int i = 1; i < 8; i++)
        {
            int max = -1, min = 20; // �ִ� �ּҸ� ����ϱ� ���� ������ �ʱⰪ
            for (int j = 1; j < 8; j++)
            {
                //������ Ȯ��
                if (blocks[j, i].gameObject.activeSelf == false)
                {
                    //j �� i��
                    if (j > max) max = j;
                    if (j < min) min = j;
                }
            }
            minMax.Add(new int[] { min, max });
        }

        //������ƮǮ ������ ���� �ѹ���
        int inactiveCount = 0;
        //���� ����� ������ŭ ������ �Ʒ��� �� ä��� 
        //minmax.count �� �� ����
        for (int i = 0; i < minMax.Count; i++)
        {
            //���� ���� ���� ��� ���� �� ���� 
            if (minMax[i][0] == 20 || minMax[i][1] == -1) continue;

            //�ִ� - �ּ� +1  => �ִ� �ּ� ���� ����
            int dif = minMax[i][1] - minMax[i][0] + 1;

            //���ϴ� ���� ���κ��� ������ ���� 
            int difPlus = dif;
            int emptyNum = 0;
            for (int j = minMax[i][0] - 1; j >= 1; j--)
            {
                //�߰��� ����ִ� ��� 
                if (blocks[j, i + 1].gameObject.activeSelf == false&& blocks[j, i + 1].GetBlockMode()!=BlockMode.Munchkin)
                {
                    difPlus++;
                    emptyNum++;
                    continue;
                }
                blocks[j, i + 1].SetBlockPos(i + 1, j + difPlus); //min => max�ڸ��� �̵� difPlus��ŭ �̵��߱� ����
                blocks[j, i + 1].MoveBlock();
                blocks[j + difPlus, i + 1] = blocks[j, i + 1];
            }

            //������ ����ִ� �� �κ����� ������ ä��� ���� 
            /*    for (int j = minMax[i][1]; j >= 1; j--)
                {
                    //�߰��� ����ִ� ��� 
                    if (blocks[j, i + 1].gameObject.activeSelf == false)
                    {
                        emptyNum++;
                        continue;
                    }
                    blocks[j, i + 1].SetBlockPos(i + 1, j + emptyNum); //min => max�ڸ��� �̵� difPlus��ŭ �̵��߱� ����
                    blocks[j, i + 1].MoveBlock();
                    blocks[j + emptyNum, i + 1] = blocks[j, i + 1];
                }*/

            if (dif != 1) //�� ���� �������� ���� �������� ��� 
            {
                //ä���� ���� ������ ĳ��
                int newDif = dif - emptyNum;

                //������ ������ �� �� ����ִ� ���κ� ä��� ����
                for (int j = 1; j <= newDif; j++)
                {

                    Debug.Log(inactiveCount);
                    inactiveList[inactiveCount].gameObject.SetActive(true);
                    blocks[j, i + 1] = null;
                    inactiveList[inactiveCount].gameObject.transform.position = new Vector2(i - 3, 4 + newDif);
                    inactiveList[inactiveCount].SetBlockRandomColor();
                    inactiveList[inactiveCount].SetBlockMode(BlockMode.Normal);
                    inactiveList[inactiveCount].SetBlockPos(i + 1, j);
                    inactiveList[inactiveCount].MoveBlock();

                    blocks[j, i + 1] = inactiveList[inactiveCount];
                    inactiveCount++;
                }
                //������ ������ �� �� ����ִ� ���κ� ä��� ����
                /*    for (int j = 1; j <= emptyNum; j++)
                    {
                        Debug.Log("dif : " + dif + ", emptynum" + emptyNum);
                        Debug.Log(inactiveCount);
                        inactiveList[inactiveCount].gameObject.SetActive(true);
                        blocks[j, i + 1] = null;
                        inactiveList[inactiveCount].gameObject.transform.position = new Vector2(i - 3, 4 + emptyNum - j);
                        inactiveList[inactiveCount].SetBlockRandomColor();
                        inactiveList[inactiveCount].SetBlockMode(BlockMode.Normal);
                        inactiveList[inactiveCount].SetBlockPos(i + 1, j);
                        inactiveList[inactiveCount].MoveBlock();

                        blocks[j, i + 1] = inactiveList[inactiveCount];
                        inactiveCount++;
                    }*/
            }
            else //�ѿ��� �ϳ��� ���� �������� ��� 
            {
            
                Debug.Log("dif : " + dif + ", emptynum" + emptyNum);
                Debug.Log(inactiveCount);
                inactiveList[inactiveCount].gameObject.SetActive(true);
                inactiveList[inactiveCount].gameObject.transform.position = new Vector2(i - 3, 4);
                inactiveList[inactiveCount].SetBlockRandomColor();
                inactiveList[inactiveCount].SetBlockPos(i + 1, 1);
                inactiveList[inactiveCount].SetBlockMode(BlockMode.Normal);
                inactiveList[inactiveCount].MoveBlock();

                blocks[1, i + 1] = inactiveList[inactiveCount];
                inactiveCount++;
            }
        }

        StartCoroutine(Co_AutoCheck(blocks));
    }
    //�ֺ��� ��ġ �Ǵ°� �ִ��� Ȯ���ϴ� �Լ� 
    IEnumerator Co_AutoCheck(Block[,] block)
    {

        yield return null;
        List<int[]> matchedBlockList = new List<int[]>();
        List<int[]> munPos = new List<int[]>();
        //������ ���� �ִ��� Ȯ���ϱ� 
        for (int i = 1; i < 8; i++)
        {
            for (int j = 1; j < 8; j++)
            {
                Debug.Log("�ڵ� ��ġ ������");

                MatchList newlist = new MatchList();
                newlist = func_match.AutoMatchblock(block, j, i);

                if (newlist.isMunchkin == true)
                    munPos.Add(newlist.munchkinBlockPos);

                for (int k = 0; k < newlist.matchedBlockPostion.Count; k++)
                {
                    matchedBlockList.Add(newlist.matchedBlockPostion[k]);
                }
            }
        }
        //������ ��� �̵��� �� �����̰� �ϴ� ���� 
        for (int i = 1; i < 8; i++)
        {
            for (int j = 1; j < 8; j++)
            {
                yield return new WaitUntil(() => blocks[i, j].ismoving == false);
            }
        }

        if (matchedBlockList.Count > 0)
            DeleteBlocksScoreUP(matchedBlockList, munPos);
    }
    //����Ʈ �ȿ� �ش� ���� �ִ��� ã�� �Լ�
    private bool IsContains(List<Block> list, Block block)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].gameObject == block.gameObject)
            {
                return true;
            }
        }
        return false;
    }
}
