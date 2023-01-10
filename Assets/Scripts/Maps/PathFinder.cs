using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Node(int _move, int _x, int _y)
    {
        move = _move; x = _x; y = _y;
    }

    public int move;
    public Node ParentNode;

    public int x, y, G, H;
    public int F { get { return G + H; } }
}

public class PathFinder : MonoBehaviour
{
    int sizeW;
    int sizeH;
    int mapSize;

    Node[, ] nodeArray;
    Node StartNode, GoalNode, CurNode;
    List<Node> openList = new List<Node>();
    List<Node> closeList = new List<Node>();

    int upCount = 0;
    int downCount = 0;

    public void InitializeMapSize(int _sizeW, int _sizeH)
    {
        sizeW = _sizeW;
        sizeH = _sizeH;
        mapSize = sizeW * _sizeH;
        
        nodeArray = new Node[sizeW, sizeH];

        for (int y = 0; y < sizeH; y++)
            for (int x = 0; x < sizeW; x++)
                nodeArray[x, y] = new Node(0, x, y);
    }

    public void PathFinding(Vector2Int _startPos, Vector2Int _goalPos, int[, ] _moveMap, List<Node> _finalNodeList)
    {
        for (int y = 0; y < sizeH; y++)
            for (int x = 0; x < sizeW; x++)
                nodeArray[x, y].move = _moveMap[x, y];
        
        StartNode = nodeArray[_startPos.x, _startPos.y];
        GoalNode = nodeArray[_goalPos.x, _goalPos.y];

        openList.Add(StartNode);

        while (openList.Count > 0)
        {
            CurNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F <= CurNode.F && openList[i].H < CurNode.H)
                {
                    CurNode = openList[i];
                }
            }

            if(closeList.Count != 0)
            {
                if (CurNode.y == closeList[closeList.Count - 1].y)
                {
                    upCount = 0;
                    downCount = 0;
                }
                if (closeList[closeList.Count - 1].y < CurNode.y && closeList[closeList.Count - 1].move != 3)
                {
                    upCount++;
                }
                if (closeList[closeList.Count - 1].y > CurNode.y && closeList[closeList.Count - 1].move != 3)
                {
                    downCount++;
                }
            }

            openList.Remove(CurNode);
            closeList.Add(CurNode);

            if (CurNode == GoalNode)
            {
                Node goalCurNode = GoalNode;
                while (goalCurNode != StartNode)
                {
                    _finalNodeList.Add(goalCurNode);
                    goalCurNode = goalCurNode.ParentNode;
                }
                _finalNodeList.Add(StartNode);
                _finalNodeList.Reverse();
            }

            if (((nodeArray[CurNode.x + 1, CurNode.y].move == 0
                || nodeArray[CurNode.x - 1, CurNode.y].move == 0)
                && upCount < 2) || nodeArray[CurNode.x, CurNode.y].move == 3) {
                OpenListAdd(CurNode.x, CurNode.y + 1); // ���̵�
            }

            if (downCount < 2 || nodeArray[CurNode.x, CurNode.y].move == 3)
            {
                OpenListAdd(CurNode.x, CurNode.y - 1);   // ���̵�
            }

            if (upCount > 0 || downCount > 0)
            {
                if(nodeArray[CurNode.x +1, CurNode.y].move >= 2)
                    OpenListAdd(CurNode.x + 1, CurNode.y);   // ���̵�    
                if (nodeArray[CurNode.x - 1, CurNode.y].move >= 2)
                    OpenListAdd(CurNode.x - 1, CurNode.y); // ���̵�
            }
            else
            {
                if (nodeArray[CurNode.x, CurNode.y].move >= 2)
                {
                    OpenListAdd(CurNode.x + 1, CurNode.y);   // ���̵�         
                    OpenListAdd(CurNode.x - 1, CurNode.y);   // ���̵�
                }
            }
        }

        openList.Clear();
        closeList.Clear();

    }

    private void OpenListAdd(int _x, int _y)
    {
        //�����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ� 
        if (_x >= 0 && _x < sizeW
            && _y >= 0 && _y < sizeH
            && nodeArray[_x, _y].move > 0
            && !closeList.Contains(nodeArray[_x, _y]))
        {
            // �̿���忡 �ְ�, ������ 10
            Node NeighborNode = nodeArray[_x, _y];
            int MoveCost = CurNode.G + 10;


            //�̵������ �̿����G���� �۰ų�, ��������Ʈ�� �̿���尡 ���ٸ�,
            //G,H,ParentNode�� ���� �� ��������Ʈ�� �߰�
            if (MoveCost < NeighborNode.G || !openList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - GoalNode.x)
                    + Mathf.Abs(NeighborNode.y - GoalNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                openList.Add(NeighborNode);
            }
        }
    }
    
}