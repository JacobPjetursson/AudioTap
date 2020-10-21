using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEffect : MonoBehaviour
{

    public float destructTimer = 0.3f;
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(destructTimer);
        Destroy(gameObject);
    }
}