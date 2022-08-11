using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkDestroy : WorkBase
{
    public WorkDestroy(Vector2Int _targetPos) : base(_targetPos)
    { 
    }

    public override float Work(float _power)
    {
        soundsManager.PlayAudio(4);
        progress -= _power * 100f;
        base.Work(_power);

        return progress;
    }
}
