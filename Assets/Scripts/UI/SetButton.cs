using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetButton : MonoBehaviour
{

    [SerializeField] private List<GameObject> iconList = new List<GameObject>();

    public void ActiveIcon(int _type)
    {
        for(int i = 0; i < iconList.Count; i++)
        {
            iconList[i].SetActive(false);
        }
        iconList[_type].SetActive(true);        
        
        return;
    }


}
