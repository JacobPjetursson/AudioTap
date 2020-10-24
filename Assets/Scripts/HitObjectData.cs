using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HitObjectData : IComparable<HitObjectData>
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
    public Vector3 position;
    // specific to slider
    public Vector3[] positions;
    public float duration;
    public bool durationInBeats; // set duration in beats or in seconds?
    public int repeats = 1;

    public HitType type; // circle or slider


    public HitObjectData(Vector3 position, float time)
    {
        this.position = position;
        this.time = time;
        this.type = HitType.Circle;
    }

    public HitObjectData(Vector3[] positions, float time, float duration, int repeats, bool durationInBeats = true)
    {
        this.positions = positions;
        this.time = time;
        this.repeats = repeats;
        this.duration = duration;
        this.durationInBeats = durationInBeats;
        this.type = HitType.Slider;
    }

    public int CompareTo(HitObjectData other)
    {
        return (time.CompareTo(other.time));
    }

    public override string ToString()
    {
       return "x: " + position.x + ", y: " + position.y + ", time: " + time;
    }


}