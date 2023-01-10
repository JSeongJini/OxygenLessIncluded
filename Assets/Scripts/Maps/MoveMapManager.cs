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
        moveMap == 0  >> 이동 불가
        moveMap == 1  >> 이동 가능
        moveMap == 2  >> 서기 가능
        moveMap == 3  >> 상하 이동 가능
     */
    private PathFinder pathFinder = null;


    private void Awake()
    {
        pathFinder = GetComponent<PathFinder>();
    }

    public void InitializeMoveMap(int _sizeW, int _sizeH, int[, ] _mapInfo)
    {
        //이동 맵 초기화
        sizeW = _sizeW;
        sizeH = _sizeH;
        moveMap = new int[sizeW, sizeH];
        pathFinder.InitializeMapSize(_sizeW, _sizeH);

        //벽이면 이동 불가능  -> 0, 공간이라면 이동 가능 -> 1, 
        for (int y = sizeH - 1; y >= 0; y--)
            for (int x = sizeW - 1; x >= 0; x--)
            {
                if(_mapInfo[x, y] == 1)     //공간이라면
                {
                    moveMap[x, y] = 1;          //이동 가능
                }
                else                                  //벽이라면
                {
                    moveMap[x, y] = 0;          //이동 불가능
                    if(y != sizeH - 1 && _mapInfo[x, y + 1] == 1)              //맵 최상단이 아니면
                        moveMap[x, y + 1] = 2;          //한칸 위는 서기 가능
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
        //아래에도 벽또는 사다리가 있다면
        if(_pos.y != 0 && (moveMap[_pos.x, _pos.y - 1] == 0 || moveMap[_pos.x, _pos.y - 1] == 3))
            moveMap[_pos.x, _pos.y] = 2;        //서기 가능
        else        //아래에 벽이 없다면
            moveMap[_pos.x, _pos.y] = 1;        //서기 불가능

        if (_pos.y != sizeH-1 && (moveMap[_pos.x, _pos.y + 1] == 2))
            moveMap[_pos.x, _pos.y + 1] = 1;    //한 칸 위는 더 이상 설 수 없음
    }

    public void OnBuildTile (Vector2Int _pos)
    {
        //타일이 생성된 곳은 벽
        moveMap[_pos.x, _pos.y] = 0;
        
        //한칸 위가 벽이 아니라면
        if(moveMap[_pos.x, _pos.y + 1] != 0)
        {
            moveMap[_pos.x, _pos.y + 1] = 2;  //한 칸 위는 서기 가능
        }
    }

    public void OnBuildLadder(Vector2Int _pos)
    {
        //사다리가 생성된 곳은 상하 이동 가능
        moveMap[_pos.x, _pos.y] = 3;

        //한칸 위가 벽이 아니라면
        if (moveMap[_pos.x, _pos.y + 1] == 1)
        {
            moveMap[_pos.x, _pos.y + 1] = 2;  //한 칸 위는 서기 가능
        }
    }

    public void OnDestroyBuilding(Vector2Int _pos)
    {
        //한 칸 아래가 벽이라면
        if (moveMap[_pos.x, _pos.y - 1] == 0)
        {
            moveMap[_pos.x, _pos.y] = 2;  //서기 가능
        }
        else
        {
            moveMap[_pos.x, _pos.y] = 1;  //이동 가능
        }
        //한 칸 위가 서기 가능이었다면
        if (moveMap[_pos.x, _pos.y + 1] == 2)
        {
            moveMap[_pos.x, _pos.y + 1] = 1;  //이동 가능
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
