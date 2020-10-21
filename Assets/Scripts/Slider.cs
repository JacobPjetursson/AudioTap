using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slider : MonoBehaviour
{

    public GameObject startCircle;

    private List<float> x_vals, y_vals;
    private float duration;
    private float opacity;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setup(HitObject hitObj)
    {
        this.x_vals = hitObj.x_values;
        this.y_vals = hitObj.y_values;
        this.duration = hitObj.duration;
    }
}
