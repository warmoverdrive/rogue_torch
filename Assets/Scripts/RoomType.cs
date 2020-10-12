using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomType : MonoBehaviour
{
    [SerializeField]
    public Type roomType;

    public enum Type
    {
        LR,
        LRD,
        LRU,
        LRUD,
        UD
    }

    public void DestroyRoom() { Destroy(gameObject); }
}
