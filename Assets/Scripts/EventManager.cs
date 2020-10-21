using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{

    void Start()
    {
        
    }


    void Update()
    {
        if (Input.GetKeyDown("v") || Input.GetKeyDown("c") || Input.GetMouseButtonDown(0)) {
            // cast 2d ray
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                Transform transform = hit.transform;
                if (transform.tag == "Circle")
                {
                    GameManager.Instance.hit(transform.position);
                    Destroy(transform.gameObject);
                }
            }
        }
        
    }

}
