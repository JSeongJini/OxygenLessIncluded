using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class MapManager : MonoBehaviour
{
    [SerializeField] private ResourceMapManager resourceMapMng = null;
    [SerializeField] private MoveMapManager moveMapMng = null;
    [SerializeField] private BuildingMapManager buildingMapMng = null;

    private RandomMapBuilder randomMapBuilder = null;
    private int sizeW = 192;
    private int sizeH = 108;
    private int[, ] mapInfo = null;

    //�ʱ�ȭ ���ܺ���, ���� �� ����, �ڿ��� ����, �̵��� ����, ������ ����


    private void Awake()
    {
        //TODO : �����ϱ��ΰ�, �ҷ������ΰ��� ����
        // ���� �������� �������, �ҷ������� ����
        mapInfo = new int[sizeW, sizeH];
        randomMapBuilder = GetComponent<RandomMapBuilder>();
        randomMapBuilder.CreateRandomMap(sizeW, sizeH, ref mapInfo);

        //�� �ҷ�����
        //InitializeFromFile("Assets\\ResourceMap.txt");
    }

    private void Start()
    {
        resourceMapMng.InitializeResouceMap(sizeW, sizeH, mapInfo);
        moveMapMng.InitializeMoveMap(sizeW, sizeH, mapInfo);
        buildingMapMng.InitializeBuildingMap(sizeW, sizeH, mapInfo);
    }

    //private void Update()
    //{
    //    switch (initialzeStep)
    //    {
    //        case 0:
    //            randomMapBuilder.CreateRandomMap(sizeW, sizeH, ref mapInfo);
    //            initialzeStep++;
    //            break;
    //        case 1:
    //            resourceMapMng.InitializeResouceMap(sizeW, sizeH, mapInfo);
    //            initialzeStep++;
    //            break;
    //        case 2:
    //            moveMapMng.InitializeMoveMap(sizeW, sizeH, mapInfo);
    //            initialzeStep++;
    //            break;
    //        case 3:
    //            buildingMapMng.InitializeBuildingMap(sizeW, sizeH, mapInfo);
    //            initialzeStep++;
    //            break;
    //        default:
    //            break;
    //    }
    //}


    private bool InitializeFromFile(string _filePath)
    {
        FileInfo fileInfo = new FileInfo(_filePath);
        StreamReader reader;

        if (fileInfo.Exists)
        {
            reader = new StreamReader(_filePath);
        }
        else return false;

        sizeW = int.Parse(reader.ReadLine());
        sizeH = int.Parse(reader.ReadLine());

        
        mapInfo = new int[sizeW, sizeH];
        for (int y = 0; y < sizeH; y++)
            for(int x = 0; x < sizeW; x++)
                mapInfo[x, y] = int.Parse(reader.ReadLine());

        reader.Close();
        return true;
    }

    public void PathFind(Vector2Int _startPos, Vector2Int _goalPos, List<Node> _path)
    {
        moveMapMng.PathFind(_startPos, _goalPos, _path);
    }

    public ResourceBase GetResourceByPos(Vector2Int _pos)
    {
        return resourceMapMng.GetResourceByPos(_pos);
    }

    public void GetWorkPos(Vector2Int _startPos, Vector2Int _targetPos, List<Vector2Int> workPosList)
    {
        //������ġ�� ���Ͽ� �� ����� �� �켱 �˻�
        int right = 1;
        int top = 1;
        if (_startPos.x < _targetPos.x) right *= -1;
        if (_startPos.y < _targetPos.y) top *= -1;

        int[] dx = { right, right, right, 0, 0, -right, -right, -right };
        int[] dy = { top, 0, -top, top, -top, top, 0, -top};
        
        //8���� �� �۾��� ������(�� ���� �� �ִ�)��ġ�� ��� ��ȯ
        for (int i = 0; i < 8; i++)
        {
            Vector2Int workPos = new Vector2Int(_targetPos.x + dx[i], _targetPos.y + dy[i]);
            bool canStand = moveMapMng.CanStand(workPos);
            if (canStand) workPosList.Add(workPos);
        }
    }

    
    int[] dx = {0, 0, 1, 1, 1, 0,  -1, -1, -1 };
    int[] dy = {0, -1, 1, 0, -1, 1, 1, 0, -1 };

    public Vector2Int GetAvoidPos(Vector2Int _pos)
    {
        Vector2Int avoidPosList;

        int distance = 1;

        //4���� �� �� ���� �� �ִ� ��ġ�� ã�� �� ���� �ݺ�
        while (distance < 10)
        {
            for (int i = 0; i < 9; i++)
            {
                avoidPosList = new Vector2Int(_pos.x + (dx[i] * distance), _pos.y + (dy[i] * distance));
                bool canStand = moveMapMng.CanStand(avoidPosList);
                if (canStand) return avoidPosList;
            }
            distance++;
        }
        return -Vector2Int.one;
    }

    public ResourceAir GetBreathAir(Vector2Int _pos)
    {
        if (resourceMapMng.GetResourceByPos(_pos).GetType() == typeof(ResourceAir))
            return resourceMapMng.GetResourceByPos(_pos) as ResourceAir;
        else
            return resourceMapMng.GetNeighbourAir(_pos);
    }

    public void SetMovable(Vector2Int _pos, int _value)
    {
        moveMapMng.SetMovable(_pos, _value);
    }

    public void OnDigged(Vector2Int _pos)
    {
        resourceMapMng.OnDigged(_pos);
        moveMapMng.OnDigged(_pos);
    }

    public void OnBuild(Vector2Int _pos, int _type)
    {
        if(_type == 0)          //Ÿ���̶��
            moveMapMng.OnBuildTile(_pos);
        else if(_type == 1)    //��ٸ����
            moveMapMng.OnBuildLadder(_pos);
        
        buildingMapMng.Build(_pos, _type);

       
        resourceMapMng.OnBuild(_pos);
    }

    public void OnDestroyBuilding(Vector2Int _pos)
    {
        buildingMapMng.OnDestroyBuilding(_pos);
        resourceMapMng.OnDigged(_pos);
        moveMapMng.OnDestroyBuilding(_pos);
    }

    public int GetBuiling(Vector2Int _pos)
    {
        return buildingMapMng.GetBuilding(_pos);
    }

    public bool IsValidPos(Vector2Int _pos)
    {
        return (_pos.x >= 0 && _pos.x < sizeW && _pos.y >= 0 && _pos.y < sizeH);
    }

    public bool IsLadder(Vector2Int _pos)
    {
        return moveMapMng.IsLadder(_pos);
    }

    public bool CanStand(Vector2Int _pos)
    {
        return moveMapMng.CanStand(_pos);
    }
}
