using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCircle : HitObject
{

    public override void Hit()
    {
        base.Hit();
        Destroy(gameObject);
    }

    public override void Miss()
    {
        base.Miss();
        Destroy(gameObject);
    }
}
