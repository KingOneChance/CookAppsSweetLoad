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

        //먼치킨 블럭이 될 수 있는지 판별
        for (int i = 0; i < TempColumnBlockPos.Count; i++) //완전 탐색하기 
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
    //가로 탐색 왼쪽
    private void FIndLeftRawDFS(Block[,] block, List<int[]> sameLine, List<int[]> tempLine, int[] startPos, int idx)
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
        //가로만 터질 때
        if (matchList.sameRawBlockPosition.Count > 2 && matchList.sameColumnBlockPosition.Count < 2)
        {
            for (int i = 0; i < matchList.sameRawBlockPosition.Count; i++)
            {
                //매치된 블럭 목록안에 중복 포함이 되는지 체크
                if (CheckContained(matchList.matchedBlockPostion, matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1]) == false)
                    matchList.matchedBlockPostion.Add(new int[] { matchList.sameRawBlockPosition[i][0], matchList.sameRawBlockPosition[i][1] });
            }
        }
        //가로만 터질 때 세로만 터질때, 
        else if (matchList.sameColumnBlockPosition.Count > 2 && matchList.sameRawBlockPosition.Count < 2)
        {
            for (int i = 0; i < matchList.sameColumnBlockPosition.Count; i++)
            {
                //매치된 블럭 목록안에 중복 포함이 되는지 체크
                if (CheckContained(matchList.matchedBlockPostion, matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1]) == false)
                    matchList.matchedBlockPostion.Add(new int[] { matchList.sameColumnBlockPosition[i][0], matchList.sameColumnBlockPosition[i][1] });
            }
        }
        //먼치킨 계산 로직 
        else if (matchList.sameColumnBlockPosition.Count >= 2 && matchList.sameRawBlockPosition.Count >= 2)
        {
            //가로세로 함께 터질때,
            if ((matchList.sameColumnBlockPosition.Count > 2 && matchList.sameRawBlockPosition.Count > 2))
            {
                if (matchList.isMunchkin == true) matchList.matchedBlockPostion.Add(matchList.tempColumnBlockPosition[0]);
                //시작 위치가 2번 적용되므로 리스트 하나는 1부터 시작. (하는  로직을 넣어한다. )
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
            //2X2만 터질때 
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
            //한쪽만 3 이상 일때 
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
            //한쪽만 3 이상 일때 
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

        //특수 조건 블럭들 담기 
    }
    //자동 매치 기능
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
    //list<int[]> 형식의 리스트 안에 x,y값이 있는지 확인하는 함수 
    public bool CheckContained(List<int[]> checkList, int x, int y)
    {
        for (int i = 0; i < checkList.Count; i++)
        {
            if (checkList[i][0] == x && checkList[i][1] == y)
                return true;
        }
        return false;
    }
    //먼치킨블럭과 부딪히는 블럭 지우는 함수 
    public List<int[]> HitFromMunchkin(Block block, Direction dir)
    {
        List<int[]> hitBlock = new List<int[]>();
        switch (dir)
        {
            case Direction.Left:
                for (int i = block.GetBlockPosX(); i > 0; i--)
                {
                    //현재 위치 부터 끝까지 블럭배열의 번호를 담음
                    hitBlock.Add(new int[] { i, block.GetBlockPosY() });
                }
                block.SetBlockPos(0, block.GetBlockPosY());
                break;
            case Direction.Right:
                for (int i = block.GetBlockPosX(); i < 8; i++)
                {
                    //현재 위치 부터 끝까지 블럭배열의 번호를 담음
                    hitBlock.Add(new int[] { i, block.GetBlockPosY() });
                }
                block.SetBlockPos(8, block.GetBlockPosY());
                break;
            case Direction.Up:
                for (int i = block.GetBlockPosY(); i > 0; i--)
                {
                    //현재 위치 부터 끝까지 블럭배열의 번호를 담음
                    hitBlock.Add(new int[] { block.GetBlockPosX(), i });
                }
                block.SetBlockPos(block.GetBlockPosX(), 0);
                break;
            case Direction.Down:
                for (int i = block.GetBlockPosY(); i < 8; i++)
                {
                    //현재 위치 부터 끝까지 블럭배열의 번호를 담음
                    hitBlock.Add(new int[] { block.GetBlockPosX(), i });
                }
                block.SetBlockPos(block.GetBlockPosX(), 8);
                break;
        }
        return hitBlock;
    }
}
