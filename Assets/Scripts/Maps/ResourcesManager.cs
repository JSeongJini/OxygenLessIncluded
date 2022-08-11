using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
   

    private int digedSandStone = 0;
    private int digedGold = 0;


    public void AddSandStone()
    {
        digedSandStone++;
    }

    public void AddGold()
    {
        digedGold++;
    }

    public void useSandStone(int _val)
    {
        if(digedSandStone >= _val)
        {
            digedSandStone -= _val;
        }
    }
    public void useGold(int _val)
    {
        if(digedGold >= _val)
        {
            digedGold -= _val;
        }
    }

    public int GetDigedSandStone()
    {
        return digedSandStone;
    }
    public int GetDigedGold()
    {
        return digedGold;
    }

}
