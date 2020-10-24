using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slider : HitObject
{
    public Vector3[] positions;
    public float duration;
    public bool durationInBeats = true;

    // todo - change to curves (using splines perhaps)
    private LineRenderer lineRenderer;
    private bool sliding = false;
    private float sliderLength;
    private float slideRate; // length / time
    private int targetIndex = 1; // position sliding towards

    // fixme - shouldn't need a variable to check whether slider is ongoing...
    protected bool finished = false;

    protected override void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
        // set positions
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
        // set width
        float circleRadius = GetComponent<CircleCollider2D>().radius;
        float width = MapGenerator.Instance.hitObjectScale * (circleRadius * 2);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        // adjust duration if it is given in beats
        if (durationInBeats)
            duration /= (MapGenerator.Instance.bpm / 60);

        sliderLength = getSliderLength();
        slideRate = sliderLength / duration;

        // set slider transparent
        lineRenderer.startColor = new Color(1f, 1f, 1f, 0f);
        lineRenderer.endColor = new Color(1f, 1f, 1f, 0f);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        // set slider opacity
        if (approaching)
        {
            lineRenderer.startColor = new Color(1f, 1f, 1f, opacity);
            lineRenderer.endColor = new Color(1f, 1f, 1f, opacity);
        }

        else if (sliding)
        {
            Slide();
        }
    }

    private float getSliderLength()
    {
        // loops though all points to compute the total length of the line
        float distance = 0f;
        for (int i = 0; i < positions.Length - 1; i++)
        {
            Vector2 pos1 = new Vector2(positions[i].x, positions[i].y);
            Vector2 pos2 = new Vector2(positions[i + 1].x, positions[i + 1].y);
            distance += Vector2.Distance(pos1, pos2);
        }
        return distance;
    }

    private void Slide()
    {
        // slide for 'duration' amount of time
        // the sliding time from curr to target is computed by looking at relative length
        float slideStep = slideRate * Time.fixedDeltaTime;
        Vector2 curr = transform.position;
        Vector2 target = positions[targetIndex];
        transform.position = Vector2.MoveTowards(curr, target, slideStep);

        if (Vector2.Distance(curr, target) <= Vector2.kEpsilon)
        {
            if (targetIndex == (positions.Length - 1))
            {
                // reached end of slider
                if (!missed)
                    GameManager.Instance.hit(target);
                finished = true;
                Destroy(gameObject);

            } else
            {
                // change target to next position in line
                targetIndex++;
            }
            
        }

    }

    // called from the generation script
    public void setup(HitObjectData hitObj)
    {
        this.positions = hitObj.positions;
        this.duration = hitObj.duration;
        this.durationInBeats = hitObj.durationInBeats;
    }

    // called from EventManager when mouse hit within bounds
    public override void Hit()
    {
        base.Hit();
        approaching = false;
        sliding = true;
        // set opacity
        lineRenderer.startColor = new Color(1f, 1f, 1f, 1f);
        lineRenderer.endColor = new Color(1f, 1f, 1f, 1f);
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);

        // hide approaching circle
        approachCircle.SetActive(false);
        
    }

    // also called from EventManager or if slider is left untouched
    public override void Miss()
    {
        base.Miss();
        // set half opaque and give red tint
        sliding = true; // start sliding if player doesn't hit it
        GameManager.Instance.missed(gameObject.transform.position);
        lineRenderer.startColor = new Color(1f, 0.8f, 0.8f, 0.5f);
        lineRenderer.endColor = new Color(1f, 0.8f, 0.8f, 0.5f);
    }

    public bool isFinished()
    {
        return finished;
    }
}
