using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    Transform parentGrid;
    [SerializeField]
    int roomWidth = 32, roomHeight = 18;
    [SerializeField]
    GameObject[] rooms;

    private int direction;
    float timeBetweenRoom;
    public float startTimeBetweenRoom = 0.25f;

    void Start()
    {
        if (!parentGrid) Debug.LogError("No Parent Grid Assigned to Generator!!!");

        Instantiate(rooms[0], transform.position, Quaternion.identity, parentGrid);

        direction = Random.Range(0, 6);
    }

    private void Update()
    {
        if (timeBetweenRoom <= 0)
        {
            Move();
            timeBetweenRoom = startTimeBetweenRoom;
        }
        else
        {
            timeBetweenRoom -= Time.deltaTime;
        }
    }

    private void Move()
    {
        if (direction == 0 || direction == 1)           // MOVE RIGHT
        {
            Vector2 newPos = new Vector2(transform.position.x + roomWidth, transform.position.y);
            transform.position = newPos;
        }
        else if (direction == 2 || direction == 3)      // MOVE LEFT
        {
            Vector2 newPos = new Vector2(transform.position.x - roomWidth, transform.position.y);
            transform.position = newPos;
        }
        else if (direction == 4)                        // MOVE UP
        {
            Vector2 newPos = new Vector2(transform.position.x, transform.position.y + roomHeight);
            transform.position = newPos;
        }
        else if (direction == 5)                        // MOVE DOWN
        {
            Vector2 newPos = new Vector2(transform.position.x, transform.position.y - roomHeight);
            transform.position = newPos;
        }

        Instantiate(rooms[0], transform.position, Quaternion.identity, parentGrid);
        direction = Random.Range(0, 6);
    }
}
