using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    [Min(0f)]
    public float selfDestructBuffer = 0.15f; // time after approach before self destruction

    public GameObject approachPrefab;

    private GameObject approachCircle;
    private float approachScale;
    private float approachRate; // rate per second
    private float approachTime; // time in seconds before approachCircle hits radius
    private bool approaching = true;

    private float opacity = 0f; // circles start with being transparent

    void Start()
    {
        // set scale of this circle
        float circleScale = MapGeneratorScript.Instance.circleScale;
        transform.localScale = new Vector3(circleScale, circleScale, 0f);
        // get approach rate
        approachTime = MapGeneratorScript.Instance.approachTime;
        approachRate = (1.0f / approachTime);
        // approach timer
        StartCoroutine(Approach());
        // instantiate approach circle
        approachScale = MapGeneratorScript.Instance.approachScale;
        approachCircle = Instantiate(approachPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        approachCircle.transform.localScale = new Vector3(approachScale, approachScale, 0f);

        // set transparent
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
        approachCircle.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
    }

    void Update()
    {
        if (approaching)
        {
            // we want the approach circle to shrink by a constant factor based on frame rate
            // it should approach the periphery of the hit circle
            float step = approachRate * Time.deltaTime;
            float shrinkAmount = step * (approachScale - 1.0f);
            Vector3 shrink = new Vector3(shrinkAmount, shrinkAmount, 0f);
            approachCircle.transform.localScale -= shrink;
            // make circle less opaque as it grows closer
            opacity += step;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
            approachCircle.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
        }
    }

    private IEnumerator Approach()
    {
        while (true)
        {
            yield return new WaitForSeconds(approachTime);
            // stop approach circle from shrinking
            approaching = false;
            // wait for buffer time and self-destruct
            yield return new WaitForSeconds(selfDestructBuffer);
            GameManager.Instance.missed(gameObject.transform.position);
            Destroy(gameObject);
        }
    }
}
