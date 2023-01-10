using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceAir : ResourceBase
{
    public bool onFlow = false;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        UpdateTransparency();
        onConsume += (pos) => {
            UpdateTransparency();
        };
    }


    private void UpdateTransparency()
    {
        Color color = sr.color;
        color.a = (float)amount / maxAmount * 0.2f;
        sr.color = color;
    }

}
