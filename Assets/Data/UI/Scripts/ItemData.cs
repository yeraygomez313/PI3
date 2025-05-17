using System;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    
}

public class ItemInstance
{
    public Guid Guid { get; protected set; }
    public virtual ItemData Data { get; protected set; }

    public void Initialize(ItemData item)
    {
        Guid = Guid.NewGuid();
        Data = item;
    }
}
