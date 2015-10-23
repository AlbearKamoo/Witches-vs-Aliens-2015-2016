using UnityEngine;
using System.Collections;

public class FloatStat{
    public delegate void Recalculate();
    private float _value;
    protected float value { get { return _value; } set { _value = value; update(); } }
    private Recalculate update;
    public FloatStat(float val, Recalculate update)
    {
        value = val;
        this.update = update;
    }
    public static implicit operator float(FloatStat stat)
    {
        return stat.value;
    }
}
