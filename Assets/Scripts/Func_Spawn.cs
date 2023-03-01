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
    public void SetMatchedList(List<int[]> matchedList, List<int[]> munPos)
    {
        matchList = matchedList;
        DeleteBlocksScoreUP(matchList, munPos);
    }
    //매치된 블럭들 지우고 점수 올리는 함수
    private void DeleteBlocksScoreUP(List<int[]> matchedList, List<int[]> munPos)
    {
        //비활성화 오브젝트 풀 초기화
        inactiveList.Clear();
        for (int i = 0; i < matchedList.Count; i++)
        {
            //matched list에서 받아온 중복처리가 되는 경우, 다음 진행 
            if (blocks[matchedList[i][1], matchedList[i][0]].GetBlockMode() == BlockMode.Empty ||
                blocks[matchedList[i][1], matchedList[i][0]].GetBlockMode() == BlockMode.Munchkin)
                continue;
            //먼치킨 블럭이 있는 경우 
            //먼치킨 블럭과 매치된 리스트가 일치할 때 
            if (munPos.Count != 0)
            {
                for (int j = 0; j < munPos.Count; j++)
                {
                    if (munPos[j][0] == matchList[i][0] && munPos[j][1] == matchList[i][1])
                    {
                        //지우지 않고 스케일 키우는걸로 확인
                        blocks[matchedList[i][1], matchedList[i][0]].SetBlockMode(BlockMode.Munchkin);
                        blocks[matchedList[i][1], matchedList[i][0]].SetBlockColor(BlockColor.MunChKin);
                        blocks[matchedList[i][1], matchedList[i][0]].gameObject.transform.localScale *= 1.2f;
                        GameManager.Instance.SetScore(40);
                        break;
                    }
                    else
                    {
                        //블럭 지우고
                        blocks[matchedList[i][1], matchedList[i][0]].SetBlockMode(BlockMode.Empty);

                        //비활성화 오브젝트 풀에 담아두기
                        inactiveList.Add(blocks[matchedList[i][1], matchedList[i][0]]);
                        blocks[matchedList[i][1], matchedList[i][0]].gameObject.SetActive(false);
                        //점수 올리기
                        GameManager.Instance.SetScore(20);
                    }
                }
            }
            else
            {
                //블럭 지우고
                blocks[matchedList[i][1], matchedList[i][0]].SetBlockMode(BlockMode.Empty);

                //비활성화 오브젝트 풀에 담아두기
                inactiveList.Add(blocks[matchedList[i][1], matchedList[i][0]]);
                blocks[matchedList[i][1], matchedList[i][0]].gameObject.SetActive(false);
                //점수 올리기
                GameManager.Instance.SetScore(20);
            }
            //새 매치 리스트 갱신 , 
            newMatchList.Add(new int[] { matchedList[i][0], matchedList[i][1] });

        }
        //열별 최대 최소 y값 찾기위한 리스트
        List<int[]> minMax = new List<int[]>();
        //열별로 이동 진행해야함
        for (int i = 1; i < 8; i++)
        {
            int max = -1, min = 20; // 최대 최소를 기록하기 위한 임의의 초기값
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
        //minmax.count 는 열 개수
        for (int i = 0; i < minMax.Count; i++)
        {
            //꺼진 블럭이 없을 경우 다음 열 진행 
            if (minMax[i][0] == 20 || minMax[i][1] == -1) continue;

            //최대 - 최소 +1  => 최대 최소 사이 개수
            int dif = minMax[i][1] - minMax[i][0] + 1;

            //최하단 빈블록 윗부분을 내리는 로직 
            int difPlus = dif;
            int emptyNum = 0;
            for (int j = minMax[i][0] - 1; j >= 1; j--)
            {
                //중간이 비어있는 경우 
                if (blocks[j, i + 1].gameObject.activeSelf == false&& blocks[j, i + 1].GetBlockMode()!=BlockMode.Munchkin)
                {
                    difPlus++;
                    emptyNum++;
                    continue;
                }
                blocks[j, i + 1].SetBlockPos(i + 1, j + difPlus); //min => max자리로 이동 difPlus만큼 이동했기 때문
                blocks[j, i + 1].MoveBlock();
                blocks[j + difPlus, i + 1] = blocks[j, i + 1];
            }

            //위에서 비어있는 밑 부분으로 내려서 채우는 로직 
            /*    for (int j = minMax[i][1]; j >= 1; j--)
                {
                    //중간이 비어있는 경우 
                    if (blocks[j, i + 1].gameObject.activeSelf == false)
                    {
                        emptyNum++;
                        continue;
                    }
                    blocks[j, i + 1].SetBlockPos(i + 1, j + emptyNum); //min => max자리로 이동 difPlus만큼 이동했기 때문
                    blocks[j, i + 1].MoveBlock();
                    blocks[j + emptyNum, i + 1] = blocks[j, i + 1];
                }*/

            if (dif != 1) //한 열에 여러개의 블럭이 지워지는 경우 
            {
                //채워질 블럭의 개수를 캐싱
                int newDif = dif - emptyNum;

                //밑으로 내리고 난 후 비어있는 윗부분 채우는 로직
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
                //밑으로 내리고 난 후 비어있는 윗부분 채우는 로직
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
            else //한열에 하나만 블럭이 지워지는 경우 
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
    //주변에 매치 되는게 있는지 확인하는 함수 
    IEnumerator Co_AutoCheck(Block[,] block)
    {

        yield return null;
        List<int[]> matchedBlockList = new List<int[]>();
        List<int[]> munPos = new List<int[]>();
        //지워질 블럭이 있는지 확인하기 
        for (int i = 1; i < 8; i++)
        {
            for (int j = 1; j < 8; j++)
            {
                Debug.Log("자동 매치 진행중");

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
        //블럭들이 모두 이동한 후 움직이게 하는 로직 
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
