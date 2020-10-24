using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : Singleton<MapGenerator>
{
    public GameObject circlePrefab;
    public GameObject sliderPrefab;
    // todo - temporary
    public AudioClip beatMapSong;
    [Min(1f)] // don't want infinite loops
    public int bpm;
    public float beatOffset;
    // map difficulty
    public float songDelay = 2f; // time it takes before song starts
    public float approachTime = 1f; // time in seconds before approachCircle hits radius
    public float hitObjectScale = 1f; // how big are the circles
    public float approachScale = 2f; // how much bigger is the approach circle compared to hitcircle
    [Tooltip("How close can a circle be to the edges of the screen?")]
    public float screenMargin = 0.1f;

    private Transform hitObjects;
    private List<HitObjectData> mapSpawns = new List<HitObjectData>();
    // get min/max in world points
    private float yMin, yMax, xMin, xMax;
    // circles should be created in an increasingly deeper level
    private float depth = 0;
    // time between beats
    private float beatTime;

    void Awake()
    {
        hitObjects = new GameObject("HitObjects").transform;

        float circleSize = circlePrefab.GetComponent<CircleCollider2D>().radius * hitObjectScale;
        float cameraVert = Camera.main.orthographicSize;
        yMax = cameraVert - circleSize - screenMargin;
        xMax = (cameraVert * (Screen.width / Screen.height)) - circleSize - screenMargin;
        xMin = -xMax;
        yMin = -yMax;

        beatTime = 60.0f / bpm;

        // notify user of bounds
        Debug.Log("xMin: " + xMin + ", xMax: " + xMax + ", yMin: " + yMin + ", yMax: " + yMax);
    }

    void Start()
    {
        generateMap();
        StartCoroutine(playSong());
        StartCoroutine(layoutMap());
    }

    private IEnumerator playSong()
    {
        // start playing the beatmap
        yield return new WaitForSeconds(songDelay);
        SoundManager.Instance.PlaySong(beatMapSong);
    }

    private IEnumerator layoutMap()
    {

        float initialOffset = songDelay - approachTime;
        float prevTime = 0f;
        if (initialOffset < 0f)
        {
            // todo - set waitTime to 0 and cut down approach rate by 'waitTime'
            Debug.Log("error, approachTime is larger than songDelay!");
        }
        yield return new WaitForSeconds(initialOffset);
        foreach (HitObjectData hitObj in mapSpawns)
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
        // timestamp (progression) is made such that the smallest approach circle aligns with the beat
        // in this way, the timestamp is independent of the approach rate

        // todo - replace this with a neural network that takes song as input and outputs/fills up mapSpawns
        float songLength = beatMapSong.length;
        float progression = beatOffset;
        while (progression < songLength)
        {
            HitObjectData hitObject;
            
            // switch between singlecircles and sliders
            if (Random.Range(0, 2) == 0) // SLIDER
            {
                // between 2 and 5 positions
                int positionCount = Random.Range(2, 6);
                // max 2 coordinates away in x- and y-dir
                float maxSpace = 3;
                // duration between 2 and 5 beats?
                int duration = Random.Range(2, 6);
                // 1 repeat (for now)
                int repeats = 1;
                Vector3[] positions = new Vector3[positionCount];
                for (int i = 0; i < positionCount; i++)
                {
                    float randomX, randomY;
                    if (i > 0) // limit position to be within 'maxspace' of previous point
                    {
                        Vector3 prevPoint = positions[i - 1];
                        randomX = Random.Range(prevPoint.x - maxSpace, prevPoint.x + maxSpace);
                        randomY = Random.Range(prevPoint.y - maxSpace, prevPoint.y + maxSpace);
                        // clamp to screen min/max
                        randomX = Math.Max(randomX, xMin);
                        randomX = Math.Min(randomX, xMax);
                        randomY = Math.Max(randomY, yMin);
                        randomY = Math.Min(randomY, yMax);
                    } else
                    {
                        randomX = Random.Range(xMin, xMax);
                        randomY = Random.Range(yMin, yMax);
                    }
                    
                    positions[i] = new Vector3(randomX, randomY, depth);
                }

                hitObject = new HitObjectData(positions, progression, duration, repeats);
                progression += beatTime * (duration + 1);
            } else
            {
                // make circle
                float randomX = Random.Range(xMin, xMax);
                float randomY = Random.Range(yMin, yMax);
                hitObject = new HitObjectData(new Vector3(randomX, randomY, depth), progression);
                progression += beatTime;
            }
            mapSpawns.Add(hitObject);
            depth += 0.01f; // increase depth slightly to make older circles be on top
        }
    }

    private void placeObject(HitObjectData hitObj)
    {
        GameObject instance;
        if (hitObj.type == HitObjectData.HitType.Circle)
        {
            instance = Instantiate(circlePrefab, hitObj.position, Quaternion.identity);
        }
        else
        {
            instance = Instantiate(sliderPrefab, hitObj.positions[0], Quaternion.identity);
            Slider sliderScript = instance.GetComponent<Slider>();
            sliderScript.setup(hitObj);
        }
        instance.transform.SetParent(hitObjects);
    }


    /*
    private void prepareBeatMapSpawns()
    {
        // sort the bestmapspawns based on the time attribute
        mapSpawns.Sort();
        // clamp the x and y values and inform user of any values outside range
        foreach (HitObjectData hitObject in mapSpawns)
        {
            Vector3 pos = hitObject.position;
            if (pos.x < xMin)
            {
                Debug.Log("x value of beatmap spawn below xMin detected. Value: " + pos.x + ". Clamping to xMin.");
                pos.x = xMin;
            }
            else if (pos.x > xMax)
            {
                Debug.Log("x value of beatmap spawn above xMax detected. Value: " + pos.x + ". Clamping to xMax.");
                pos.x = xMax;
            }
            else if (pos.y < yMin)
            {
                Debug.Log("y value of beatmap spawn below yMin detected. Value: " + pos.y + ". Clamping to yMin.");
                pos.y = yMin;
            }
            else if (pos.y > yMax)
            {
                Debug.Log("y value of beatmap spawn above yMax detected. Value: " + pos.y + ". Clamping to yMax.");
                pos.y = yMax;
            }
        }
    }
    */


}
