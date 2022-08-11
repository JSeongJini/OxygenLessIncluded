using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceAir : ResourceBase
{
    protected override void Awake()
    {
        base.Awake();
        sr.sprite = sprites[24];
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
