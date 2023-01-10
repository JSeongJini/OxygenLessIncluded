using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

public class WorkManager : MonoBehaviour
{
    [SerializeField] private NPCManager npcManager = null;
    [SerializeField] private MapManager mapManager = null;
    [SerializeField] private ResourcesManager resourcesManager = null;
    [SerializeField] private SoundsManager soundsManager = null;
    [SerializeField] private GameObject[] signPrefab = null;
    [SerializeField] private CautionManager cautionManager = null;

    private List<GameObject> signs = null;
    private List<WorkBase> waitStream = null;
    private List<WorkBase> doingList = null;

    private void Awake()
    {
        waitStream = new List<WorkBase>();
        doingList = new List<WorkBase>();
        signs = new List<GameObject>();
        npcManager.ConnectWorkStream(waitStream, doingList);
    }

    public void RequestDig(Vector2Int _targetPos)
    {

        ResourceBase resource = mapManager.GetResourceByPos(_targetPos);

        if (resource is ResourceSandStone || resource is ResourceGold)
        {
            WorkDig dig = new WorkDig(_targetPos, resource);
            bool condition = (!waitStream.Contains(dig) && !doingList.Contains(dig));

            if (condition)
            {
                waitStream.Add(dig);
                GameObject digSignGo = MakeSign(_targetPos, 0);
                dig.onFinishWork += () =>
                {
                    Destroy(digSignGo);
                    mapManager.OnDigged(_targetPos);
                    npcManager.FindNewPathAll();
                };
            }
        }

    }

    public void RequestBuild(Vector2Int _targetPos, int _type)
    {
        //이미 무언가가 건축되어 있는 자리라면 리턴
        if (mapManager.GetBuiling(_targetPos) != 0)
            return;
        
        int sandStone = resourcesManager.GetDigedSandStone();
        //보유 사암이 건축 비용보다 적다면 리턴
        if (sandStone < (2 - _type))
        {
            cautionManager.ShowCaution(1);
            return;
        }
        resourcesManager.useSandStone(3 - _type);

        ResourceBase resource = mapManager.GetResourceByPos(_targetPos);
        //건설하려는 곳에 사암이나 금이 있다면 굴착 먼저 진행
        if (resource.GetType() == typeof(ResourceSandStone) || resource.GetType() == typeof(ResourceGold))
            RequestDig(_targetPos);

        WorkBase build = new WorkBuild(_targetPos, _type);
        if (!waitStream.Contains(build) && !doingList.Contains(build))
        {
            waitStream.Add(build);
            GameObject signGo = MakeSign(_targetPos, _type +1);
            build.onFinishWork += () =>
            {
                Destroy(signGo);
                mapManager.OnBuild(_targetPos, _type);
                npcManager.FindNewPathAll();
            };
        }
    }

    public void RequestDestroy(Vector2Int _targetPos)
    {
        //해당 자리에 건축물이 없다면 리턴
        if (mapManager.GetBuiling(_targetPos) == 0)
            return;

        WorkDestroy destroy = new WorkDestroy(_targetPos);
        if (!waitStream.Contains(destroy) && !doingList.Contains(destroy))
        {
            waitStream.Add(destroy);
            GameObject signGo = MakeSign(_targetPos, 3);
            destroy.onFinishWork += () =>
            {
                Destroy(signGo);
                npcManager.FindNewPathAll();
                mapManager.OnDestroyBuilding(_targetPos);
            };
        }
    }

    public void RequestCancel(Vector2Int _targetPos)
    {
        foreach(WorkBase work in waitStream)
        {
            if(work.GetTargetPosition() == _targetPos)
            {
                WorkBase cancel = work;
                waitStream.Remove(cancel);
                break;
            }
        }

        Vector3 pos = new Vector3(_targetPos.x, _targetPos.y, 0f);
        foreach(GameObject sign in signs)
        {
            if(sign != null && sign.transform.position == pos)
            {
                signs.Remove(sign);
                Destroy(sign);
                return;
            }
        }
    }

    public void RequestRocket(Vector2Int _targetPos)
    {
        //이미 무언가가 건축되어 있는 자리라면 리턴
        if (mapManager.GetBuiling(_targetPos) != 0)
            return;

        int gold = resourcesManager.GetDigedGold();
        //보유 사암이 건축 비용보다 적다면 리턴
        if (gold < 15)
        {
            cautionManager.ShowCaution(1);
            return;
        }
        resourcesManager.useGold(15);

        ResourceBase resource = mapManager.GetResourceByPos(_targetPos);
        //건설하려는 곳에 사암이나 금이 있다면 굴착 먼저 진행
        if (resource.GetType() == typeof(ResourceSandStone) || resource.GetType() == typeof(ResourceGold))
            RequestDig(_targetPos);

        WorkBase build = new WorkBuild(_targetPos, 0);
        if (!waitStream.Contains(build) && !doingList.Contains(build))
        {
            waitStream.Add(build);
            GameObject signGo = MakeSign(_targetPos, 4);
            build.onFinishWork += () =>
            {
                Destroy(signGo);
                mapManager.OnBuild(_targetPos, 2);
                npcManager.GatherNPCAll(_targetPos);
            };
        }
    }


    public GameObject MakeSign(Vector2Int _pos, int _type)
    {
        GameObject signGo = Instantiate(signPrefab[_type], new Vector3(_pos.x, _pos.y, 0f), Quaternion.identity);
        signGo.transform.SetParent(transform);
        signs.Add(signGo);
        return signGo;
    }

}
