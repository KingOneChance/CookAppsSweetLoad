using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchList
{
    public List<int[]> sameRawBlockPosition = new List<int[]>();
    public List<int[]> tempRawBlockPosition = new List<int[]>();
    public List<int[]> sameColumnBlockPosition = new List<int[]>();
    public List<int[]> tempColumnBlockPosition = new List<int[]>();
    public List<int[]> matchedBlockPostion = new List<int[]>();
    public int[] munchkinBlockPos = new int[2];
    public bool isMunchkin = false;
}

public class Func_Match : MonoBehaviour
{
    //BFS�˰����� ���ؼ� �ֺ� ��� ��ü Ž�� => �밢���� ��쵵 �� ����.. ����
    public void FindAllMatchBlock(Block[,] blocks, int x, int y)
    {
        Queue<int[]> targetPos = new Queue<int[]>();
        List<int[]> visitedPos = new List<int[]>();
        List<int[]> sameblockPos = new List<int[]>();


        targetPos.Enqueue(new int[] { x, y });
        sameblockPos.Add(new int[] { x, y });


        int[] dirX = { 0, 0, -1, 1 };
        int[] dirY = { -1, 1, 0, 0 };
        while (targetPos.Count > 0)
        {
            int[] nowPos = targetPos.Dequeue();
            visitedPos.Add(nowPos);

            for (int i = 0; i < 4; i++) // 4���� Ž��
            {

                int[] temp = { nowPos[0] + dirX[i], nowPos[1] + dirY[i] };
                //�湮�� ���̶�� ���� ����

                if (VisitContains(visitedPos, temp)) continue;

                if (blocks[nowPos[1], nowPos[0]] == null || blocks[temp[1], temp[0]] == null) continue;

                if (temp[0] < 0 || temp[0] >= blocks.GetLength(1) || temp[1] < 0 || temp[1] >= blocks.GetLength(0)) continue;

                //�ڽŰ� �ֺ� ����� ���� ������ Ȯ��
                if (blocks[nowPos[1], nowPos[0]].GetBlockColor() == blocks[temp[1], temp[0]].GetBlockColor())
                {
                    targetPos.Enqueue(temp);
                    sameblockPos.Add(temp);
                }
                else //�ٸ��� �湮 üũ
                {
                    visitedPos.Add(temp);
                }
            }
        }
        //�ϴܲ��� Ȯ���غ���
        if (sameblockPos.Count > 2)
            for (int i = 0; i < sameblockPos.Count; i++)
            {
                blocks[sameblockPos[i][1], sameblockPos[i][0]].gameObject.SetActive(false);
            }
    }

