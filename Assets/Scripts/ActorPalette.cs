using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Actor Palette",
    menuName = "ScriptableObjects/Actor Palette Object", order = 1)]
public class ActorPalette : ScriptableObject
{
    public GameObject player;
    public GameObject meleeEnemy;
}
