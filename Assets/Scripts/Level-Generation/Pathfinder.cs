using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Pathfinding using Breadth First Search with Early Exit
// Researched/Implimented from RedBlobGames Python pathfinding blogs
// https://www.redblobgames.com/pathfinding/a-star/introduction.html

public class Pathfinder
{
	private Room[,] map;
	private Vector2Int startingRoom;
	private Vector2Int endingRoom;
	private List<Vector2Int> deadEndRooms;

	private Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
	private Queue<Vector2Int> queue = new Queue<Vector2Int>();
	private List<Vector2Int> path = new List<Vector2Int>();
	private Vector2Int searchCenter;

	private bool isRunning = true;

	private Vector2Int[] directions = {
		Vector2Int.up, 
		Vector2Int.down, 
		Vector2Int.left, 
		Vector2Int.right };

	public Pathfinder(Room[,] _map, Vector2Int _startingRoom, Vector2Int _endingRoom, List<Vector2Int> _deadEndGrids)
	{
		map = _map;
		startingRoom = _startingRoom += Vector2Int.right;
		endingRoom = _endingRoom += Vector2Int.left;
		deadEndRooms = _deadEndGrids;
	}

	public List<Vector2Int> CalculatePath()
	{
		BreadthFirstSearch();

		while (!cameFrom.ContainsKey(endingRoom))
		{
			Debug.LogWarning("No path found! Destroying random Dead End and trying again!");
			queue.Clear();
			cameFrom = new Dictionary<Vector2Int, Vector2Int>();

			deadEndRooms.Remove(deadEndRooms[Random.Range(0, deadEndRooms.Count)]);
			Debug.LogWarning("dead ends left: "+deadEndRooms.Count);
			BreadthFirstSearch();
		}
		Debug.LogWarning("Path completed: " + cameFrom.ContainsKey(endingRoom));

		CreatePath();

		return path;
	}

	private void BreadthFirstSearch()
	{
		Debug.Log("Running BFS");
		Vector2Int secondRoom = startingRoom;
		cameFrom.Add(secondRoom, startingRoom);

		queue.Enqueue(secondRoom);
		while (queue.Count > 0 && isRunning)
		{
			searchCenter = queue.Dequeue();
			HaltIfFound();
			ExploreNeighbors();
		}
	}

	private void HaltIfFound()
	{
		if (searchCenter == endingRoom) isRunning = false;
	}

	private void ExploreNeighbors()
	{
		if (!isRunning) return;

		foreach (Vector2Int direction in directions)
		{
			Vector2Int neighborCoodinates = searchCenter + direction;
			if (!cameFrom.ContainsKey(neighborCoodinates))
			{
				QueueNewNeighbors(neighborCoodinates);
			}
		}
	}

	private void QueueNewNeighbors(Vector2Int neighborCoords)
	{
		if (queue.Contains(neighborCoords)) return;
		else
		{
			if (InBounds(neighborCoords))
			{
				// check for valid space against starting room and dead ends
				if (deadEndRooms.Contains(neighborCoords)) return;
				if (neighborCoords == endingRoom + Vector2Int.right) return;
				if (neighborCoords == startingRoom + Vector2Int.left) return;

				queue.Enqueue(neighborCoords);
				cameFrom.Add(neighborCoords, searchCenter);
			}
		}
	}

	private bool InBounds(Vector2Int pos)
	{
		return 0 <= pos.x && pos.x < map.GetLength(0) &&
			0 <= pos.y && pos.y < map.GetLength(1);
	}

	private void CreatePath()
	{
		path.Add(endingRoom);

		Vector2Int prevPoint = cameFrom[endingRoom];

		while (prevPoint != startingRoom)
		{
			path.Add(prevPoint);
			prevPoint = cameFrom[prevPoint];
		}

		path.Add(startingRoom);

		path.Reverse();
	}
}