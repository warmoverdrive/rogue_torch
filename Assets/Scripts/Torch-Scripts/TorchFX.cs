using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TorchFX : MonoBehaviour
{
    [Header("Config Parameters")]
    [SerializeField]
    float minRandomLightIntensity = 0f;
    [SerializeField]
    public float maxRandomLightIntensity = 1f;
    [SerializeField]
    Color[] randomColors;
    [SerializeField]
    [Tooltip("1 in X chance per frame to change color.")]
    int colorChangeChance = 25;
    [SerializeField]
    float colorChangeSpeed = 5f;
    [SerializeField]
    [Range(0, 50)]
    int flickerSmoothing = 10;

    Light2D thisLight;
    Color startingColor;
    Color nextColor;
    bool changingColor = false;
    float startTime;
    Queue<float> smoothingQueue;
    float lastSum = 0;

    void Start()
    {
        smoothingQueue = new Queue<float>(flickerSmoothing);
        
        thisLight = GetComponent<Light2D>();

        thisLight.color = randomColors[Random.Range(0, randomColors.Length - 1)];
    }

    void Update()
    {
        if (!thisLight) return;

        //remove an item if the queue is bigger than smoothing value
        while (smoothingQueue.Count >= flickerSmoothing)
        {
            lastSum -= smoothingQueue.Dequeue();
        }

        //generate new random item, calculate new average
        float newVal = Random.Range(minRandomLightIntensity, maxRandomLightIntensity);
        smoothingQueue.Enqueue(newVal);
        lastSum += newVal;

        //calculate new smoothed average
        thisLight.intensity = lastSum / smoothingQueue.Count;

        // % chance to set new random color
        if (Random.Range(0, colorChangeChance) == 1 && !changingColor)
        {
            nextColor = randomColors[Random.Range(0, randomColors.Length-1)];
            startingColor = thisLight.color;
            changingColor = true;
            startTime = Time.time;
        }

        if (changingColor)
        {
            thisLight.color = Color.Lerp(startingColor, nextColor, (Time.time - startTime) * colorChangeSpeed);
            if (thisLight.color == nextColor) changingColor = false;
        }
    }
}
