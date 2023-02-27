using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Func_Spawn : MonoBehaviour
{
    [SerializeField] private Block blockPrefab =null;
    [SerializeField] private Func_Swap func_swap = null;
    [SerializeField] private List<Block> spawnListPool;
    [SerializeField] private List<Block> inactiveList;

    private List<int[]> matchList;
    private List<int[]> newMatchList = new List<int[]>();
    private Block[,] blocks;
    private void Awake()
    {
        func_swap = FindObjectOfType<Func_Swap>();
    }
    //맵 블럭 정보 받기
    public void SetMapBlocksInfo(Block[,] blocks)
    {
        this.blocks = blocks;
    }
    //초기 블럭들 오브젝트 풀에 넣기 
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
    //매치된 블럭 리스트 받기
    public void SetMatchedList(List<int[]> matchedList)
    {
        matchList = matchedList;
        DeleteBlocksScoreUP(matchList);
    }
    //매치된 블럭들 지우고 점수 올리기
    private void DeleteBlocksScoreUP(List<int[]> matchedList)
    {
        //비활성화 오브젝트 풀 초기화
        inactiveList.Clear();
        for (int i = 0; i < matchedList.Count; i++)
        {
            //중복된 좌표값이 아닐 경우
            if (blocks[matchedList[i][1], matchedList[i][0]].GetBlockMode() != BlockMode.Empty)
            {
                //블럭 지우고
                blocks[matchedList[i][1], matchedList[i][0]].SetBlockMode(BlockMode.Empty);
                blocks[matchedList[i][1], matchedList[i][0]].gameObject.SetActive(false);

                //비활성화 오브젝트 풀에 담아두기
                inactiveList.Add(blocks[matchedList[i][1], matchedList[i][0]]);

                //점수 올리기
                GameManager.Instance.SetScore(20);

                //새 매치 리스트 갱신 , 
                newMatchList.Add(new int[] { matchedList[i][0], matchedList[i][1] });
            }
            else
            {
                Debug.Log("중복됌");
            }
        }
        //열별 최대 최소 y값 찾기위한 리스트
        List<int[]> minMax = new List<int[]>();
        //열별로 이동 진행해야함
        for (int i = 1; i < 8; i++)
        {
            int max = -1, min = 20;
            for (int j = 1; j < 8; j++)
            {
                //꺼진블럭 확인
                if (blocks[j, i].gameObject.activeSelf == false)
                {
                    //j 행 i열
                    if (j > max) max = j;
                    if (j < min) min = j;
                }
            }
            minMax.Add(new int[] { min, max });
        }

        //오브젝트풀 돌리기 위한 넘버링
        int inactiveCount = 0;
        //꺼진 블록의 개수만큼 위에서 아래로 값 채우기 
        for (int i = 0; i < minMax.Count; i++) //minmax.count 는 열 개수
        {
            if (minMax[i][0] == 20 || minMax[i][1] == -1) continue; //꺼진 블럭이 없을 경우 

            int dif = minMax[i][1] - minMax[i][0] + 1; //최대 - 최소 +1  => 최대 최소 사이 개수

            //빈블록 윗부분을 내리는 로직
            for (int j = minMax[i][0]-1; j >= 1; j--)
            {
                if (blocks[j, i + 1].gameObject.activeSelf == false) continue;
                blocks[j, i + 1].SetBlockPos(i + 1, j + dif); //min => max자리로 이동 dif만큼 이동했기 때문
                blocks[j, i + 1].MoveBlock();
                blocks[j + dif, i + 1] = blocks[j, i + 1];
            }
            if (dif != 1)
            {
                //밑으로 내리고 난 후 비어있는 윗부분 채우는 로직
                for (int j = 1; j <= dif; j++)
                {
                    inactiveList[inactiveCount].gameObject.SetActive(true);
                    blocks[j, i + 1] = null;
                    inactiveList[inactiveCount].gameObject.transform.position = new Vector2(i - 3, 4 + dif - j);
                    inactiveList[inactiveCount].SetBlockRandomColor();
                    inactiveList[inactiveCount].SetBlockMode(BlockMode.Normal);
                    inactiveList[inactiveCount].SetBlockPos(i + 1, j);
                    inactiveList[inactiveCount].MoveBlock();
            
                    blocks[j, i + 1] = inactiveList[inactiveCount];
                    inactiveCount++;
                }
            }
            else
            {
                inactiveList[inactiveCount].gameObject.SetActive(true);
                inactiveList[inactiveCount].gameObject.transform.position = new Vector2(i - 3, 4);
                inactiveList[inactiveCount].SetBlockRandomColor();
                inactiveList[inactiveCount].SetBlockPos(i + 1, 1);
                inactiveList[inactiveCount].SetBlockMode(BlockMode.Normal);
                inactiveList[inactiveCount].MoveBlock();

                inactiveList[inactiveCount].gameObject.name = (i + 1).ToString();
                blocks[1, i + 1] = inactiveList[inactiveCount];
                inactiveCount++;
            }
        }
        //StartCoroutine(Co_AutoSwap());
    }
    //주변에 매치 되는게 있는지 확인하는 함수 
    IEnumerator Co_AutoSwap()
    {
        for(int i = 1; i < 8; i++)
        {
            for(int  j = 1; j < 8; j++)
            {
                yield return new WaitUntil(() => blocks[i, j].ismoving == false); 
            }
        }
        for (int i = 1; i < 8; i++)
        {
            for (int j = 1; j < 8; j++)
            {
                func_swap.AutoSwapBlock(blocks, j,i);
            }
        }
    }
    //리스트 안에 해당 블럭이 있는지 찾는 함수
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
