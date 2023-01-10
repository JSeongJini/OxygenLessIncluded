using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

public class ResourceMapManager : MonoBehaviour
{
    [SerializeField] private ResourcesManager resourceManager = null;

    [Header("Tile Prefabs")]
    [SerializeField] private GameObject sandstonePrefab = null;
    [SerializeField] private GameObject airPrefab = null;
    [SerializeField] private GameObject goldPrefab = null;

    private static ProfilerMarker myMaker = new ProfilerMarker("FindBottleNeck");
    private static ProfilerMarker myMaker2 = new ProfilerMarker("FindBottleNeck2");

    private ResourceBase[, ] resourceMap = null;

    private int sizeW = 0;
    private int sizeH = 0;

    public void InitializeResouceMap(int _sizeW, int _sizeH, int[, ] _mapInfo)
    {
        //자원 맵 초기화
        sizeW = _sizeW;
        sizeH = _sizeH;
        resourceMap = new ResourceBase[_sizeW, _sizeH];

       
        //맵 정보로부터 알맞은 자원 생성 및 위치 초기화
        for (int y = 0; y < _sizeH; y++)
            for (int x = 0; x < _sizeW; x++)
            {
                resourceMap[x, y] = ResourceFactory((EResourceType)_mapInfo[x, y], 999f);
                resourceMap[x, y].transform.position = new Vector3(x, y, 0f);
            }
    
        //사암 자원 스프라이트를 9-Slice 룰에 맞게 적용
        for (int y = 0; y < sizeH; y++) {
            for(int x = 0; x < sizeW; x++)
            UpdateSandStoenSprite(new Vector2Int(x, y));
        }
    }

    private ResourceBase ResourceFactory(EResourceType _type, float _amount)
    {
        GameObject go = null;
        switch (_type)
        {
            case EResourceType.SandStone:
                //사암 사이사이에 금(Gold) 자원 끼워넣기
                if (UnityEngine.Random.Range(0f, 100f) < 5f)
                {
                    go = Instantiate(goldPrefab, transform);
                    go.GetComponent<ResourceGold>().onDestroy += (pos) =>
                    {
                        resourceManager.AddGold();
                    };
                }
                else
                {
                    myMaker.Begin();
                    go = Instantiate(sandstonePrefab, transform);
                    myMaker.End();
                    myMaker2.Begin();
                    go.GetComponent<ResourceSandStone>().onDestroy += (pos) =>
                    {
                        resourceManager.AddSandStone();
                    };
                    myMaker2.End();
                }
                break;
            case EResourceType.Air:
                go = Instantiate(airPrefab, transform);
                go.GetComponent<ResourceAir>().onConsume += FlowAir;
                break;
            case EResourceType.Empty:
                go = new GameObject("ResourceEmpty");
                go.transform.SetParent(transform);
                return go.AddComponent<ResourceEmpty>();
            default:
                break;
        }
       
        ResourceBase rb = go.GetComponent<ResourceBase>();
        rb.Gain(_amount);

        return rb;
    }

    public ResourceBase GetResourceByPos(Vector2Int _pos)
    {

        return resourceMap[_pos.x, _pos.y];
    }

    private void UpdateSandStoenSprite(Vector2Int _pos)
    {
        if (!IsValidPos(_pos)) return;

        //사암이 아니라면 리턴
        if (resourceMap[_pos.x, _pos.y].GetType() != typeof(ResourceSandStone)) return;

        //좌, 우, 위, 아래 방향이 사암 혹은 맵 밖인지의 여부를 검사
        bool left = false;
        bool right = false;
        bool top = false;
        bool bottom = false;

        left = (_pos.x == 0 || resourceMap[_pos.x - 1, _pos.y].GetType() != typeof(ResourceAir));
        right = (_pos.x == (sizeW -1) || resourceMap[_pos.x + 1, _pos.y].GetType() != typeof(ResourceAir));
        top = (_pos.y == (sizeH - 1) || resourceMap[_pos.x, _pos.y + 1].GetType() != typeof(ResourceAir));
        bottom = (_pos.y == 0 || resourceMap[_pos.x, _pos.y - 1].GetType() != typeof(ResourceAir));
        
        ((ResourceSandStone)resourceMap[_pos.x, _pos.y]).SpriteUpdate(left, right, top, bottom);
    }

    public void OnDigged(Vector2Int _diggedPos)
    {
        //부서진 타일을 산소로 변경 
        if(resourceMap[_diggedPos.x, _diggedPos.y] is ResourceEmpty)
        {
            GameObject.Destroy(resourceMap[_diggedPos.x, _diggedPos.y].gameObject);
        }
        resourceMap[_diggedPos.x, _diggedPos.y] = ResourceFactory(EResourceType.Air, 1f);
        resourceMap[_diggedPos.x, _diggedPos.y].transform.position = new Vector3(_diggedPos.x, _diggedPos.y, 0f);
        resourceMap[_diggedPos.x, _diggedPos.y].Consume(0f);

        //부서진 타일의 상, 하, 좌, 우에 있는 사암 스프라이트 업데이트
        UpdateSandStoenSprite(new Vector2Int(_diggedPos.x, _diggedPos.y + 1));
        UpdateSandStoenSprite(new Vector2Int(_diggedPos.x, _diggedPos.y - 1));
        UpdateSandStoenSprite(new Vector2Int(_diggedPos.x + 1, _diggedPos.y));
        UpdateSandStoenSprite(new Vector2Int(_diggedPos.x - 1, _diggedPos.y));
    }

