using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitObject : MonoBehaviour
{
    public GameObject approachPrefab;

    protected float approachScale; // how much bigger is the approach circle?
    protected float approachRate; // rate per second
    protected float approachTime; // time in seconds before approachCircle hits radius
    protected float approachStep; // how much relatively closer the object has become
    protected bool approaching = true;
    protected float opacity = 0f; // circles start with being transparent
    protected bool hit = false; // has the circle/slider been hit?
    protected bool missed = false; // also counts for sliderbreak, such that hit and miss can both be true
    protected GameObject approachCircle;

    [Min(0f)]
    public float missedBuffer = 0.15f; // time after approach before miss is detected

    protected virtual void Start()
    {
        // set scale
        float scale = MapGenerator.Instance.hitObjectScale;
        transform.localScale = new Vector3(scale, scale, 0f);
        // get approach rate
        approachTime = MapGenerator.Instance.approachTime;
        approachRate = (1.0f / approachTime);

        // instantiate approach circle
        approachScale = MapGenerator.Instance.approachScale;
        approachCircle = Instantiate(approachPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        approachCircle.transform.localScale = new Vector3(approachScale, approachScale, 0f);
        // set transparent
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
        approachCircle.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);

        // approach timer (time before miss occurs)
        StartCoroutine(Approach());
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (approaching)
        {
            approachStep = approachRate * Time.fixedDeltaTime;
            // make circle less opaque as it grows closer
            opacity += approachStep;

            // we want the approach circle to shrink by a constant factor based on frame rate
            // it should approach the periphery of the hit circle
            float shrinkAmount = approachStep * (approachScale - 1.0f);
            Vector3 shrink = new Vector3(shrinkAmount, shrinkAmount, 0f);
            approachCircle.transform.localScale -= shrink;
            // set opacity
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
            approachCircle.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
        }
    }

    private IEnumerator Approach()
    {
        yield return new WaitForSeconds(approachTime);
        // stop approach circle from shrinking
        approaching = false;
        yield return new WaitForSeconds(missedBuffer);
        if (!hit)
            Miss();
    }

    public virtual void Hit()
    {
        hit = true;
        GameManager.Instance.hit(transform.position);
    }

    public virtual void Miss()
    {
        missed = true;
        GameManager.Instance.missed(transform.position);
    }

    public bool isMissed()
    {
        return missed;
    }

    public bool isHit()
    {
        return hit;
    }


}
