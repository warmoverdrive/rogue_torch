using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Room Palette", 
    menuName = "ScriptableObjects/Room Palette Object", order = 1)]
public class RoomPalette : ScriptableObject
{
    public GameObject blankRoom, 
        exitRoom;
    public GameObject[] startRooms,
        endRooms,
        LRRooms,
        UDRooms,
        LRUDRooms,
        LRURooms,
        LRDRooms,
        LUDRooms,
        RUDRooms,
        LDRooms,
        LURooms,
        RURooms,
        RDRooms,
        LRooms,
        RRooms,
        URooms,
        DRooms;
}