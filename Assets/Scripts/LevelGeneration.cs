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
    [SerializeField]
    int numberOfDeadEnds = 5;

    private Room[,] castleMap;
    private Transform parentObject;
    private int startingRoomY;
    private int endingRoomY;

    private void Awake()
	{
		parentObject = transform.GetComponentInParent<Transform>();

		// generate a virtual grid of the castle
		castleMap = new Room[levelWidthInRooms, levelHeightInRooms];

		// determine starting point on left side castleMap[0, rand]
		startingRoomY = Random.Range(0, levelHeightInRooms);
		InitializeRoom(0, startingRoomY, Room.Type.StartRoom);

		// determine end point on right side castleMap[levelWidthInRooms - 1, rand]
		endingRoomY = Random.Range(0, levelHeightInRooms);
		InitializeRoom(levelWidthInRooms - 1, endingRoomY, Room.Type.L);

		// generate dead ends randomly, checking spaces for start and end points before placing them
		// store dead ends in list

		// find path to exit, store path in list
		// if no path, remove random dead end from list
		// try to find path again... repeat until path found

		// create rooms along path, checking surrounding rooms to not create unintentional dead ends

		// Fill remaining rooms somehow...?
		FillRemainingRooms();

		// Loop through array, generating all rooms
		GenerateAllRooms();

		/*
				//      *** Initializing and creating rooms ***    //

				// Generate room object in the grid and initialize its type
				InitializeRoom(0, 0, Room.Type.LR);

				// If type change is needed:
				ChangeRoomType(0, 0, Room.Type.StartRoom);

				// Actually generates the room in the game world
				GenerateRoom(0, 0);
		*/
	}
	private void FillRemainingRooms()
	{
		for (int x = 0; x < levelWidthInRooms; x++)
		{
			for (int y = 0; y < levelHeightInRooms; y++)
			{
				if (!CheckRoom(x, y))
				{
					InitializeRoom(x, y, roomArchetype.GetRandomType());
				}
			}
		}
	}

	private void GenerateAllRooms()
	{
		for (int x = 0; x < levelWidthInRooms; x++)
		{
			for (int y = 0; y < levelHeightInRooms; y++)
			{
				if (CheckRoom(x, y))
				{
					Debug.Log("Generating " + CheckRoom(x, y).roomType + "at " + x + " " + y);
					GenerateRoom(x, y);
				}
			}
		}
	}


	private void InitializeRoom(int gridX, int gridY, Room.Type roomType)
	{
        castleMap[gridX, gridY] = new GameObject().AddComponent<Room>();
        castleMap[gridX, gridY].roomPalette = roomArchetype.roomPalette;
        castleMap[gridX, gridY].roomType = roomType;
        Debug.Log("Initialized "+castleMap[gridX, gridY].roomType);
	}

    private void ChangeRoomType(int gridX, int gridY, Room.Type roomType) { castleMap[gridX, gridY].roomType = roomType; }

    private void GenerateRoom(int gridX, int gridY)
    {
        castleMap[gridX, gridY].CreateRoom(gridX * roomWidth, gridY * roomHeight, parentObject);
    }

    private Room CheckRoom(int gridX, int gridY)
    {
        if (castleMap[gridX, gridY]) return castleMap[gridX, gridY];
        else return null;
    }
}
