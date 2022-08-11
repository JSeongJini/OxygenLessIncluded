using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesOverlay : MonoBehaviour
{
    [SerializeField] private ResourceMapManager resourceMapManager = null;
    private GameObject[,] oxygenMap = null;
   
    private Sprite[] sprites = null;

    private Vector3 leftBottom = Vector3.zero;
    private Vector3 topRight = Vector3.zero;
    private int sizeW = 0;
    private int sizeH = 0;
    private bool toggleOverlay = false;

    private void Awake()
    {
        sprites = Resources.LoadAll<Sprite>("Sprites\\Sprite_Tile");
        SetOverlayPool();
        
    }


    public void StartOverlayMap()
    {
        ToggleOverlay();
    }
       
    private IEnumerator OverlayCoroutine()
    {
        while(true)
        {
            //맵 정보로부터 알맞은 자원 생성 및 위치 초기화
            leftBottom = Camera.main.ViewportToWorldPoint(Vector3.zero);
            topRight = Camera.main.ViewportToWorldPoint(Vector3.one);
            Debug.Log(leftBottom);
            Debug.Log(topRight);

            int my = Mathf.RoundToInt(leftBottom.y);
            int mx = Mathf.RoundToInt(leftBottom.x);

          
                for ( int y = my ; y < Mathf.RoundToInt(topRight.y + 1); y++)
                {

                    for (int x = mx ; x < Mathf.RoundToInt(topRight.x + 1); x++)
                    {
                        //y부터 x까지 좌하단부터 우상단까지 순차적으로 x,y 진입함 

                        ResourceBase rb = resourceMapManager.GetResourceByPos(new Vector2Int(x, y));

                        Debug.Log(x + ", " + y);

                        oxygenMap[x, y].gameObject.SetActive(true);

                        if (rb.GetType() == typeof(ResourceAir))
                        {
                            oxygenMap[x, y].GetComponent<SpriteRenderer>().sprite = sprites[21];
                            oxygenMap[x, y].GetComponent<SpriteRenderer>().color = ChangeColor(0.2f, 1f, 0.2f, rb.GetAmount() / 1000);
                        }
                        else if (rb.GetType() == typeof(ResourceGold))
                        {
                            oxygenMap[x, y].GetComponent<SpriteRenderer>().sprite = sprites[21];
                            oxygenMap[x, y].GetComponent<SpriteRenderer>().color = ChangeColor(0.7f, 0.7f, 0.2f, rb.GetAmount() / 1000);
                        }
                        else if(rb.GetType() == typeof(ResourceSandStone))
                        {
                            oxygenMap[x, y].GetComponent<SpriteRenderer>().sprite = sprites[21];
                            oxygenMap[x, y].GetComponent<SpriteRenderer>().color = ChangeColor(0.2f, 0.2f, 0.2f, rb.GetAmount() / 1000);
                        }
                    }
                }
            yield return null;
        }
    }
       
    private void SetOverlayPool()
    {
        sizeW = 192;
        sizeH = 108;
        oxygenMap = new GameObject[sizeW, sizeH];
        for (int y = 0; y < sizeH; y++)
        {
            for (int x = 0; x < sizeW; x++)
            {
                oxygenMap[x, y] = new GameObject("Air Overlay");
                oxygenMap[x, y].AddComponent<SpriteRenderer>();
                oxygenMap[x, y].transform.SetParent(transform);
                oxygenMap[x, y].transform.position = new Vector3(x, y);
                oxygenMap[x, y].gameObject.SetActive(false);
            }
        }
    }

    private void ToggleOverlay()
    {
        if (!toggleOverlay)
        {
            toggleOverlay = true;
            StartCoroutine("OverlayCoroutine");
        }
        else
        {
            for (int y = 0; y < sizeH; y++)
            {
                for (int x = 0; x < sizeW; x++)
                {
                    if (oxygenMap[x, y].activeSelf)
                    {
                        oxygenMap[x, y].SetActive(false);
                    }
                }
            }
            toggleOverlay = false;
            StopCoroutine("OverlayCoroutine");
        }
    }
    private Color ChangeColor(float _r, float _g, float _b, float _a)
    {
        float a = _a * 0.3f;

        return new Color(_r, _g, _b , a);
    }

   



}
