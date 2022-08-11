using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{
    [SerializeField] private Sprite[] spriteList = null;
    [SerializeField] private Image overlayBtn = null;

    private bool btnToggle = false;


    public void ToggleImage()
    {
        if (!btnToggle)
        {
            overlayBtn.sprite = spriteList[0];
            btnToggle = true;
        }
        else
        {
            overlayBtn.sprite = spriteList[1];
            btnToggle = false;
        }
    }
}
