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
}

public class Func_Match : MonoBehaviour
{
    //BFS알고리즘을 통해서 주변 블록 전체 탐색 => 대각선의 경우도 다 깨짐.. 변경
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

            for (int i = 0; i < 4; i++) // 4방위 탐색
            {

                int[] temp = { nowPos[0] + dirX[i], nowPos[1] + dirY[i] };
                //방문한 곳이라면 다음 진행

                if (VisitContains(visitedPos, temp)) continue;

                if (blocks[nowPos[1], nowPos[0]] == null || blocks[temp[1], temp[0]] == null) continue;

                if (temp[0] < 0 || temp[0] >= blocks.GetLength(1) || temp[1] < 0 || temp[1] >= blocks.GetLength(0)) continue;

                //자신과 주변 블록의 색이 같은지 확인
                if (blocks[nowPos[1], nowPos[0]].GetBlockColor() == blocks[temp[1], temp[0]].GetBlockColor())
                {
                    targetPos.Enqueue(temp);
                    sameblockPos.Add(temp);
                }
                else //다르면 방문 체크
                {
                    visitedPos.Add(temp);
                }
            }
        }
        //일단꺼서 확인해보기
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

        //여기까진 BFS와 같다. 하지만 시작점으로부터 가로 세로 깊이 우선 탐색을 진행한다.
        FIndLeftRawDFS(blocks, sameRawBlockPos, TempColumnBlockPos, startPos, 1);
        FIndRightRawDFS(blocks, sameRawBlockPos, TempColumnBlockPos, startPos, 1);
        FIndUpColumnDFS(blocks, sameColumnBlockPos, TempRawBlockPos, startPos, 1);
        FIndDownColumnDFS(blocks, sameColumnBlockPos, TempRawBlockPos, startPos, 1);

        matchList.sameColumnBlockPosition = sameColumnBlockPos;
        matchList.tempColumnBlockPosition = TempColumnBlockPos;
        matchList.sameRawBlockPosition = sameRawBlockPos;
        matchList.tempRawBlockPosition = TempRawBlockPos;

        //가로세로 동시에 터져야 하는 경우 하나에서 빼줘야함\
        if(sameColumnBlockPos.Count>2 && sameRawBlockPos.Count>2)
            sameRawBlockPos.Remove(startPos);

        FindMatchedBlock(blocks, matchList);


        return matchList;
    }
    //가로 탐색 왼쪽
    private void FIndLeftRawDFS(Block[,] block, List<int[]> sameLine, List<int[]> tempLine, int[] startPos, int idx)
    {
        //왼쪽진행
        if (block[startPos[1], startPos[0] - idx] != null)
        {
            if (block[startPos[1], startPos[0] - idx].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
            {
                sameLine.Add(new int[] { startPos[0] - idx, startPos[1] });
                //추가로 위, 아래 탐색 한 번씩만 진행, 먼치킨 블럭을 위함
                if (block[startPos[1] + 1, startPos[0] - idx] != null)//아래
                {
                    if (block[startPos[1] + 1, startPos[0] - idx].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                    {
                        tempLine.Add(new int[] { startPos[0] - idx, startPos[1] + 1 });
                    }
                }
                if (block[startPos[1] - 1, startPos[0] - idx] != null)//위
                {
                    if (block[startPos[1] - 1, startPos[0] - idx].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                    {
                        tempLine.Add(new int[] { startPos[0] - idx, startPos[1] - 1 });
                    }
                }
                FIndLeftRawDFS(block, sameLine, tempLine, startPos, idx + 1);
            }
        }
        else return;
    }
    //가로 탐색 오른쪽
    private void FIndRightRawDFS(Block[,] block, List<int[]> sameLine, List<int[]> tempLine, int[] startPos, int idx)
    {
        //오른쪽진행
        if (block[startPos[1], startPos[0] + idx] != null)
        {
            if (block[startPos[1], startPos[0] + idx].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
            {
                sameLine.Add(new int[] { startPos[0] + idx, startPos[1] });
                //추가로 위, 아래 탐색 한 번씩만 진행, 먼치킨 블럭을 위함
                if (block[startPos[1] + 1, startPos[0] + idx] != null)//아래
                {
                    if (block[startPos[1] + 1, startPos[0] + idx].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                    {
                        tempLine.Add(new int[] { startPos[0] + idx, startPos[1] + 1 });
                    }
                }
                if (block[startPos[1] - 1, startPos[0] + idx] != null)//위
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
    //세로 탐색 위쪽
    private void FIndUpColumnDFS(Block[,] block, List<int[]> sameLine, List<int[]> tempLine, int[] startPos, int idx)
    {
        //위진행
        if (block[startPos[1] - idx, startPos[0]] != null)
        {
            if (block[startPos[1] - idx, startPos[0]].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
            {
                sameLine.Add(new int[] { startPos[0], startPos[1] - idx });
                //추가로 좌, 우 탐색 한 번씩만 진행, 먼치킨 블럭을 위함
                if (block[startPos[1] - idx, startPos[0] + 1] != null)//오른쪽
                {
                    if (block[startPos[1] - idx, startPos[0] + 1].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                    {
                        tempLine.Add(new int[] { startPos[0] + 1, startPos[1] - idx });
                    }
                }
                if (block[startPos[1] - idx, startPos[0] - 1] != null)//왼쪽
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
    //세로 탐색 아래쪽
    private void FIndDownColumnDFS(Block[,] block, List<int[]> sameLine, List<int[]> tempLine, int[] startPos, int idx)
    {
        //아래진행
        if (block[startPos[1] + idx, startPos[0]] != null)
        {
            if (block[startPos[1] + idx, startPos[0]].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
            {
                sameLine.Add(new int[] { startPos[0], startPos[1] + idx });
                //추가로 좌, 우 탐색 한 번씩만 진행, 먼치킨 블럭을 위함
                if (block[startPos[1] + idx, startPos[0] + 1] != null)//오른쪽
                {
                    if (block[startPos[1] + idx, startPos[0] + 1].GetBlockColor() == block[startPos[1], startPos[0]].GetBlockColor())
                    {
                        tempLine.Add(new int[] { startPos[0] + 1, startPos[1] + idx });
                    }
                }
                if (block[startPos[1] + idx, startPos[0] - 1] != null)//왼쪽
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
    //방문 목록에 있는지 확인하는 함수  BFS로직에서 사용
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
    //매치된 블럭들 담아주기, 터트릴 블럭들
    public void FindMatchedBlock(Block[,] blocks, MatchList matchList)
    {
        if (matchList.sameRawBlockPosition.Count > 2)
        {
            //시작 위치가 2번 적용되므로 리스트 하나는 1부터 시작.
            for (int i = 0; i < matchList.sameRawBlockPosition.Count; i++)
            {
                matchList.matchedBlockPostion.Add(new int[] { matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1] });
            }
        }
        if (matchList.sameColumnBlockPosition.Count > 2)
        {
            for (int i = 0; i < matchList.sameColumnBlockPosition.Count; i++)
            {
                matchList.matchedBlockPostion.Add(new int[] { matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1] });
            }
        }
        //특수 조건 블럭들 담기 
    }
    //자동 매치 기능
    public void AutoMatchblock(Block[,] blocks)
    {
        List<int[]> sameRawBlockPos = new List<int[]>();
        List<int[]> TempRawBlockPos = new List<int[]>();
        List<int[]> sameColumnBlockPos = new List<int[]>();
        List<int[]> TempColumnBlockPos = new List<int[]>();
        MatchList matchList = new MatchList();
        // 0 은 x 1은 y
        int[] startPos = new int[] {};
        for (int i = 1; i < 8; i++)
        {
            for(int j = 1; j < 8; j++)
            {
                startPos[0] = j;
                startPos[1] = i;
                FIndLeftRawDFS(blocks, sameRawBlockPos, TempColumnBlockPos, startPos, 1);
                FIndRightRawDFS(blocks, sameRawBlockPos, TempColumnBlockPos, startPos, 1);
                FIndUpColumnDFS(blocks, sameColumnBlockPos, TempRawBlockPos, startPos, 1);
                FIndDownColumnDFS(blocks, sameColumnBlockPos, TempRawBlockPos, startPos, 1);
            }
        }
        matchList.sameColumnBlockPosition = sameColumnBlockPos;
        matchList.tempColumnBlockPosition = TempColumnBlockPos;
        matchList.sameRawBlockPosition = sameRawBlockPos;
        matchList.tempRawBlockPosition = TempRawBlockPos;

        FindMatchedBlock(blocks, matchList);
    }
}
