using UnityEngine;
using System;

public class Data<T>
{
    private T v;
    public T Value
    {
        get
        {
            return this.v;
        }
        set
        {
            this.v = value;
            this.onChange?.Invoke(value);
        }
    }
    public System.Action<T> onChange;
}

public class DeathData<T>
{
    private T v;
    public T Value
    {
        get
        {
            return this.v;
        }
        set
        {
            this.v = value;
            this.onChange?.Invoke(value);
        }
    }
    public System.Action<T> onChange;
}