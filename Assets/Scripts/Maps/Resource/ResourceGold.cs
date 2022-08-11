using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGold : ResourceSandStone
{
    protected override void Awake()
    {
        base.Awake();
        sr.sprite = sprites[8];
    }

}
