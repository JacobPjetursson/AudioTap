using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGeneratorScript : Singleton<MapGeneratorScript>
{
    public GameObject circlePrefab;
    public GameObject sliderPrefab;
    // todo - temporary
    public AudioClip beatMapSong;
    [Min(1f)] // don't want infinite loops
    public int bpm;
    public float beatOffset;
    // map difficulty
    public float initialDelay = 2f; // time it takes before map starts
    public float approachTime = 1f; // time in seconds before approachCircle hits radius
    public float circleScale = 1f; // how big is the circles
    public float approachScale = 2f; // how much bigger is the approach circle compared to hitcircle
    [Tooltip("How close can a circle be to the edges of the screen?")]
    public float screenMargin = 0.1f;

    private Transform hitObjects;
    private List<HitObject> mapSpawns = new List<HitObject>();
    // get min/max in world points
    private float yMin, yMax, xMin, xMax;
    // circles should be created in an increasingly deeper level
    private float depth = 0;
    // time between beats
    private float beatTime;

    void Awake()
    {
        hitObjects = new GameObject("HitObjects").transform;
        float circleSize = circlePrefab.GetComponent<CircleCollider2D>().radius * circleScale;

        float cameraVert = Camera.main.orthographicSize;
        yMax = cameraVert - circleSize - screenMargin;
        xMax = (cameraVert * (Screen.width / Screen.height)) - circleSize - screenMargin;
        xMin = -xMax;
        yMin = -yMax;

        beatTime = 60.0f / bpm;

        // notify user of bounds
        Debug.Log("xMin: " + xMin + ", xMax: " + xMax + ", yMin: " + yMin + ", yMax: " + yMax);
    }

    IEnumerator Start()
    {
        // wait the initial delay
        yield return new WaitForSeconds(initialDelay);
        //prepareBeatMapSpawns();
        generateMap();
        // start playing the beatmap
        SoundManager.Instance.PlaySong(beatMapSong);
        StartCoroutine(layoutMap());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator layoutMap()
    {
        float prevTime = 0f;
        foreach (HitObject hitObj in mapSpawns)
        {
            float waitTime = hitObj.time - prevTime;
            yield return new WaitForSeconds(waitTime);
            placeObject(hitObj);
            prevTime = hitObj.time;
        }

        // close down application when beatmap is finished
        // todo - temporary
        Application.Quit(0);
    }

    private void generateMap()
    {
        // todo - replace this with a neural network that takes song as input and outputs/fills up mapSpawns
        float songLength = beatMapSong.length;
        float progression = beatOffset;
        while (progression < songLength)
        {
            float randomX = Random.Range(xMin, xMax);
            float randomY = Random.Range(yMin, yMax);
            HitObject hitObject = new HitObject(randomX, randomY, progression);
            mapSpawns.Add(hitObject);
            progression += beatTime;
        }
    }

    private void placeObject(HitObject hitObj)
    {
        GameObject instance;
        if (hitObj.type == HitObject.HitType.Circle)
        {
            instance = Instantiate(circlePrefab, new Vector3(hitObj.x, hitObj.y, depth), Quaternion.identity);
        }
        else
        {
            float first_x = hitObj.x_values[0];
            float first_y = hitObj.y_values[1];
            instance = Instantiate(sliderPrefab, new Vector3(first_x, first_y, depth), Quaternion.identity);
            Slider sliderScript = instance.GetComponent<Slider>();
            sliderScript.setup(hitObj);
        }
        instance.transform.SetParent(hitObjects);
        depth += 0.01f; // increase depth slightly to make older circles be on top
    }
 
    private void prepareBeatMapSpawns()
    {
        // sort the bestmapspawns based on the time attribute
        mapSpawns.Sort();
        // clamp the x and y values and inform user of any values outside range
        foreach (HitObject hitObject in mapSpawns)
        {
            if (hitObject.x < xMin)
            {
                Debug.Log("x value of beatmap spawn below xMin detected. Value: " + hitObject.x + ". Clamping to xMin.");
                hitObject.x = xMin;
            }
            else if (hitObject.x > xMax)
            {
                Debug.Log("x value of beatmap spawn above xMax detected. Value: " + hitObject.x + ". Clamping to xMax.");
                hitObject.x = xMax;
            }
            else if (hitObject.y < yMin)
            {
                Debug.Log("y value of beatmap spawn below yMin detected. Value: " + hitObject.y + ". Clamping to yMin.");
                hitObject.y = yMin;
            }
            else if (hitObject.y > yMax)
            {
                Debug.Log("y value of beatmap spawn above yMax detected. Value: " + hitObject.y + ". Clamping to yMax.");
                hitObject.y = yMax;
            }
        }
    }


}
