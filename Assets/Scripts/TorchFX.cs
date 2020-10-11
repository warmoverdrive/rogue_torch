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
    float maxRandomLightIntensity = 1f;
    [SerializeField]
    [Range(0, 50)]
    int flickerSmoothing = 10;

    [SerializeField]
    Light2D thisLight;

    Queue<float> smoothingQueue;
    float lastSum = 0;


    // Start is called before the first frame update
    void Start()
    {
        smoothingQueue = new Queue<float>(flickerSmoothing);
        
        thisLight = GetComponent<Light2D>();
    }

    // Update is called once per frame
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
    }
}
