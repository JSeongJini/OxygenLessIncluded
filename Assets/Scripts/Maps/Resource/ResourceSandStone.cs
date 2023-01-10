using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSandStone : ResourceBase
{
    [SerializeField] private Sprite[] sprites = null;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SpriteUpdate(bool _left, bool _right, bool _top, bool _bottom)
    {
        if (!sr) return;
        //주변 타일이 같은 사암인지의 여부에 따라서
        //9 Slice된 스프라이트 중 적절한 스프라이트로 변경
        if (!_left && _right && !_top && _bottom)        //TopLeft
            sr.sprite = sprites[0];
        else if ((_left && _right && !_top && _bottom) ||
            (!_left && !_right && !_top && _bottom))         //Top
            sr.sprite = sprites[1];
        else if (_left && !_right && !_top && _bottom)        //TopRight
            sr.sprite = sprites[2];
        else if ((!_left && _right && _top && _bottom) ||
            (!_left && _right && !_top && !_bottom))         //Left
            sr.sprite = sprites[3];
        else if ((_left && !_right && _top && _bottom) ||
            (_left && !_right && !_top && !_bottom))         //Right
            sr.sprite = sprites[4];
        else if (!_left && _right && _top && !_bottom)        //BottomLeft
            sr.sprite = sprites[5];
        else if ((_left && _right && _top && !_bottom) ||
            (!_left && !_right && _top && !_bottom))         //Bottom
            sr.sprite = sprites[6];
        else if (_left && !_right && _top && !_bottom)        //BottomLeft
            sr.sprite = sprites[7];
        else                                                                //Center
            sr.sprite = sprites[8];
    }
}
