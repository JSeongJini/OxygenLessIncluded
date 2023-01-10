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

    Node[, ] NodeArray;
    Node StartNode, GoalNode, CurNode;
    List<Node> openList = new List<Node>();
    List<Node> closeList = new List<Node>();

    int upCount = 0;
    int downCount = 0;

    public List<Node> PathFinding(Vector2Int _startPos, Vector2Int _goalPos, int _sizeW, int _sizeH, int[, ] _moveMap)
    {
        List<Node> FinalNodeList = new List<Node>();

        sizeW = _sizeW;
        sizeH = _sizeH;
        mapSize = sizeW * _sizeH;
        
        NodeArray = new Node[sizeW, sizeH];

        for (int y = 0; y < sizeH; y++)
            for(int x = 0; x < sizeW; x++)
                    NodeArray[x, y] = new Node(_moveMap[x, y], x, y);
        
        StartNode = NodeArray[_startPos.x, _startPos.y];
        GoalNode = NodeArray[_goalPos.x, _goalPos.y];

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
                    FinalNodeList.Add(goalCurNode);
                    goalCurNode = goalCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();
            }

            if (((NodeArray[CurNode.x + 1, CurNode.y].move == 0
                || NodeArray[CurNode.x - 1, CurNode.y].move == 0)
                && upCount < 2) || NodeArray[CurNode.x, CurNode.y].move == 3) {
                OpenListAdd(CurNode.x, CurNode.y + 1); // 상이동
            }

            if (downCount < 2 || NodeArray[CurNode.x, CurNode.y].move == 3)
            {
                OpenListAdd(CurNode.x, CurNode.y - 1);   // 하이동
            }

            if (upCount > 0 || downCount > 0)
            {
                if(NodeArray[CurNode.x +1, CurNode.y].move >= 2)
                    OpenListAdd(CurNode.x + 1, CurNode.y);   // 우이동    
                if (NodeArray[CurNode.x - 1, CurNode.y].move >= 2)
                    OpenListAdd(CurNode.x - 1, CurNode.y); // 좌이동
            }
            else
            {
                if (NodeArray[CurNode.x, CurNode.y].move >= 2)
                {
                    OpenListAdd(CurNode.x + 1, CurNode.y);   // 우이동         
                    OpenListAdd(CurNode.x - 1, CurNode.y);   // 좌이동
                }
            }
        }

        return FinalNodeList;
    }

    private void OpenListAdd(int _x, int _y)
    {
        //상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면 
        if (_x >= 0 && _x < sizeW
            && _y >= 0 && _y < sizeH
            && NodeArray[_x, _y].move > 0
            && !closeList.Contains(NodeArray[_x, _y]))
        {
            // 이웃노드에 넣고, 직선은 10
            Node NeighborNode = NodeArray[_x, _y];
            int MoveCost = CurNode.G + 10;


            //이동비용이 이웃노드G보다 작거나, 열린리스트에 이웃노드가 없다면,
            //G,H,ParentNode를 설정 후 열린리스트에 추가
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