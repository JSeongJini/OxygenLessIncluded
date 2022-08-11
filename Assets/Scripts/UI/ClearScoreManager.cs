using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearScoreManager : MonoBehaviour
{
    [SerializeField] private ResourcesManager resourceManager = null;
    [SerializeField] private GameObject missionClaerGo = null;
    [SerializeField] private Text[] clearTxt = null;
    

    private int sand = 0;
    private int gold = 0;
    private int totalPoint = 0;
    private readonly int sandpt = 100;
    private readonly int goldpt = 400;
    private readonly int clearpt = 40000;
    
    public void SetScore(bool _isWin)
    {
        sand = resourceManager.GetDigedSandStone();
        gold = resourceManager.GetDigedGold();
        totalPoint = (sand * sandpt) + (gold * goldpt);
        missionClaerGo.SetActive(true);
        SetTxt(_isWin);
       
    }

    private void SetTxt(bool _isWin)
    {
        clearTxt[0].text = sand + " x 100pt = " + (sand * sandpt) + "pt";
        clearTxt[1].text = gold + " x 400pt = " + (gold * goldpt) + "pt";
        clearTxt[2].text = (_isWin) ? clearpt + "pt" : " 0 pt";
        clearTxt[3].text = (totalPoint + ((_isWin) ? clearpt : 0)) + "pt";
        clearTxt[4].text = "다시 시작하시겠습니까?";
        clearTxt[5].text = (_isWin) ? "Mission Clear" : "Mission Fail";
    }
}
