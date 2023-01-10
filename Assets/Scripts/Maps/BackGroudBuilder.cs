using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroudBuilder : MonoBehaviour
{
    [SerializeField] private Sprite sprite = null;
    [SerializeField] private GameObject backgroundPrefab = null;


    private GameObject[,] backgroundMap = null;
    private int sizeW = 0;
    private int sizeH = 0;

    private void Awake()
    {
        SetBackgroundPool();
    }

    private void SetBackgroundPool()
    {
        sizeW = 192;
        sizeH = 108;
        backgroundMap = new GameObject[sizeW, sizeH];
        for (int y = 0; y < sizeH; y++)
        {
            for (int x = 0; x < sizeW; x++)
            {
                backgroundMap[x, y] = Instantiate(backgroundPrefab, transform);
                backgroundMap[x, y].transform.position = new Vector3(x, y);

                if (Random.Range(0f, 100f) < 5f)
                {
                    backgroundMap[x, y].GetComponent<SpriteRenderer>().sprite = sprite;
                }
            }
        }
    }

    //private void Test()
    //{
    //    for (int y = 0; y < sizeH; y++)
    //    {

    //        for (int x = 0; x < sizeW; x++)
    //        {
    //            //y부터 x까지 좌하단부터 우상단까지 순차적으로 x,y 진입함 


    //            if (Random.Range(0f, 100f) < 5f)
    //            {
    //                backgroundMap[x, y].GetComponent<SpriteRenderer>().sprite = sprites[13];
    //            }
    //            else
    //            {
    //                backgroundMap[x, y].GetComponent<SpriteRenderer>().sprite = sprites[10];
    //            }
    //            backgroundMap[x, y].GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
    //        }
    //    }
    //}

}
