using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceBase : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer sr = null; 
    
    [SerializeField] protected float amount = 0f;
    protected readonly int maxAmount = 999;

    public delegate void OnChanged(Vector2Int _pos);
    public OnChanged onDestroy = null;
    public OnChanged onConsume = null;
    protected Vector2Int pos;
    

    protected virtual void Awake()
    {
        //sr = GetComponent<SpriteRenderer>();
        pos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }

    public float Consume(float _value)
    {
        amount -= _value;
        if(amount <= 0f)
        {
            Destroy();
            return _value + amount;
        }
        else
        {
            GetPosition();
            onConsume?.Invoke(pos);
            return _value;
        }
    }

    public float Gain(float _value)
    {
        amount += _value;
        if (amount > maxAmount)
            amount = maxAmount;

        return amount;
    }

    public float GetAmount()
    {
        return amount;
    }

    public void GetPosition()
    {
        pos.x = (int)transform.position.x;
        pos.y = (int)transform.position.y;
    }

    protected void Destroy()
    {
        onDestroy?.Invoke(pos);
        Destroy(gameObject);
    }

    
}
