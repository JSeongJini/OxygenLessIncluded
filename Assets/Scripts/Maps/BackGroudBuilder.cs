using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroudBuilder : MonoBehaviour
{
    private GameObject[,] backgroundMap = null;
    private Sprite[] sprites = null;
    private int sizeW = 0;
    private int sizeH = 0;

    private void Awake()
    {
        sprites = Resources.LoadAll<Sprite>("Sprites\\Sprite_BgTile");
        SetBackgroundPool();
        Test();
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
                backgroundMap[x, y] = new GameObject("BackgroundImage");
                backgroundMap[x, y].AddComponent<SpriteRenderer>().sortingOrder = -2;
                backgroundMap[x, y].transform.SetParent(transform);
                backgroundMap[x, y].transform.position = new Vector3(x, y);
                backgroundMap[x, y].gameObject.SetActive(true);
            }
        }
    }

    private void Test()
    {
        for (int y = 0; y < sizeH; y++)
        {

            for (int x = 0; x < sizeW; x++)
            {
                //y���� x���� ���ϴܺ��� ���ܱ��� ���������� x,y ������ 


                if (Random.Range(0f, 100f) < 5f)
                {
                    backgroundMap[x, y].GetComponent<SpriteRenderer>().sprite = sprites[13];
                }
                else
                {
                    backgroundMap[x, y].GetComponent<SpriteRenderer>().sprite = sprites[10];
                }
                    backgroundMap[x, y].GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
            }
        }
    }

}