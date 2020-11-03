using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPosition : MonoBehaviour
{
    [SerializeField]
    ActorPalette actorPalette;

    [SerializeField]
    SpawnType actorToSpawn;

    public enum SpawnType
	{ 
        Player,
        MeleeEnemy
    }

    // Start is called before the first frame update
    void Start()
	{
		SpawnActor();
	}

	private void SpawnActor()
	{
		switch (actorToSpawn)
		{
			case SpawnType.Player:
				Instantiate(actorPalette.player, transform.position, Quaternion.identity, gameObject.transform);
				break;

			case SpawnType.MeleeEnemy:
				Instantiate(actorPalette.meleeEnemy, transform.position, Quaternion.identity, gameObject.transform);
				break;
		}
	}

	public void SpawnReset()
	{
		SpawnActor();
	}
}