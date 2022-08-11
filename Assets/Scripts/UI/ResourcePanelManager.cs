using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanelManager : MonoBehaviour
{
    [SerializeField] private ResourcesManager resourceManager = null;
    [SerializeField] private Image bgImg = null;
    [SerializeField] private Image toggleBtn = null;
    [SerializeField] private Text sandStoneTxt = null;
    [SerializeField] private Text goldTxt = null;
    [SerializeField] private Sprite[] btnSprite = null;

    


    public void TogglePanel()
    {
        if(bgImg.gameObject.activeSelf)
        {
            bgImg.gameObject.SetActive(false);
            StopCoroutine("UpdateCurResource");
            toggleBtn.sprite = btnSprite[1];
        }
        else
        {
            bgImg.gameObject.SetActive(true);
            toggleBtn.sprite = btnSprite[0];
            StartCoroutine("UpdateCurResource");
        }
    }

    private IEnumerator UpdateCurResource()
    {
        while(true)
        {
            sandStoneTxt.text = "SandStone : " + resourceManager.GetDigedSandStone() + " kg";
            goldTxt.text = "Gold : " + resourceManager.GetDigedGold() + " kg";
            yield return null;
        }
    }



}