    public MatchList FindDirectMatchBlock(Block[,] blocks, int x, int y)
    {
        List<int[]> sameRawBlockPos = new List<int[]>();
        List<int[]> TempRawBlockPos = new List<int[]>();
        List<int[]> sameColumnBlockPos = new List<int[]>();
        List<int[]> TempColumnBlockPos = new List<int[]>();
        MatchList matchList = new MatchList();
        int[] startPos = { x, y };

        sameRawBlockPos.Add(startPos);
        sameColumnBlockPos.Add(startPos);

        //������� BFS�� ����. ������ ���������κ��� ���� ���� ���� �켱 Ž���� �����Ѵ�.
        FIndLeftRawDFS(blocks, sameRawBlockPos, TempColumnBlockPos, startPos, 1);
        FIndRightRawDFS(blocks, sameRawBlockPos, TempColumnBlockPos, startPos, 1);
        FIndUpColumnDFS(blocks, sameColumnBlockPos, TempRawBlockPos, startPos, 1);
        FIndDownColumnDFS(blocks, sameColumnBlockPos, TempRawBlockPos, startPos, 1);

        matchList.sameColumnBlockPosition = sameColumnBlockPos;
        matchList.tempColumnBlockPosition = TempColumnBlockPos;
        matchList.sameRawBlockPosition = sameRawBlockPos;
        matchList.tempRawBlockPosition = TempRawBlockPos;

        //��ġŲ ���� �� �� �ִ��� �Ǻ�
        for (int i = 0; i < TempColumnBlockPos.Count; i++) //���� Ž���ϱ� 
        {
            for (int j = 0; j < TempRawBlockPos.Count; j++)
            {
                if ((TempColumnBlockPos[i][0] == TempRawBlockPos[j][0]) &&
                    (TempColumnBlockPos[i][1] == TempRawBlockPos[j][1]))
                {
                    matchList.munchkinBlockPos = startPos;
                    matchList.isMunchkin = true;
                }
            }
        }

        FindMatchedBlock(blocks, matchList);
        return matchList;
    }
    //���� Ž�� ����
    private void FIndLeftRawDFS(Block[,] block, List<int[]> sameLine, List<int[]> tempLine, int[] startPos, int idx)
    {
        if (block[startPos[1], startPos[0] - idx].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
        {
            sameLine.Add(new int[] { startPos[0] - idx, startPos[1] });
            //�߰��� ��, �Ʒ� Ž�� �� ������ ����, ��ġŲ ���� ����
            if (block[startPos[1] + 1, startPos[0] - idx] != null)//�Ʒ�
            {
                if (block[startPos[1] + 1, startPos[0] - idx].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                {
                    tempLine.Add(new int[] { startPos[0] - idx, startPos[1] + 1 });
                }
            }
            if (block[startPos[1] - 1, startPos[0] - idx] != null)//��
            {
                if (block[startPos[1] - 1, startPos[0] - idx].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                {
                    tempLine.Add(new int[] { startPos[0] - idx, startPos[1] - 1 });
                }
            }
            FIndLeftRawDFS(block, sameLine, tempLine, startPos, idx + 1);
        }
        else return;
    }
    //���� Ž�� ������
    private void FIndRightRawDFS(Block[,] block, List<int[]> sameLine, List<int[]> tempLine, int[] startPos, int idx)
    {
        //����������
        if (block[startPos[1], startPos[0] + idx] != null)
        {
            if (block[startPos[1], startPos[0] + idx].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
            {
                sameLine.Add(new int[] { startPos[0] + idx, startPos[1] });
                //�߰��� ��, �Ʒ� Ž�� �� ������ ����, ��ġŲ ���� ����
                if (block[startPos[1] + 1, startPos[0] + idx] != null)//�Ʒ�
                {
                    if (block[startPos[1] + 1, startPos[0] + idx].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                    {
                        tempLine.Add(new int[] { startPos[0] + idx, startPos[1] + 1 });
                    }
                }
                if (block[startPos[1] - 1, startPos[0] + idx] != null)//��
                {
                    if (block[startPos[1] - 1, startPos[0] + idx].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                    {
                        tempLine.Add(new int[] { startPos[0] + idx, startPos[1] - 1 });
                    }
                }
                FIndRightRawDFS(block, sameLine, tempLine, startPos, idx + 1);
            }
        }
        else return;
    }
    //���� Ž�� ����
    private void FIndUpColumnDFS(Block[,] block, List<int[]> sameLine, List<int[]> tempLine, int[] startPos, int idx)
    {
        //������
        if (block[startPos[1] - idx, startPos[0]] != null)
        {
            if (block[startPos[1] - idx, startPos[0]].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
            {
                sameLine.Add(new int[] { startPos[0], startPos[1] - idx });
                //�߰��� ��, �� Ž�� �� ������ ����, ��ġŲ ���� ����
                if (block[startPos[1] - idx, startPos[0] + 1] != null)//������
                {
                    if (block[startPos[1] - idx, startPos[0] + 1].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                    {
                        tempLine.Add(new int[] { startPos[0] + 1, startPos[1] - idx });
                    }
                }
                if (block[startPos[1] - idx, startPos[0] - 1] != null)//����
                {
                    if (block[startPos[1] - idx, startPos[0] - 1].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                    {
                        tempLine.Add(new int[] { startPos[0] - 1, startPos[1] - idx });
                    }
                }
                FIndUpColumnDFS(block, sameLine, tempLine, startPos, idx + 1);
            }
        }
        else return;
    }
    //���� Ž�� �Ʒ���
    private void FIndDownColumnDFS(Block[,] block, List<int[]> sameLine, List<int[]> tempLine, int[] startPos, int idx)
    {
        //�Ʒ�����
        if (block[startPos[1] + idx, startPos[0]] != null)
        {
            if (block[startPos[1] + idx, startPos[0]].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
            {
                sameLine.Add(new int[] { startPos[0], startPos[1] + idx });
                //�߰��� ��, �� Ž�� �� ������ ����, ��ġŲ ���� ����
                if (block[startPos[1] + idx, startPos[0] + 1] != null)//������
                {
                    if (block[startPos[1] + idx, startPos[0] + 1].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                    {
                        tempLine.Add(new int[] { startPos[0] + 1, startPos[1] + idx });
                    }
                }
                if (block[startPos[1] + idx, startPos[0] - 1] != null)//����
                {
                    if (block[startPos[1] + idx, startPos[0] - 1].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                    {
                        tempLine.Add(new int[] { startPos[0] - 1, startPos[1] + idx });
                    }
                }
                FIndDownColumnDFS(block, sameLine, tempLine, startPos, idx + 1);
            }
        }
        else return;
    }
    //�湮 ��Ͽ� �ִ��� Ȯ���ϴ� �Լ�  BFS�������� ���
    private bool VisitContains(List<int[]> visitedPos, int[] nowPos)
    {
        for (int j = 0; j < visitedPos.Count; j++)
        {
            if (visitedPos[j][0] == nowPos[0] && visitedPos[j][1] == nowPos[1])
            {
                return true;
            }
        }
        return false;
    }
    //��ġ�� ���� ����ֱ�, ��Ʈ�� ����
    public void FindMatchedBlock(Block[,] blocks, MatchList matchList)
    {
        //���θ� ���� ��
        if (matchList.sameRawBlockPosition.Count > 2 && matchList.sameColumnBlockPosition.Count < 2)
        {
            for (int i = 0; i < matchList.sameRawBlockPosition.Count; i++)
            {
                //��ġ�� �� ��Ͼȿ� �ߺ� ������ �Ǵ��� üũ
                if (CheckContained(matchList.matchedBlockPostion, matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1]) == false)
                    matchList.matchedBlockPostion.Add(new int[] { matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1] });
            }
        }
        //���θ� ���� �� ���θ� ������, 
        else if (matchList.sameColumnBlockPosition.Count > 2 && matchList.sameRawBlockPosition.Count < 2)
        {
            for (int i = 0; i < matchList.sameColumnBlockPosition.Count; i++)
            {
                //��ġ�� �� ��Ͼȿ� �ߺ� ������ �Ǵ��� üũ
                if (CheckContained(matchList.matchedBlockPostion, matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1]) == false)
                    matchList.matchedBlockPostion.Add(new int[] { matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1] });
            }
        }
        //��ġŲ ��� ���� 
        else if (matchList.sameColumnBlockPosition.Count >= 2 && matchList.sameRawBlockPosition.Count >= 2)
        {
            //���μ��� �Բ� ������,
            if ((matchList.sameColumnBlockPosition.Count > 2 && matchList.sameRawBlockPosition.Count > 2))
            {
                if (matchList.isMunchkin == true) matchList.matchedBlockPostion.Add(matchList.tempColumnBlockPosition[0]);
                //���� ��ġ�� 2�� ����ǹǷ� ����Ʈ �ϳ��� 1���� ����. (�ϴ�  ������ �־��Ѵ�. )
                for (int i = 1; i < matchList.sameRawBlockPosition.Count; i++)
                {
                    if (CheckContained(matchList.matchedBlockPostion, matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1]) == false)
                        matchList.matchedBlockPostion.Add(new int[] { matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1] });
                }
                for (int i = 0; i < matchList.sameColumnBlockPosition.Count; i++)
                {
                    if (CheckContained(matchList.matchedBlockPostion, matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1]) == false)
                        matchList.matchedBlockPostion.Add(new int[] { matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1] });
                }
            }
            //2X2�� ������ 
            else if (matchList.sameRawBlockPosition.Count == 2 && matchList.sameColumnBlockPosition.Count == 2)
            {
                if (matchList.isMunchkin == true)
                {
                    if (matchList.isMunchkin == true) matchList.matchedBlockPostion.Add(matchList.tempColumnBlockPosition[0]);

                    for (int i = 1; i < matchList.sameRawBlockPosition.Count; i++)
                    {
                        if (CheckContained(matchList.matchedBlockPostion, matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1]) == false)
                            matchList.matchedBlockPostion.Add(new int[] { matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1] });
                    }
                    for (int i = 0; i < matchList.sameColumnBlockPosition.Count; i++)
                    {
                        if (CheckContained(matchList.matchedBlockPostion, matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1]) == false)
                            matchList.matchedBlockPostion.Add(new int[] { matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1] });
                    }
                }
            }
            //���ʸ� 3 �̻� �϶� 
            else if (matchList.sameRawBlockPosition.Count > 2 && matchList.sameColumnBlockPosition.Count == 2)
            {
                if (matchList.isMunchkin == true) matchList.matchedBlockPostion.Add(matchList.tempColumnBlockPosition[0]);
                for (int i = 0; i < matchList.sameColumnBlockPosition.Count; i++)
                {
                    if (CheckContained(matchList.matchedBlockPostion, matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1]) == false)
                        matchList.matchedBlockPostion.Add(new int[] { matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1] });
                }
                for (int i = 0; i < matchList.sameRawBlockPosition.Count; i++)
                {
                    if (CheckContained(matchList.matchedBlockPostion, matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1]) == false)
                        matchList.matchedBlockPostion.Add(new int[] { matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1] });
                }
            }
            //���ʸ� 3 �̻� �϶� 
            else if (matchList.sameRawBlockPosition.Count == 2 && matchList.sameColumnBlockPosition.Count > 2)
            {
                if (matchList.isMunchkin == true) matchList.matchedBlockPostion.Add(matchList.tempColumnBlockPosition[0]);
                for (int i = 0; i < matchList.sameColumnBlockPosition.Count; i++)
                {
                    if (CheckContained(matchList.matchedBlockPostion, matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1]) == false)
                        matchList.matchedBlockPostion.Add(new int[] { matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1] });
                }
                for (int i = 0; i < matchList.sameRawBlockPosition.Count; i++)
                {
                    if (CheckContained(matchList.matchedBlockPostion, matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1]) == false)
                        matchList.matchedBlockPostion.Add(new int[] { matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1] });
                }
            }
        }

        //Ư�� ���� ���� ��� 
    }
    //�ڵ� ��ġ ���
    public MatchList AutoMatchblock(Block[,] blocks, int x, int y)
    {
        List<int[]> sameRawBlockPos = new List<int[]>();
        List<int[]> TempRawBlockPos = new List<int[]>();
        List<int[]> sameColumnBlockPos = new List<int[]>();
        List<int[]> TempColumnBlockPos = new List<int[]>();
        MatchList matchList = new MatchList();
        int[] startPos = { x, y };

        sameRawBlockPos.Add(startPos);
        sameColumnBlockPos.Add(startPos);

        FIndLeftRawDFS(blocks, sameRawBlockPos, TempColumnBlockPos, startPos, 1);
        FIndRightRawDFS(blocks, sameRawBlockPos, TempColumnBlockPos, startPos, 1);
        FIndUpColumnDFS(blocks, sameColumnBlockPos, TempRawBlockPos, startPos, 1);
        FIndDownColumnDFS(blocks, sameColumnBlockPos, TempRawBlockPos, startPos, 1);
        matchList.sameColumnBlockPosition = sameColumnBlockPos;
        matchList.tempColumnBlockPosition = TempColumnBlockPos;
        matchList.sameRawBlockPosition = sameRawBlockPos;
        matchList.tempRawBlockPosition = TempRawBlockPos;

        if (sameColumnBlockPos.Count > 2 && sameRawBlockPos.Count > 2)
            sameRawBlockPos.Remove(startPos);
        FindMatchedBlock(blocks, matchList);

        return matchList;
    }
    //list<int[]> ������ ����Ʈ �ȿ� x,y���� �ִ��� Ȯ���ϴ� �Լ� 
    public bool CheckContained(List<int[]> checkList, int x, int y)
    {
        for (int i = 0; i < checkList.Count; i++)
        {
            if (checkList[i][0] == x && checkList[i][1] == y)
                return true;
        }
        return false;
    }
    //��ġŲ���� �ε����� �� ����� �Լ� 
    public List<int[]> HitFromMunchkin(Block block, Direction dir)
    {
        List<int[]> hitBlock = new List<int[]>();
        switch (dir)
        {
            case Direction.Left:
                for (int i = block.GetBlockPosX(); i > 0; i--)
                {
                    //���� ��ġ ���� ������ ���迭�� ��ȣ�� ����
                    hitBlock.Add(new int[] { i, block.GetBlockPosY() });
                }
                block.SetBlockPos(0, block.GetBlockPosY());
                break;
            case Direction.Right:
                for (int i = block.GetBlockPosX(); i < 8; i++)
                {
                    //���� ��ġ ���� ������ ���迭�� ��ȣ�� ����
                    hitBlock.Add(new int[] { i, block.GetBlockPosY() });
                }
                block.SetBlockPos(8, block.GetBlockPosY());
                break;
            case Direction.Up:
                for (int i = block.GetBlockPosY(); i > 0; i--)
                {
                    //���� ��ġ ���� ������ ���迭�� ��ȣ�� ����
                    hitBlock.Add(new int[] { block.GetBlockPosX(), i });
                }
                block.SetBlockPos(block.GetBlockPosX(), 0);
                break;
            case Direction.Down:
                for (int i = block.GetBlockPosY(); i < 8; i++)
                {
                    //���� ��ġ ���� ������ ���迭�� ��ȣ�� ����
                    hitBlock.Add(new int[] { block.GetBlockPosX(), i });
                }
                block.SetBlockPos(block.GetBlockPosX(), 8);
                break;
        }
        return hitBlock;
    }
}
