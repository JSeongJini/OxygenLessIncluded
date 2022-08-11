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

        //�� ���� �� ���귯 ���丶Ÿ�� �ڿ� ���� ����
        Divide(0, 0, _sizeW, _sizeH, 1);

        //�� ��� ���� ���� ����
        BuildCenterBase(_sizeW, _sizeH, ref _mapInfo);
    }

    private void Divide(int _leftBottomX, int _leftBottomY,
        int _rightTopX, int _rightTopY, int _level)
    {
        if(_level == 9)
        {
            //������ �Ϸ�Ǹ�, ���귯 ���丶Ÿ ������� �ڿ� ���� ����
            CellularAutomata(_leftBottomX, _leftBottomY, _rightTopX, _rightTopY);
        }
        else
        {
            if (_level % 2 == 0) {//���η� ����
                int quarterX = (_rightTopX - _leftBottomX) / 4;
                int randX = Random.Range(_leftBottomX + quarterX, _rightTopX - quarterX);

                Divide(_leftBottomX, _leftBottomY, randX, _rightTopY, _level + 1);
                Divide(randX + 1, _leftBottomY, _rightTopX, _rightTopY, _level + 1);
            } else {                        //���η� ����
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
                //45% Ȯ���� �� ����.   0 : ��, 1 : ����
                map[x, y] = (Random.Range(1, 21) <= 9) ? 0 : 1;

        //�ֺ� 8������ Ÿ�� �� 5�� �̻��� ���̶�� �ڽ��� ������ ����
        Reproducing(_leftBottomX, _leftBottomY, _rightTopX, _rightTopY);

        //������ �ٵ�� -> ���� ���̿� �����ִ� ���� �������� ����
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
                //�ش� Ÿ���� ���̶�� ���� Ÿ�Ϸ� �Ѿ
                if (map[x, y] == 0)
                    continue;

                wallCount = CalcWallCount(x, y);
                
                //�ֺ��� ���� 5�� �̻� ������, �ش� Ÿ���� ������ ����
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

        //������ ���簢�� �����
        for (int i = -3; i <= 3; i++)
            for (int j = -5; j <= 5; j++)
                _mapInfo[centerX + j, centerY + i] = 0;

        //���η� 9ĭ ����
        for (int i = -4; i <= 4; i++)
            _mapInfo[centerX + i, centerY] = 1;    

        //��ĭ �� ���η� 7ĭ ����
        for (int i = -3; i <= 3; i++)
            _mapInfo[centerX + i, centerY + 1] = 1;    

        //��ĭ �� ���η� 7ĭ ����
        for (int i = -3; i <= 3; i++)
            _mapInfo[centerX + i, centerY + 2] = 1;    
    }
}
