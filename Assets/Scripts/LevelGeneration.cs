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

	//storage Vector2Ints of key points for pathfinding
	private Vector2Int startingRoom;
	private Vector2Int endingRoom;
	private List<Vector2Int> deadEndGrids;

    private void Awake()
	{
		parentObject = transform.GetComponentInParent<Transform>();

		// generate a virtual grid of the castle
		castleMap = new Room[levelWidthInRooms, levelHeightInRooms];

		// determine starting point on left side castleMap[0, rand]
		startingRoom = new Vector2Int(0, Random.Range(0, levelHeightInRooms));
		InitializeRoom(startingRoom.x, startingRoom.y, Room.Type.StartRoom);

		// determine end point on right side castleMap[levelWidthInRooms - 1, rand]
		endingRoom = new Vector2Int(levelWidthInRooms - 1, Random.Range(0, levelHeightInRooms));
		InitializeRoom(endingRoom.x, endingRoom.y, Room.Type.L);

		// generate dead ends randomly, checking spaces for start and end points before placing them
		// store dead ends in list
		GenerateDeadEnds();

		// find path to exit via empty tiles, store path in list
		// if no path, remove random dead end from list
		// try to find path again... repeat until path found

		// create rooms along path, checking surrounding rooms to not create unintentional dead ends

		// create path back from dead ends to start
		// change room type of path if needed to make it work

		// Fill remaining rooms by checking neighbors for entrances and walls
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

	private void GenerateDeadEnds()
	{
		for (int deadEndsGenerated = 0; deadEndsGenerated < numberOfDeadEnds; deadEndsGenerated++)
		{
			int randX = Random.Range(0, levelWidthInRooms - 1);
			int randY = Random.Range(0, levelHeightInRooms - 1);

			if(CheckRoom(randX, randY) == null)
			{
				// check neighbors to see what kind of dead end to generate
				// initialize dead end
				// add dead end to list
				deadEndGrids.Add(new Vector2Int(randX, randY));
			}
		}
	}

	private Room.Type CheckNeighbors(int currentX, int currentY, bool deadEnd)
	{
		bool up, down, left, right;
		// Check +1/-1 in all directions and get room type
		// for each, if they have an entrance in that direction, then Direction = true
		// null also equals true, unless out of bounds
		// if theres a wall in place, then direction is false
		// if dead end, then pick from appropriate room tpyes at random
		// if all false, pick blank room
		// return appropriate Room.Type

		return Room.Type.LRUD; // placeholder
	}

	private void FillRemainingRooms()
	{
		// iterate through entire grid
		for (int x = 0; x < levelWidthInRooms; x++)
		{
			for (int y = 0; y < levelHeightInRooms; y++)
			{
				if (!CheckRoom(x, y))
				{
					// Check neighbors for room type and initialize
					InitializeRoom(x, y, CheckNeighbors(x, y, false));
				}
			}
		}
	}

	private void GenerateAllRooms()
	{
		// iterate through grid instantiating all rooms
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
		// check for a room at gridX, gridY
		// if it exists, return the Room with all associated data
		// if it doesnt, return null.
        if (castleMap[gridX, gridY]) return castleMap[gridX, gridY];
        else return null;
    }
}
