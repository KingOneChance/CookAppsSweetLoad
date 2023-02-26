using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Func_Spawn : MonoBehaviour
{
    [SerializeField] private Block blockPrefab;
    [SerializeField] private List<Block> spawnListPool;
    [SerializeField] private List<Block> inactiveList;

    private List<int[]> matchList;
    private List<int[]> newMatchList = new List<int[]>();
    private Block[,] blocks;
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
    public void SetMatchedList(List<int[]> matchedList)
    {
        matchList = matchedList;
        DeleteBlocksScoreUP(matchList);
    }
    //��ġ�� ���� ����� ���� �ø���
    private void DeleteBlocksScoreUP(List<int[]> matchedList)
    {
        //��Ȱ��ȭ ������Ʈ Ǯ �ʱ�ȭ
        inactiveList.Clear();
        for (int i = 0; i < matchedList.Count; i++)
        {
            //�ߺ��� ��ǥ���� �ƴ� ���
            if (blocks[matchedList[i][1], matchedList[i][0]].GetBlockMode() != BlockMode.Empty)
            {
                //�� �����
                blocks[matchedList[i][1], matchedList[i][0]].SetBlockMode(BlockMode.Empty);
                blocks[matchedList[i][1], matchedList[i][0]].gameObject.SetActive(false);

                //��Ȱ��ȭ ������Ʈ Ǯ�� ��Ƶα�
                inactiveList.Add(blocks[matchedList[i][1], matchedList[i][0]]);

                //���� �ø���
                GameManager.Instance.SetScore(20);

                //�� ��ġ ����Ʈ ���� , 
                newMatchList.Add(new int[] { matchedList[i][0], matchedList[i][1] });
            }
        }
        //���� �ִ� �ּ� y�� ã������ ����Ʈ
        List<int[]> minMax = new List<int[]>();
        //������ �̵� �����ؾ���
        for (int i = 1; i < 8; i++)
        {
            int max = -1, min = 20;
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
            Debug.Log(minMax[i - 1][0] + " vs " + minMax[i - 1][1]);
        }

        //������ƮǮ ������ ���� �ѹ���
        int inactiveCount = 0;
        //���� ����� ������ŭ ������ �Ʒ��� �� ä��� 
        for (int i = 0; i < minMax.Count; i++) //minmax.count �� �� ����
        {
            if (minMax[i][0] == 20 || minMax[i][1] == -1) continue; //���� ���� ���� ��� 

            int dif = minMax[i][1] - minMax[i][0] + 1; //�ִ� - �ּ� +1  => �ִ� �ּ� ���� ����
           
            //���� ���κ��� ������ ����
            for (int j = minMax[i][0]; j >= 1; j--)
            {
                blocks[j, i + 1].SetBlockPos(i + 1, j + dif); //min => max�ڸ��� �̵� dif��ŭ �̵��߱� ����
                blocks[j, i + 1].MoveBlock();
                blocks[j + dif, i + 1] = blocks[j, i + 1];
            }
            //������ ������ �� �� ����ִ� ���κ� ä��� ����
            for (int j = 1; j <= dif; j++)
            {
                blocks[j, i+1] = null;
                inactiveList[inactiveCount].gameObject.SetActive(true);
                inactiveList[inactiveCount].gameObject.transform.position = new Vector2(i-3,4+dif-j);
                inactiveList[inactiveCount].SetBlockRandomColor();
                inactiveList[inactiveCount].SetBlockPos(i + 1, j);
                inactiveList[inactiveCount].SetBlockMode(BlockMode.Normal);
                inactiveList[inactiveCount].MoveBlock();

                // Debug.Log("inactiveList[inactiveCount] x , y: " + inactiveList[inactiveCount].GetBlockPosX() +" ,  " + inactiveList[inactiveCount].GetBlockPosY());

                blocks[j, i+1] = inactiveList[inactiveCount];
             
                inactiveCount++;
            }
        }
        #region blocks ����׹�
        /*for (int t = 1; t < 8; t++)
        {
            for (int y = 1; y < 8; y++)
            {
                if (y == 1)
                    Debug.Log("blocks  " + t + ", " + y + " �� posx : " + blocks[t, y].GetBlockPosX() + ", posy : " + blocks[t, y].GetBlockPosY());
            }
        }*/
        #endregion
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

    /// <summary>
    /// Ŭ����� ����ŭ �� ���� �� �����ϱ�
    /// x = blockPos.x, y = blockPos.y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void CreateNewBlock(int x, int y, int count)
    {

    }

    IEnumerator Co_MoveBlock(GameObject blockObject, int x, int y)
    {
        //������ġ�� ������ġ ����
        Vector3 startPos = blockObject.transform.position;
        Vector3 endPos = blockObject.transform.position + Vector3.down * (4 - y);
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
    }


    private bool CheckBlock(Vector3 position)
    {

        return true;
    }
    //������ ������ ������ �����Ű��

}
