using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Func_Swap : MonoBehaviour
{
    //ù ���� ���� �ݶ��̴��� �� ��° ���� �ݶ��̴��� ��ġ�� �ٲ۴�. 
    //���� ��ġ�� �������� X ��翡�� ��� ������ �̵��ϴ��� üũ�ϴ� ���� �ۼ� �ʿ�

    [SerializeField] private bool canTouch = true; //�ܺο��� Ž�� �˰���� ���� �� ����ı��� �ٳ����� ȣ��Ǿ� Ʈ�簡 �ȴ�.
    [SerializeField] private GameObject myBlock = null;
    [SerializeField] private GameObject swapblock = null;
    [SerializeField] private Map_Information mapInfo = null;

    private Vector2 initialPosition;
    private bool isDrag;
    private BlockPos initPos;
    private Block[,] blocks;
    private void Awake()
    {
        mapInfo = FindObjectOfType<Map_Information>();
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

            if (Vector3.Distance (initialPosition ,mousePosition)>0.2f)
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
        switch (dir)
        {
            case Direction.Up:
                if (blocks[pos.y - 1, pos.x] != null) //������ ���� �ƴ϶�� ������ ����
                {
                    //�ʿ��� ������Ʈ �̵�
                    StartCoroutine(SwapBlockPos(blocks[pos.y, pos.x].gameObject, blocks[pos.y - 1, pos.x].gameObject));
                    //�ڽ��� ����� �����ǰ� �־��ֱ� 
                    blocks[pos.y, pos.x].SetBlockPos(pos.x, pos.y - 1);
                    blocks[pos.y - 1, pos.x].SetBlockPos(pos.x, pos.y);
                    //�ʿ����� ���� ��ȯ
                    temp = blocks[pos.y, pos.x];
                    blocks[pos.y, pos.x] = blocks[pos.y - 1, pos.x];
                    blocks[pos.y - 1, pos.x] = temp;
                }
                break;
            case Direction.Down:
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
                }
                break;
            case Direction.Left:
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
                }
                break;
            case Direction.Right:
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
                }
                break;
            default:
                break;
        }
    }

    IEnumerator SwapBlockPos(GameObject block1, GameObject block2)
    {
        //������ġ�� ������ġ ����
        Vector3 startPos1 = block1.transform.position, startPos2 = block2.transform.position;
        Vector3 endPos1 = startPos2, endPos2 = startPos1;
        //���� �ð��ȵ� �����ϰ� �ϱ����� ���� ����
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
        canTouch = true;
    }
}
