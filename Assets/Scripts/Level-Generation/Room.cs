using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    public RoomPalette roomPalette;
    [SerializeField]
    public Type roomType;

    private GameObject roomToSpawn;

    public enum Type
    {
        StartRoom,
        EndRoom,
        LR,
        UD,
        LRUD,
        LRU,
        LRD,
        LUD,
        RUD,
        LD,
        LU,
        RU,
        RD,
        L,
        R,
        U,
        D,
        Blank,
        Exit
    }

	private void Start()
	{
        //check if this is attached to a Grid parent, if not, delete (garbage collection)
        if (!GetComponentInParent<Grid>()) Destroy(gameObject);
	}

	public void CreateRoom(int xSpawnPos, int ySpawnPos, Transform parent)
    {
        switch (roomType)
        {
            case Type.StartRoom:
                roomToSpawn = roomPalette.startRooms[Random.Range(0, roomPalette.startRooms.Length)];
                break;
            case Type.EndRoom:
                roomToSpawn = roomPalette.endRooms[Random.Range(0, roomPalette.endRooms.Length)];
                break;
            case Type.LR:
                roomToSpawn = roomPalette.LRRooms[Random.Range(0, roomPalette.LRRooms.Length)];
                break;
            case Type.UD:
                roomToSpawn = roomPalette.UDRooms[Random.Range(0, roomPalette.UDRooms.Length)];
                break;
            case Type.LRUD:
                roomToSpawn = roomPalette.LRUDRooms[Random.Range(0, roomPalette.LRUDRooms.Length)];
                break;
            case Type.LRU:
                roomToSpawn = roomPalette.LRURooms[Random.Range(0, roomPalette.LRURooms.Length)];
                break;
            case Type.LRD:
                roomToSpawn = roomPalette.LRDRooms[Random.Range(0, roomPalette.LRDRooms.Length)];
                break;
            case Type.LUD:
                roomToSpawn = roomPalette.LUDRooms[Random.Range(0, roomPalette.LUDRooms.Length)];
                break;
            case Type.RUD:
                roomToSpawn = roomPalette.RUDRooms[Random.Range(0, roomPalette.RUDRooms.Length)];
                break;
            case Type.LD:
                roomToSpawn = roomPalette.LDRooms[Random.Range(0, roomPalette.LDRooms.Length)];
                break;
            case Type.LU:
                roomToSpawn = roomPalette.LURooms[Random.Range(0, roomPalette.LURooms.Length)];
                break;
            case Type.RU:
                roomToSpawn = roomPalette.RURooms[Random.Range(0, roomPalette.RURooms.Length)];
                break;
            case Type.RD:
                roomToSpawn = roomPalette.RDRooms[Random.Range(0, roomPalette.RDRooms.Length)];
                break;
            case Type.L:
                roomToSpawn = roomPalette.LRooms[Random.Range(0, roomPalette.LRooms.Length)];
                break;
            case Type.R:
                roomToSpawn = roomPalette.RRooms[Random.Range(0, roomPalette.RRooms.Length)];
                break;
            case Type.U:
                roomToSpawn = roomPalette.URooms[Random.Range(0, roomPalette.URooms.Length)];
                break;
            case Type.D:
                roomToSpawn = roomPalette.DRooms[Random.Range(0, roomPalette.DRooms.Length)];
                break;
            case Type.Blank:
                roomToSpawn = roomPalette.blankRoom;
                break;
            case Type.Exit:
                roomToSpawn = roomPalette.exitRoom;
                break;
        }

        Instantiate(roomToSpawn, new Vector2(xSpawnPos, ySpawnPos), Quaternion.identity, parent);
    }
}
