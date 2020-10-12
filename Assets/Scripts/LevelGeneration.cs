using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    [Header("Level Parameters")]
    [SerializeField]
    Room roomArchetype;
    [SerializeField]
    int roomWidth = 32,
        roomHeight = 18,
        levelWidthInRooms = 8,
        levelHeightInRooms = 5;

    private Room[,] castleMap;
    private Transform parentObject;

    private void Awake()
    {
        parentObject = transform.GetComponentInParent<Transform>();

        // generate a virtual grid of the castle
        castleMap = new Room[levelWidthInRooms, levelHeightInRooms];

        //Test for initializing and creating rooms
        GenerateRoom(0, 0, Room.Type.StartRoom);

        // determine starting point on left side castleMap[0, rand]

        // determine end point on right side
    }

    private void GenerateRoom(int gridX, int gridY, Room.Type roomType)
    {
        castleMap[gridX, gridY] = roomArchetype;
        castleMap[gridX, gridY].roomType = roomType;
        castleMap[gridX, gridY].CreateRoom(gridX * roomWidth, gridY * roomHeight, parentObject);
    }
}
