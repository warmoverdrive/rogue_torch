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

	private PolygonCollider2D polyCollider;

    private Room[,] castleMap;
    private Transform parentObject;

	private List<Vector2Int> path;

	//storage Vector2Ints of key points for pathfinding
	private Vector2Int startingRoom;
	private Vector2Int endingRoom;
	private List<Vector2Int> deadEndGrids = new List<Vector2Int>();

    private void Awake()
	{
		// Set points for the polygon collider for Camera stuff
		polyCollider = GetComponent<PolygonCollider2D>();
		SetPoligonColliderBounds();

		// Set the parent object to the Tilemap Grid for all of the created rooms
		parentObject = transform.GetComponentInParent<Transform>();

		// generate a virtual grid of the castle
		castleMap = new Room[levelWidthInRooms, levelHeightInRooms];

		// determine starting point on left side castleMap[0, rand]
		startingRoom = new Vector2Int(0, Random.Range(0, levelHeightInRooms));
		InitializeRoom(startingRoom.x, startingRoom.y, Room.Type.StartRoom);

		// determine end point on right side castleMap[levelWidthInRooms - 1, rand]
		endingRoom = new Vector2Int(levelWidthInRooms - 1, Random.Range(0, levelHeightInRooms));
		InitializeRoom(endingRoom.x, endingRoom.y, Room.Type.EndRoom);

		Debug.Log("Start room: " + startingRoom);
		Debug.Log("Ending room: " + endingRoom);
		// generate dead ends randomly, checking spaces for start and end points before placing them
		// store dead ends in list
		GenerateDeadEndsList();
		foreach (var deadEnd in deadEndGrids)
		{
			Debug.Log("Dead end: " + deadEnd);
		}


		//Construct Pathfinder object (local scope to destroy after completion of Awake())
		Pathfinder pathfinder = new Pathfinder(castleMap, startingRoom, endingRoom, deadEndGrids);
		path = pathfinder.CalculatePath();

		// build path
		// first loop is to destroy all dead ends blocking the path, 
		// and the second one resets the rooms to have a valid room type
		Debug.Log("Path:");
		foreach (var point in path)
		{
			Debug.Log(point);
			InitializeRoom(point.x, point.y, Room.Type.LRUD);
		}
		foreach (var point in path)
		{
			InitializeRoom(point.x, point.y, CheckNeighbors(point.x, point.y, false));
		}

		// Fill remaining rooms by checking neighbors for entrances and walls
		Debug.Log("Filling remaining rooms");
		FillRemainingRooms();

		// Loop through array, generating all rooms
		Debug.Log("Generating Rooms in Game World...");
		GenerateAllRooms();
	}

	private void GenerateDeadEndsList()
	{
		for (int deadEndsGenerated = 0; deadEndsGenerated < numberOfDeadEnds; deadEndsGenerated++)
		{
			int randX = Random.Range(0, levelWidthInRooms - 1);
			int randY = Random.Range(0, levelHeightInRooms - 1);

			if(CheckRoom(randX, randY) == null)
			{
				Vector2Int deadEndPos = new Vector2Int(randX, randY);
				if ((deadEndPos + Vector2Int.left) != startingRoom &&
					(deadEndPos + Vector2Int.right) != endingRoom)
				{
					// add dead end to list
					deadEndGrids.Add(deadEndPos);
					//initialize blank room
					InitializeRoom(randX, randY, Room.Type.Blank);
				}
				// ensure we always generate at least the desired amount of blank rooms
				else deadEndsGenerated--;	
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

		// select and return appropriate Room.Type
		return SelectRoomType(up, down, left, right);
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

	private void SetPoligonColliderBounds()
	{
		polyCollider.points = new Vector2[] {
			new Vector2Int(0-(roomWidth/2), 0-(roomHeight/2)),	// bottom left
			new Vector2Int(0-(roomWidth/2), (roomHeight * levelHeightInRooms) - (roomHeight/2)),	// top left
			new Vector2Int((roomWidth * levelWidthInRooms) - (roomWidth/2), (roomHeight * levelHeightInRooms) - (roomHeight/2)),   // top right
			new Vector2Int((roomWidth * levelWidthInRooms) - (roomWidth/2), 0-(roomHeight/2))	// bottom right
		};
	}
}
