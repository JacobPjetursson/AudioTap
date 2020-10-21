using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Text comboText;
    public GameObject missedSprite;
    public GameObject hitSprite;
    public AudioClip hitClip;
    public AudioClip missedClip;

    private int combo = 0;
    private GameObject effects;

    void Awake()
    {
        effects = new GameObject("Effects");
    }


    void Update()
    {
        
    }

    public void hit(Vector3 pos)
    {
        // play hit sound
        SoundManager.Instance.PlayEffect(hitClip);

        combo++;
        comboText.text = "Combo: " + combo;

        if (hitSprite != null) // allowed to not have a hitsprite
        {
            // instantiate hit sprite
            Instantiate(hitSprite, pos, Quaternion.identity, effects.transform);
        }
    }

    public void missed(Vector3 pos)
    {
        if (combo >= 5) // only play missed if combo high enough
            SoundManager.Instance.PlayEffect(missedClip);

        combo = 0;
        comboText.text = "Combo: " + combo;
        // instantiate missed sprite
        Instantiate(missedSprite, pos, Quaternion.identity, effects.transform);
    }
    
}
