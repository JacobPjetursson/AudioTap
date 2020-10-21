using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class HitObject : IComparable<HitObject>
{
    [System.Serializable]
    public enum HitType
    {
        Circle,
        Slider
        // more?
    }

    public float time;
    // specific to hitcircle
    public float x;
    public float y;
    // specific to slider
    public List<float> x_values;
    public List<float> y_values;
    public float duration;
    public int repeats = 1;

    public HitType type; // circle or slider


    public HitObject(float x, float y, float time)
    {
        this.x = x;
        this.y = y;
        this.time = time;
        this.type = HitType.Circle;
    }

    public HitObject(List<float> x_vals, List<float> y_vals, float time, float duration, int repeats)
    {
        this.x_values = x_vals;
        this.y_values = y_vals;
        this.time = time;
        this.repeats = repeats;
        this.duration = duration;
        this.type = HitType.Slider;
    }

    public int CompareTo(HitObject other)
    {
        return (time.CompareTo(other.time));
    }

    public override string ToString()
    {
       return "x: " + x + ", y: " + y + ", time: " + time;
    }


}