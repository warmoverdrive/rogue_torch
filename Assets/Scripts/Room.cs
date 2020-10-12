using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    RoomPalette roomPalette;
    [SerializeField]
    public Type roomType;

    private GameObject roomToSpawn;

    public enum Type
    {
        LR,
        LRD,
        LRU,
        LRUD,
        UD,
        L,
        R,
        U,
        D,
        StartRoom
    }

    public void CreateRoom(int xSpawnPos, int ySpawnPos, Transform parent)
    {
        switch (roomType)
        {
            case Type.LR:
                roomToSpawn = roomPalette.LRRooms[Random.Range(0, roomPalette.LRRooms.Length)];
                break;
            case Type.LRD:
                roomToSpawn = roomPalette.LRDRooms[Random.Range(0, roomPalette.LRDRooms.Length)];
                break;
            case Type.LRU:
                roomToSpawn = roomPalette.LRURooms[Random.Range(0, roomPalette.LRURooms.Length)];
                break;
            case Type.LRUD:
                roomToSpawn = roomPalette.LRUDRooms[Random.Range(0, roomPalette.LRUDRooms.Length)];
                break;
            case Type.UD:
                roomToSpawn = roomPalette.UDRooms[Random.Range(0, roomPalette.UDRooms.Length)];
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
            case Type.StartRoom:
                roomToSpawn = roomPalette.startRoom;
                break;
        }

        Instantiate(roomToSpawn, new Vector2(xSpawnPos, ySpawnPos), Quaternion.identity, parent);
    }
}
