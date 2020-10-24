using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    private Transform slider;
    private bool sliding = false;

    void FixedUpdate()
    {
        if (pressKey()) {
            // cast 2d ray
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            
            if (hit.collider != null)
            {
                Transform transform = hit.transform;
                HitObject hitObj = transform.GetComponent<HitObject>();
                if (hitObj.isMissed() || hitObj.isHit())
                    return;

                if (transform.tag == "Slider")
                {
                    // start sliding
                    sliding = true;
                    slider = transform;
                }
                transform.gameObject.GetComponent<HitObject>().Hit();

            }
        }
        if (sliding)
        {
            if (slider == null || slider.GetComponent<Slider>().isFinished())
            {
                // slider was destroyed elsewhere, meaning it reached end of duration
                sliding = false;
            }
            else if (holdKey())
            {
                // ensure we are still within bounds
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit.collider == null || hit.transform != slider)
                {
                    missedSlider();
                }
            }
            else
            {
                // not holding key means that you missed
                missedSlider();
            }
        }
        
    }

    private void missedSlider()
    {
        slider.GetComponent<Slider>().Miss();
        sliding = false;
    }

    private bool pressKey()
    {
        return Input.GetKeyDown("v") || Input.GetKeyDown("c") || Input.GetMouseButtonDown(0);
    }

    private bool holdKey()
    {
        return Input.GetKey("v") || Input.GetKey("c") || Input.GetMouseButton(0);
    }

}
