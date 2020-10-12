using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Room Palette", 
    menuName = "ScriptableObjects/Room Palette Object", order = 1)]
public class RoomPalette : ScriptableObject
{
    public GameObject startRoom;
    public GameObject[] LRRooms,
        LRDRooms,
        LRURooms,
        LRUDRooms,
        UDRooms,
        LRooms,
        RRooms,
        URooms,
        DRooms;
}