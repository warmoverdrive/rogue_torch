using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCameraConfiner : MonoBehaviour
{
    void Start()
    {
        GetComponent<CinemachineConfiner>().m_BoundingShape2D = 
            FindObjectOfType<LevelGeneration>().gameObject.GetComponent<PolygonCollider2D>();
    }
}
