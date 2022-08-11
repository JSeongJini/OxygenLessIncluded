using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private GameObject clockGo = null;
    [SerializeField] private List<Image> clockBtnImg = new List<Image>();

    [SerializeField] private float defaultTime = 0f;
    private float nigthTime = 18f;
    private float oneDay = 10f;
    [SerializeField] private int dayCount = 0;

    private Sprite[] sprites = null;

    public event Action onNight;


    private void Awake()
    {
        sprites = Resources.LoadAll<Sprite>("Sprites\\Sprite_Cursor");
        StartTime();
        NomalTimeScale();

    }
    public void StartTime()
    {
        StartCoroutine("ElapsedTime");
    }

    public float GetCurTime()
    {
        return defaultTime;
    }
    public void SetCurTime(float _val)
    {
        defaultTime = _val;
    }

    public void StopTimeScale()
    {
        Time.timeScale = 0;
        clockBtnImg[0].sprite = sprites[19];
        clockBtnImg[1].sprite = sprites[11];
        clockBtnImg[2].sprite = sprites[21];
    }
    public void NomalTimeScale()
    {
        Time.timeScale = 1;
        clockBtnImg[1].sprite = sprites[9];
        clockBtnImg[0].sprite = sprites[22];
        clockBtnImg[2].sprite = sprites[21];
    }
    public void DoubleTimeScale()
    {
        Time.timeScale = 2;
        clockBtnImg[2].sprite = sprites[20];
        clockBtnImg[0].sprite = sprites[22];
        clockBtnImg[1].sprite = sprites[11];
    }

    private IEnumerator ElapsedTime()
    {
        while(true)
        {
            defaultTime += Time.deltaTime;
            clockGo.transform.eulerAngles = -Vector3.forward * defaultTime * oneDay;
            yield return null;

            if(defaultTime >= nigthTime)
            {
                nigthTime += 36f;
                dayCount++;
                onNight?.Invoke();
            }
        }
    }


}
