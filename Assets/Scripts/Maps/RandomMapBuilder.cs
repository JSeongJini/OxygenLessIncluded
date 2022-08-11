using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMapBuilder : MonoBehaviour
{
    private int sizeW = 0;
    private int sizeH = 0;
    private int[, ] map = null;

    public void CreateRandomMap(int _sizeW, int _sizeH, ref int[, ] _mapInfo)
    {
        sizeW = _sizeW;
        sizeH = _sizeH;
        map = _mapInfo;

        //맵 분할 후 셀룰러 오토마타로 자원 군집 생성
        Divide(0, 0, _sizeW, _sizeH, 1);

        //맵 가운데 시작 기지 생성
        BuildCenterBase(_sizeW, _sizeH, ref _mapInfo);
    }

    private void Divide(int _leftBottomX, int _leftBottomY,
        int _rightTopX, int _rightTopY, int _level)
    {
        if(_level == 9)
        {
            //분할이 완료되면, 셀룰러 오토마타 방식으로 자원 군집 생성
            CellularAutomata(_leftBottomX, _leftBottomY, _rightTopX, _rightTopY);
        }
        else
        {
            if (_level % 2 == 0) {//세로로 분할
                int quarterX = (_rightTopX - _leftBottomX) / 4;
                int randX = Random.Range(_leftBottomX + quarterX, _rightTopX - quarterX);

                Divide(_leftBottomX, _leftBottomY, randX, _rightTopY, _level + 1);
                Divide(randX + 1, _leftBottomY, _rightTopX, _rightTopY, _level + 1);
            } else {                        //가로로 분할
                int quarterY = (_rightTopY - _leftBottomY) / 4;
                int randY = Random.Range(_leftBottomY + quarterY, _rightTopY - quarterY);

                Divide(_leftBottomX, _leftBottomY, _rightTopX, randY, _level + 1);
                Divide(_leftBottomX, randY + 1, _rightTopX, _rightTopY, _level + 1);
            }            
        }
    }

    private void CellularAutomata(int _leftBottomX, int _leftBottomY, int _rightTopX, int _rightTopY)
    {
        for(int y = _leftBottomY;  y < _rightTopY; y++)
            for(int x = _leftBottomX; x < _rightTopX; x++)
                //45% 확률로 벽 생성.   0 : 벽, 1 : 공간
                map[x, y] = (Random.Range(1, 21) <= 9) ? 0 : 1;

        //주변 8방향의 타일 중 5개 이상이 벽이라면 자신을 벽으로 변경
        Reproducing(_leftBottomX, _leftBottomY, _rightTopX, _rightTopY);

        //마무리 다듬기 -> 공간 사이에 끼어있는 벽을 공간으로 변경
        for (int y = _leftBottomY; y < _rightTopY; y++)
            for (int x = _leftBottomX; x < _rightTopX; x++)
            {
                int wallCount = CalcWallCount(x, y);
                if (wallCount < 5)
                {
                    map[x, y] = 1;
                }
            }
    }

    private void Reproducing(int _leftBottomX, int _leftBottomY, int _rightTopX, int _rightTopY)
    {
        int wallCount = 0;
        for (int y = _leftBottomY; y < _rightTopY; y++)
            for (int x = _leftBottomX; x < _rightTopX; x++)
            {
                //해당 타일이 벽이라면 다음 타일로 넘어감
                if (map[x, y] == 0)
                    continue;

                wallCount = CalcWallCount(x, y);
                
                //주변에 벽이 5개 이상 있으면, 해당 타일을 벽으로 변경
                if (wallCount >= 5)
                    map[x, y] = 0;
            }
    }

    private int CalcWallCount(int _x, int _y)
    {
        int wallCount = 0;

        //LeftTop
        if (_x == 0 || _y == (sizeH - 1) || map[_x - 1, _y + 1] == 0)
            wallCount++;
        //Top
        if (_y == (sizeH - 1) || map[_x, _y + 1] == 0)
            wallCount++;
        //RightTop
        if (_x == (sizeW - 1) || _y == (sizeH - 1) || map[_x+1, _y+1] == 0)
            wallCount++;
        //Left
        if (_x == 0 || map[_x-1, _y] == 0)
            wallCount++;
        //Right
        if (_x == (sizeW - 1) || map[_x+1, _y] == 0)
            wallCount++;
        //LeftBottom
        if (_x == 0 || _y == 0 || map[_x-1, _y-1] == 0)
            wallCount++;
        //Bottom
        if (_y == 0 || map[_x, _y-1] == 0)
            wallCount++;
        //RightBottom
        if (_x == (sizeW - 1) || _y == 0 || map[_x+1, _y-1] == 0)
            wallCount++;

        return wallCount;
    }

    private void BuildCenterBase(int _sizeW, int _sizeH, ref int[, ] _mapInfo)
    {
        int centerX = _sizeW >> 1;       // * 0.5
        int centerY = _sizeH >> 1;

        //벽으로 직사각형 만들기
        for (int i = -3; i <= 3; i++)
            for (int j = -5; j <= 5; j++)
                _mapInfo[centerX + j, centerY + i] = 0;

        //가로로 9칸 공간
        for (int i = -4; i <= 4; i++)
            _mapInfo[centerX + i, centerY] = 1;    

        //한칸 위 가로로 7칸 공간
        for (int i = -3; i <= 3; i++)
            _mapInfo[centerX + i, centerY + 1] = 1;    

        //두칸 위 가로로 7칸 공간
        for (int i = -3; i <= 3; i++)
            _mapInfo[centerX + i, centerY + 2] = 1;    
    }
}
