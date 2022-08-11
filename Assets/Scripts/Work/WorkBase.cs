using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorkBase
{
    protected Vector2Int targetPos;              //작업물의 위치 좌표
    protected float progress = 999f;
    protected SoundsManager soundsManager = null;

    public event Action onFinishWork = null;


    public WorkBase(Vector2Int _targetPos)
    {
        targetPos = _targetPos;
        soundsManager = GameObject.FindObjectOfType<SoundsManager>();
        onFinishWork += soundsManager.StopAudio;
    }

    public Vector2Int GetTargetPosition()
    {
        return targetPos;
    }

    public virtual float Work(float _power)
    {
        if (progress <= 0f)
            onFinishWork?.Invoke();

        return progress;
    }

    public static bool operator ==(WorkBase op1, WorkBase op2)
    {
        return (op1.targetPos == op2.targetPos) && op1.GetType() == op2.GetType();
    }

    public static bool operator !=(WorkBase op1, WorkBase op2)
    {
        return !((op1.targetPos == op2.targetPos) && op1.GetType() == op2.GetType());
    }

    public override bool Equals(object _other)
    {
        return this == _other as WorkBase;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
