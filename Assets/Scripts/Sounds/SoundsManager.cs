using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    private static SoundsManager instance;
    public static SoundsManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    //0.BuildTile, 1.BuildLedder, 2.DigSandStone, 3.DigGold, 4.Destroy, 5.UI, 6.Die, 7.ClearWin, 8.ClearFail, 9.Caution
    public enum ESFX {BuildTile, BuildLedder,DigSandStone, DigGold, Destroy, UI, Die, ClearWin, ClearFail, Caution}
    private AudioClip[] sfxList = null;

    private AudioSource audioSource = null;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        sfxList = Resources.LoadAll<AudioClip>("Audio\\SFX");
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void PlayAudio(int _val)
    {
        audioSource.PlayOneShot(sfxList[_val]);

        return;
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }

}