    private void FlowAir(Vector2Int _pos)
    {
        ResourceAir air = GetResourceByPos(_pos) as ResourceAir;
        if (air && !air.onFlow)
        {
            air.onFlow = true;
            StartCoroutine("FlowAirCoroutine", _pos);
        }
    }

    private IEnumerator FlowAirCoroutine(Vector2Int _pos)
    {
        ResourceAir neighbourAir = GetNeighbourAir(_pos);
        ResourceAir air = GetResourceByPos(_pos) as ResourceAir;

        //근처에 산소가 있다면
        if (neighbourAir)
        {
            float ownAmount = resourceMap[_pos.x, _pos.y].GetAmount();
            float negihbourAmount = neighbourAir.GetAmount();

            while (Mathf.Abs(negihbourAmount - ownAmount) > 20f)
            {
                //산소량이 많은 산소에서 적은 산소로 산소 이동
                if (negihbourAmount >= ownAmount)
                {
                    float delta = Mathf.Lerp(0f, (negihbourAmount - ownAmount), 0.1f);
                    resourceMap[_pos.x, _pos.y].Gain(delta);
                    neighbourAir.Consume(delta);
                }
                else
                {
                    float delta = Mathf.Lerp(0f, (ownAmount - negihbourAmount), 0.1f);
                    resourceMap[_pos.x, _pos.y].Consume(delta);
                    neighbourAir.Gain(delta);
                }

                yield return new WaitForSeconds(1f);

                ownAmount = resourceMap[_pos.x, _pos.y].GetAmount();
                negihbourAmount = neighbourAir.GetAmount();
            }
            air.onFlow = false;
        }
        yield return null;
    }
 
    public ResourceAir GetNeighbourAir(Vector2Int _pos)
    {
        List<ResourceAir> neighbourAir = new List<ResourceAir>();
        if (resourceMap[_pos.x, _pos.y + 1].GetType() == typeof(ResourceAir))
            neighbourAir.Add(((ResourceAir)resourceMap[_pos.x, _pos.y + 1]));
        if (resourceMap[_pos.x - 1, _pos.y].GetType() == typeof(ResourceAir))
            neighbourAir.Add(((ResourceAir)resourceMap[_pos.x -1, _pos.y]));
        if (resourceMap[_pos.x + 1, _pos.y].GetType() == typeof(ResourceAir))
            neighbourAir.Add(((ResourceAir)resourceMap[_pos.x + 1, _pos.y]));
        if (resourceMap[_pos.x, _pos.y - 1].GetType() == typeof(ResourceAir))
            neighbourAir.Add(((ResourceAir)resourceMap[_pos.x, _pos.y - 1]));

        if (neighbourAir.Count == 0) return null;

        //주변에 있는 산소들 중 가장 산소량이 많은 산소 반환
        ResourceAir bigger = neighbourAir[0];
        for(int i = 1; i < neighbourAir.Count; i++)
        {
            if (bigger.GetAmount() < neighbourAir[i].GetAmount())
                bigger = neighbourAir[i];
        }

        return bigger;
    }

    public void OnBuild(Vector2Int _buildPos)
    {
        //해당 위치에 있던 산소가 근처 산소로 옮겨가며 없어짐
        if (resourceMap[_buildPos.x, _buildPos.y].GetType() == typeof(ResourceAir))
        {
            float amout = resourceMap[_buildPos.x, _buildPos.y].GetAmount();
            resourceMap[_buildPos.x, _buildPos.y] = ResourceFactory(EResourceType.Empty, 1f);
            resourceMap[_buildPos.x, _buildPos.y].transform.position = new Vector3(_buildPos.x, _buildPos.y, 0f);

            List<ResourceAir> neighbourAir = new List<ResourceAir>();
            int[] dx = { 0, -1, 1, 0 };
            int[] dy = { 1, 0, 0, -1 };
            for (int i = 0; i < 4; i++)
                if (resourceMap[_buildPos.x + dx[i], _buildPos.y + dy[i]].GetType() == typeof(ResourceAir))
                    neighbourAir.Add(((ResourceAir)resourceMap[_buildPos.x + dx[i], _buildPos.y + dy[i]]));

            if (neighbourAir.Count != 0)
                amout /= neighbourAir.Count;

            foreach (ResourceBase rb in neighbourAir)
                rb.Gain(amout);
        }
        
    }

    private bool IsValidPos(Vector2Int _pos)
    { 
        return !(_pos.x < 0 || _pos.x > sizeW || _pos.y < 0 || _pos.y > sizeH);
    }
}
