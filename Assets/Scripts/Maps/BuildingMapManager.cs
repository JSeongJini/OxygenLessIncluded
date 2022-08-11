using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMapManager : MonoBehaviour
{
    private int sizeW = 0;
    private int sizeH = 0;
    private int[,] buildingMap = null;

    private GameObject[] buildingPrefabs = null;
    private List<GameObject> buildings = null;

    private GameObject rocket = null;

    /*
        buildingMap == 0  >> ��� ����
        buildingMap == 1  >> Ÿ��
        buildingMap == 2  >> ��ٸ�
     */


    //TODO : �������� �޾ƿ��� �ƴϾƴϾƴϵǿ�!!
    public void InitializeBuildingMap(int _sizeW, int _sizeH, int[,] _mapInfo)
    {
        //���� �� �ʱ�ȭ
        sizeW = _sizeW;
        sizeH = _sizeH;
        buildingMap = new int[sizeW, sizeH];
        buildings = new List<GameObject>();

        buildingPrefabs = Resources.LoadAll<GameObject>("Prefabs\\Buildings\\");

        //ó������ �ƹ��� ���๰�� �����Ƿ� ��� 0 : �������
        for (int y = 0; y < sizeH; y++)
            for (int x = 0; x < sizeW; x++)
                buildingMap[x, y] = 0;
    }

    public void Build(Vector2Int _pos, int _type)
    {
        if (buildingMap == null) return;

        buildingMap[_pos.x, _pos.y] = _type + 1;
        GameObject buildingGo = Instantiate(buildingPrefabs[_type], new Vector3(_pos.x, _pos.y, 0f), Quaternion.identity);
        buildingGo.transform.SetParent(transform);
        buildings.Add(buildingGo);

        if (_type == 2)  //�����̶��
            rocket = buildingGo;
    }

    public void OnDestroyBuilding(Vector2Int _pos)
    {
        buildingMap[_pos.x, _pos.y] = 0;        //�������
        GameObject destroyBuilding;
        foreach(GameObject building in buildings)
            if(building.transform.position == new Vector3(_pos.x, _pos.y, 0f))
            {
                destroyBuilding = building;
                buildings.Remove(destroyBuilding);
                Destroy(destroyBuilding);
                break;
            }
    }

    public int GetBuilding(Vector2Int _pos)
    {
        return buildingMap[_pos.x, _pos.y];
    }


    public void LaunchRocket()
    {
        Animator anim = rocket.GetComponentInChildren<Animator>();
        anim.GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine("LaunchRocketCoroutine", rocket.transform);
    }

    private IEnumerator LaunchRocketCoroutine(Transform _rocketTr)
    {
        float elapsed = 0f;
        while (elapsed < 5f)
        {
            elapsed += Time.deltaTime;
            yield return null;
            _rocketTr.Translate(Vector3.up * Time.deltaTime * 2f);
        }
    }
}
