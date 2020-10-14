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
	private List<Vector2Int> deadEndGrids = new List<Vector2Int>();

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
		InitializeRoom(endingRoom.x, endingRoom.y, Room.Type.EndRoom);

		// generate dead ends randomly, checking spaces for start and end points before placing them
		// store dead ends in list
		Debug.Log("Generating Dead ends");
		GenerateDeadEnds();

		// find path to exit via empty tiles, store path in list
		// if no path, remove random dead end from list
		// try to find path again... repeat until path found

		// create rooms along path, checking surrounding rooms to not create unintentional dead ends

		// create path back from dead ends to start
		// change room type of path if needed to make it work

		// Fill remaining rooms by checking neighbors for entrances and walls
		Debug.Log("Filling remaining rooms");
		FillRemainingRooms();

		// Loop through array, generating all rooms
		Debug.Log("Generating Rooms in Game World...");
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
				Vector2Int deadEndPos = new Vector2Int(randX, randY);
				if ((deadEndPos += Vector2Int.left) != startingRoom &&
					(deadEndPos += Vector2Int.right) != endingRoom)
				{
					// check neighbors to see what kind of dead end to generate
					// initialize dead end
					InitializeRoom(randX, randY, CheckNeighbors(randX, randY, true));
					// add dead end to list
					deadEndGrids.Add(deadEndPos);
				}
			}
		}
	}

	private Room.Type CheckNeighbors(int currentX, int currentY, bool deadEnd)
	{
		bool up, down, left, right;
		List<Room.Type> acceptableTypes = new List<Room.Type>();

		// Check Up
		if (currentY + 1 < levelHeightInRooms) // check OOB (out of bounds)
		{
			Room checkedRoom = CheckRoom(currentX, currentY + 1);
			if (checkedRoom != null)
			{
				//check if the room type has a downward exit
				if (checkedRoom.roomType == Room.Type.D ||
					checkedRoom.roomType == Room.Type.LRD ||
					checkedRoom.roomType == Room.Type.LRUD ||
					checkedRoom.roomType == Room.Type.UD ||
					checkedRoom.roomType == Room.Type.LD ||
					checkedRoom.roomType == Room.Type.RD ||
					checkedRoom.roomType == Room.Type.LUD ||
					checkedRoom.roomType == Room.Type.RUD) up = true;
				else up = false;
			}
			else up = true;
		}
		else up = false;

		// Check Down
		if (currentY - 1 >= 0) // check OOB (out of bounds)
		{
			Room checkedRoom = CheckRoom(currentX, currentY - 1);
			if (checkedRoom != null)
			{
				//check if the room type has a upward exit
				if (checkedRoom.roomType == Room.Type.U ||
					checkedRoom.roomType == Room.Type.LRU ||
					checkedRoom.roomType == Room.Type.LRUD ||
					checkedRoom.roomType == Room.Type.UD ||
					checkedRoom.roomType == Room.Type.RU ||
					checkedRoom.roomType == Room.Type.LU ||
					checkedRoom.roomType == Room.Type.RUD ||
					checkedRoom.roomType == Room.Type.LUD) down = true;
				else down = false;
			}
			else down = true;
		}
		else down = false;

		// Check Right
		if (currentX + 1 < levelWidthInRooms) // check OOB (out of bounds)
		{
			Room checkedRoom = CheckRoom(currentX + 1, currentY);
			if (checkedRoom != null)
			{
				//check if the room type has a leftward exit
				if (checkedRoom.roomType == Room.Type.L ||
					checkedRoom.roomType == Room.Type.LRU ||
					checkedRoom.roomType == Room.Type.LRD ||
					checkedRoom.roomType == Room.Type.LRUD ||
					checkedRoom.roomType == Room.Type.LR ||
					checkedRoom.roomType == Room.Type.LUD ||
					checkedRoom.roomType == Room.Type.LU ||
					checkedRoom.roomType == Room.Type.LD ||
					checkedRoom.roomType == Room.Type.EndRoom) right = true;
				else right = false;
			}
			else right = true;
		}
		else right = false;

		// Check Left
		if (currentX - 1 >= 0) // check OOB (out of bounds)
		{
			Room checkedRoom = CheckRoom(currentX - 1, currentY);
			if (checkedRoom != null)
			{
				//check if the room type has a rightward exit
				if (checkedRoom.roomType == Room.Type.R ||
					checkedRoom.roomType == Room.Type.LRU ||
					checkedRoom.roomType == Room.Type.LRD ||
					checkedRoom.roomType == Room.Type.LRUD ||
					checkedRoom.roomType == Room.Type.LR ||
					checkedRoom.roomType == Room.Type.RUD ||
					checkedRoom.roomType == Room.Type.RU ||
					checkedRoom.roomType == Room.Type.RD ||
					checkedRoom.roomType == Room.Type.StartRoom) left = true;
				else left = false;
			}
			else left = true;
		}
		else left = false;

		if(deadEnd)
		{
			// if dead end, then pick from appropriate room types at random
			if (up) acceptableTypes.Add(Room.Type.U);
			if (down) acceptableTypes.Add(Room.Type.D);
			if (right) acceptableTypes.Add(Room.Type.R);
			if (left) acceptableTypes.Add(Room.Type.L);

			if (acceptableTypes.Count > 0)
			{
				return acceptableTypes[Random.Range(0, acceptableTypes.Count)];
			}
			// if all false, pick blank room
			else return Room.Type.Blank;
		}
		else
		{
			// select and return appropriate Room.Type
			return SelectRoomType(up, down, left, right);
		}

	}

	private Room.Type SelectRoomType(bool up, bool down, bool left, bool right)
	{
		if (!up && !down && left && right)
			return Room.Type.LR;
		else if (up && down && !left && !right)
			return Room.Type.UD;
		else if (up && down && left && right)
			return Room.Type.LRUD;
		else if (up && !down && left && right)
			return Room.Type.LRU;
		else if (!up && down && left && right)
			return Room.Type.LRD;
		else if (up && down && left && !right)
			return Room.Type.LUD;
		else if (up && down && !left && right)
			return Room.Type.RUD;
		else if (!up && down && left && !right)
			return Room.Type.LD;
		else if (up && !down && left && !right)
			return Room.Type.LU;
		else if (up && !down && !left && right)
			return Room.Type.RU;
		else if (!up && down && !left && right)
			return Room.Type.RD;
		else if (!up && !down && left && !right)
			return Room.Type.L;
		else if (!up && !down && !left && right)
			return  Room.Type.R;
		else if (up && !down && !left && !right)
			return Room.Type.U;
		else if (!up && down && !left && !right)
			return Room.Type.D;
		else return Room.Type.Blank;
	}

	private void FillRemainingRooms()
	{
		// iterate through entire grid
		for (int y = 0; y < levelHeightInRooms; y++)
		{
			for (int x = 0; x < levelWidthInRooms; x++)
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
		for (int y = 0; y < levelHeightInRooms; y++)
		{
			for (int x = 0; x < levelWidthInRooms; x++)
			{
				if (CheckRoom(x, y))
				{
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
