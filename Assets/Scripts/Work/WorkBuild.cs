using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorkBuild : WorkBase
{
    private int type = 0;

    public WorkBuild(Vector2Int _targetPos, int _type) : base(_targetPos)
    {
        type = _type;
    }

    public override float Work(float _power)
    {
        soundsManager.PlayAudio(type);
        progress -= _power * 100f;
        base.Work(_power);
        
        return progress;
    }

}
