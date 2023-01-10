using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMapManager : MonoBehaviour
{
    private int sizeW = 0;
    private int sizeH = 0;
    private int[, ] moveMap = null;
    /*
        moveMap == 0  >> �̵� �Ұ�
        moveMap == 1  >> �̵� ����
        moveMap == 2  >> ���� ����
        moveMap == 3  >> ���� �̵� ����
     */
    private PathFinder pathFinder = null;


    private void Awake()
    {
        pathFinder = GetComponent<PathFinder>();
    }

    public void InitializeMoveMap(int _sizeW, int _sizeH, int[, ] _mapInfo)
    {
        //�̵� �� �ʱ�ȭ
        sizeW = _sizeW;
        sizeH = _sizeH;
        moveMap = new int[sizeW, sizeH];
        pathFinder.InitializeMapSize(_sizeW, _sizeH);

        //���̸� �̵� �Ұ���  -> 0, �����̶�� �̵� ���� -> 1, 
        for (int y = sizeH - 1; y >= 0; y--)
            for (int x = sizeW - 1; x >= 0; x--)
            {
                if(_mapInfo[x, y] == 1)     //�����̶��
                {
                    moveMap[x, y] = 1;          //�̵� ����
                }
                else                                  //���̶��
                {
                    moveMap[x, y] = 0;          //�̵� �Ұ���
                    if(y != sizeH - 1 && _mapInfo[x, y + 1] == 1)              //�� �ֻ���� �ƴϸ�
                        moveMap[x, y + 1] = 2;          //��ĭ ���� ���� ����
                }
            }
    }

    public bool CanMove(Vector2Int _pos)
    {
        return (moveMap[_pos.x, _pos.y] > 1);
    }

    public bool CanStand(Vector2Int _pos)
    {
        return (moveMap[_pos.x, _pos.y] >= 2);
    }

    public void PathFind(Vector2Int _startPos, Vector2Int _goalPos, List<Node> _path)
    {
        pathFinder.PathFinding(_startPos, _goalPos, moveMap, _path);
    }

    public void SetMovable(Vector2Int _pos, int _value)
    {
        if(_value >= 0 && _value <= 3)
            moveMap[_pos.x, _pos.y] = _value;
    }

    public void OnDigged(Vector2Int _pos)
    {
        //�Ʒ����� ���Ǵ� ��ٸ��� �ִٸ�
        if(_pos.y != 0 && (moveMap[_pos.x, _pos.y - 1] == 0 || moveMap[_pos.x, _pos.y - 1] == 3))
            moveMap[_pos.x, _pos.y] = 2;        //���� ����
        else        //�Ʒ��� ���� ���ٸ�
            moveMap[_pos.x, _pos.y] = 1;        //���� �Ұ���

        if (_pos.y != sizeH-1 && (moveMap[_pos.x, _pos.y + 1] == 2))
            moveMap[_pos.x, _pos.y + 1] = 1;    //�� ĭ ���� �� �̻� �� �� ����
    }

    public void OnBuildTile (Vector2Int _pos)
    {
        //Ÿ���� ������ ���� ��
        moveMap[_pos.x, _pos.y] = 0;
        
        //��ĭ ���� ���� �ƴ϶��
        if(moveMap[_pos.x, _pos.y + 1] != 0)
        {
            moveMap[_pos.x, _pos.y + 1] = 2;  //�� ĭ ���� ���� ����
        }
    }

    public void OnBuildLadder(Vector2Int _pos)
    {
        //��ٸ��� ������ ���� ���� �̵� ����
        moveMap[_pos.x, _pos.y] = 3;

        //��ĭ ���� ���� �ƴ϶��
        if (moveMap[_pos.x, _pos.y + 1] == 1)
        {
            moveMap[_pos.x, _pos.y + 1] = 2;  //�� ĭ ���� ���� ����
        }
    }

    public void OnDestroyBuilding(Vector2Int _pos)
    {
        //�� ĭ �Ʒ��� ���̶��
        if (moveMap[_pos.x, _pos.y - 1] == 0)
        {
            moveMap[_pos.x, _pos.y] = 2;  //���� ����
        }
        else
        {
            moveMap[_pos.x, _pos.y] = 1;  //�̵� ����
        }
        //�� ĭ ���� ���� �����̾��ٸ�
        if (moveMap[_pos.x, _pos.y + 1] == 2)
        {
            moveMap[_pos.x, _pos.y + 1] = 1;  //�̵� ����
        }
    }

    public int Test(Vector2Int _pos)
    {
        return moveMap[_pos.x, _pos.y];
    }

    public bool IsLadder(Vector2Int _pos)
    {
        return moveMap[_pos.x, _pos.y] == 3;
    }
}
