using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    Transform parentGrid;
    [SerializeField]
    int roomWidth = 32, 
        roomHeight = 18, 
        minX = 0, 
        maxX = 256, 
        minY = 0, 
        maxY = 72;
    [SerializeField]
    LayerMask roomLayer;
    [SerializeField]
    GameObject[] rooms; // [0] -> LR, [1] -> LRD, [2] -> LRU, [3] -> LRUD

    private int direction;
    float timeBetweenRoom;
    public float startTimeBetweenRoom = 0.25f;

    private bool stopGeneration = false;

    void Start()
    {
        if (!parentGrid) Debug.LogError("No Parent Grid Assigned to Generator!!!");

        Instantiate(rooms[0], transform.position, Quaternion.identity, parentGrid);

        direction = Random.Range(0, 6);
        Debug.Log(direction);
    }

    private void Update()
    {
        if (timeBetweenRoom <= 0 && !stopGeneration)
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
            if (transform.position.x < maxX)
            {
                Vector2 newPos = new Vector2(transform.position.x + roomWidth, transform.position.y);
                transform.position = newPos;

                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity, parentGrid);

                direction = Random.Range(0, 6); // Set direction to never be left
                if (direction == 2) direction = 1;
                else if (direction == 3) direction = 5;
            }
            else direction = 5;
        }
        else if (direction == 2 || direction == 3)      // MOVE LEFT
        {
            if (transform.position.x > minX)
            {
                Vector2 newPos = new Vector2(transform.position.x - roomWidth, transform.position.y);
                transform.position = newPos;

                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity, parentGrid);

                direction = Random.Range(2, 6); // set direction to never be right
            }
            else direction = 5;
        }
        else if (direction == 4)                        // MOVE DOWN
        {
            if (transform.position.y > minY)
            {
                //Check if current room has a bottom opening
                // if not, destroy room and replace with one that does
                Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, roomLayer);
                Debug.Log(roomDetection);
                if (roomDetection)
                {
                    if (roomDetection.GetComponent<RoomType>().roomType == RoomType.Type.LR ||
                        roomDetection.GetComponent<RoomType>().roomType == RoomType.Type.LRU &&
                        roomDetection != null)
                    {
                        roomDetection.GetComponent<RoomType>().DestroyRoom();

                        int randBottomRoom = Random.Range(1, 4);
                        if (randBottomRoom == 2) randBottomRoom = 3;
                        Instantiate(rooms[randBottomRoom], transform.position, Quaternion.identity, parentGrid);
                    }
                }

                Vector2 newPos = new Vector2(transform.position.x, transform.position.y - roomHeight);
                transform.position = newPos;

                int rand = Random.Range(2, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity, parentGrid);

                direction = Random.Range(0, 4); // set to only go left or right
            }
            else direction = 5;
        }
        else if (direction == 5)                        // MOVE UP
        {
            if(transform.position.y < maxY)
            {
                //Check if current room has a top opening
                // if not, destroy room and replace with one that does
                Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, roomLayer);
                Debug.Log(roomDetection);
                if(roomDetection)
                {
                    if (roomDetection.GetComponent<RoomType>().roomType == RoomType.Type.LR ||
                        roomDetection.GetComponent<RoomType>().roomType == RoomType.Type.LRD && 
                        roomDetection != null)
                    {
                        Debug.Log("Destroying Room");
                        roomDetection.GetComponent<RoomType>().DestroyRoom();

                        int randTopRoom = Random.Range(2, 4);
                        Debug.Log("Random Top Room is " + randTopRoom);
                        Instantiate(rooms[randTopRoom], transform.position, Quaternion.identity, parentGrid);
                    }
                }

                Vector2 newPos = new Vector2(transform.position.x, transform.position.y + roomHeight);
                transform.position = newPos;

                int rand = Random.Range(1, rooms.Length);
                if (rand == 3) rand = 1;
                Instantiate(rooms[rand], transform.position, Quaternion.identity, parentGrid);

                direction = Random.Range(0, 6);
                if (direction == 4) direction = 5;
            }
            else
            {
                //stop generation, make exit
                stopGeneration = true;
            }
        }

        Debug.Log(direction);
    }
}
