using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    int timeBeforeRespawn = 2;

    List<TorchMarker> torches = new List<TorchMarker>();
    public SpawnPosition[] spawnPositions;
    public List<EnemyAI> activeEnemies = new List<EnemyAI>();

    void Start()
    {
        spawnPositions = FindObjectsOfType<SpawnPosition>();
        CollectActiveEnemies();
    }

    void CollectActiveEnemies()
	{
        activeEnemies = new List<EnemyAI>();

        foreach (EnemyAI enemy in FindObjectsOfType<EnemyAI>())
		{
            activeEnemies.Add(enemy);
		}
	}

    public void GenerateTorch(TorchMarker torchObject)
	{
        torches.Add(torchObject);
		torchObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = torches.Count;
    }

    public void TriggerReset() => StartCoroutine(ResetLevelRoutine());

    IEnumerator ResetLevelRoutine()
	{
		yield return new WaitForSeconds(timeBeforeRespawn);
		ResetLevel();
	}

	private void ResetLevel()
	{
		foreach (EnemyAI enemy in activeEnemies)
		{
			// Idk why this works but if I don't do a null check here the respawns break
			if (enemy == null) continue;
			enemy.DestroySelf();
		}
		foreach (SpawnPosition spawnPos in spawnPositions)
		{
			spawnPos.SpawnReset();
		}
		foreach (TorchMarker torch in torches)
		{
			torch.ResetFlame();
		}

		CollectActiveEnemies();
	}
}
