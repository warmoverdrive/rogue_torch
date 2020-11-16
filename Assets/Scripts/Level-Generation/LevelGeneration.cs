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
		roomHeight = 18;

	public int numberOfBlankRooms,
		castleWidthInRooms,
		castleHeightInRooms;

	private PolygonCollider2D polyCollider;

    private Room[,] castleMap;
    private Transform parentObject;

	private List<Vector2Int> path;

	//storage Vector2Ints of key points for pathfinding
	private Vector2Int startingRoom;
	private Vector2Int endingRoom;
	private List<Vector2Int> blankRoomGrids = new List<Vector2Int>();

    private void Awake()
	{
		GetGenerationSettings();

		polyCollider = GetComponent<PolygonCollider2D>();
		SetPoligonColliderBounds();

		parentObject = transform.GetComponentInParent<Transform>();

		castleMap = new Room[castleWidthInRooms, castleHeightInRooms];

		CreateStartAndEndRooms();

		GenerateBlankRoomsList();

		GeneratePath();

		DrawPathRooms();

		FillRemainingRooms();

		GenerateAllRooms();
	}

	private void GetGenerationSettings()
	{
		var levelGenManager = FindObjectOfType<LevelGenManager>();
		if (levelGenManager == null) Debug.LogError("No Level Gen Manager Found!");

		numberOfBlankRooms = levelGenManager.blankRooms;
		castleWidthInRooms = levelGenManager.castleWidthInRooms;
		castleHeightInRooms = levelGenManager.castleHeightInRooms;
	}

	private void GeneratePath()
	{
		Pathfinder pathfinder = new Pathfinder(castleMap, startingRoom, endingRoom, blankRoomGrids);
		path = pathfinder.CalculatePath();
	}

	private void DrawPathRooms()
	{
		foreach (var point in path)
		{
			InitializeRoom(point.x, point.y, Room.Type.LRUD);
		}
		foreach (var point in path)
		{
			InitializeRoom(point.x, point.y, CheckNeighbors(point.x, point.y, false));
		}
	}

	private void CreateStartAndEndRooms()
	{
		startingRoom = new Vector2Int(0, Random.Range(0, castleHeightInRooms));
		InitializeRoom(startingRoom.x, startingRoom.y, Room.Type.StartRoom);

		endingRoom = new Vector2Int(castleWidthInRooms - 1, Random.Range(0, castleHeightInRooms));
		InitializeRoom(endingRoom.x, endingRoom.y, Room.Type.EndRoom);

		var exitRoomCoords = new Vector2Int(endingRoom.x + 1, endingRoom.y);
		GenerateExitRoom(InitializeExitRoom(), exitRoomCoords.x, exitRoomCoords.y);	
	}

	private void GenerateBlankRoomsList()
	{
		for (int blankRoomsGenerated = 0; blankRoomsGenerated < numberOfBlankRooms; blankRoomsGenerated++)
		{
			int randX = Random.Range(0, castleWidthInRooms - 1);
			int randY = Random.Range(0, castleHeightInRooms - 1);

			if(CheckRoom(randX, randY) == null)
			{
				Vector2Int blankRoomPos = new Vector2Int(randX, randY);
				if ((blankRoomPos + Vector2Int.left) != startingRoom &&
					(blankRoomPos + Vector2Int.right) != endingRoom)
				{
					blankRoomGrids.Add(blankRoomPos);
					InitializeRoom(randX, randY, Room.Type.Blank);
				}
				else blankRoomsGenerated--;	
			}
		}
	}

	private Room.Type CheckNeighbors(int currentX, int currentY, bool deadEnd)
	{
		bool up, down, left, right;

		// Check Up
		if (currentY + 1 < castleHeightInRooms) // check OOB (out of bounds)
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
		if (currentX + 1 < castleWidthInRooms) // check OOB (out of bounds)
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
		for (int y = 0; y < castleHeightInRooms; y++)
		{
			for (int x = 0; x < castleWidthInRooms; x++)
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
		for (int y = 0; y < castleHeightInRooms; y++)
		{
			for (int x = 0; x < castleWidthInRooms; x++)
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

	private Room InitializeExitRoom()
	{
		Room exitRoom = new GameObject().AddComponent<Room>();
		exitRoom.roomPalette = roomArchetype.roomPalette;
		exitRoom.roomType = Room.Type.Exit;

		return exitRoom;
	}

	private void ChangeRoomType(int gridX, int gridY, Room.Type roomType) { castleMap[gridX, gridY].roomType = roomType; }

    private void GenerateRoom(int gridX, int gridY)
    {
        castleMap[gridX, gridY].CreateRoom(gridX * roomWidth, gridY * roomHeight, parentObject);
    }

	private void GenerateExitRoom(Room exitRoom, int gridX, int gridY)
	{
		exitRoom.CreateRoom(gridX * roomWidth, gridY * roomHeight, parentObject);
	}

    private Room CheckRoom(int gridX, int gridY)
    {
        if (castleMap[gridX, gridY]) return castleMap[gridX, gridY];
        else return null;
    }

	private void SetPoligonColliderBounds()
	{
		polyCollider.points = new Vector2[] {
			new Vector2Int(0-(roomWidth/2), 0-(roomHeight/2)),	// bottom left
			new Vector2Int(0-(roomWidth/2), (roomHeight * castleHeightInRooms) - (roomHeight/2)),	// top left
			new Vector2Int((roomWidth * castleWidthInRooms) - (roomWidth/2), (roomHeight * castleHeightInRooms) - (roomHeight/2)),   // top right
			new Vector2Int((roomWidth * castleWidthInRooms) - (roomWidth/2), 0-(roomHeight/2))	// bottom right
		};
	}
}
